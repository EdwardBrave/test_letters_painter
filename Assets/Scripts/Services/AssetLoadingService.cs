using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Tools;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Services
{
    
    public class AssetLoadingService : IInitializable, IDisposable
    {
        private readonly Dictionary<IDisposeNotifier, LoadedContext> _loadedContexts = new();
        private CancellationTokenSource _cts;
        private bool _disposed;

        public void Initialize()
        {
            _cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            
            foreach (var pair in _loadedContexts)
            {
                pair.Key.OnDispose -= pair.Value.Handler;
                pair.Value.Dispose();
            }
            
            _loadedContexts.Clear();

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
        
        public ContextLoadingOperation StartAsyncContextLoading(CancellationToken token = default)
        {
            if (_disposed || _cts == null)
            {
                throw new ObjectDisposedException(nameof(AssetLoadingService));
            }

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, token);
            
            return new ContextLoadingOperation(this, linked.Token);
        }
        
        private void RegisterLoadedContext(IDisposeNotifier binding, LoadedContext context)
        {
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(AssetLoadingService));
            }
            
            context.Handler = () => ReleaseLoadedContext(binding);
            _loadedContexts.Add(binding, context);
            binding.OnDispose += context.Handler;
        }
        
        private void ReleaseLoadedContext(IDisposeNotifier binding)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AssetLoadingService));
            
            if (_loadedContexts.TryGetValue(binding, out var context))
            {
                binding.OnDispose -= context.Handler;
                context.Dispose();
                _loadedContexts.Remove(binding);
            }
        }
        
        public class ContextLoadingOperation : IDisposable
        {
            private readonly AssetLoadingService _service;
            private readonly CancellationTokenSource _cts;
            
            private readonly LoadedContext _context;
            private IDisposeNotifier _binding;
            
            public ContextLoadingOperation(AssetLoadingService service, CancellationToken token)
            {
                _service = service;
                _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                _context = new LoadedContext();
            }
            
            public async UniTask<TAsset> LoadAssetAsync<TAsset>(string assetKey)
            {
                var assetHandle = Addressables.LoadAssetAsync<TAsset>(assetKey);
                _context.AddHandle(assetHandle);
                return await assetHandle.ToUniTask(cancellationToken: _cts.Token);
            }

            public void Bind(IDisposeNotifier notifier)
            {
                _binding = notifier;
            }

            public void Dispose()
            {
                _cts?.Cancel();
                _cts?.Dispose();
                
                if (!_context.IsAllLoaded)
                {
                    _context.Dispose();
                    throw new InvalidOperationException("Not all assets loaded but operation termination is attempted");
                }
                
                if (_binding != null)
                {
                    _service.RegisterLoadedContext(_binding, _context);
                }
                else
                {
                    _context.Dispose();
                }
            }
        }

        public class LoadedContext : IDisposable
        {
            private List<AsyncOperationHandle> _handles = new ();

            public Action Handler { get; set; }

            public bool IsAllLoaded => _handles.All(handle => handle.Status == AsyncOperationStatus.Succeeded);

            public void AddHandle(AsyncOperationHandle handle)
            {
                _handles.Add(handle);
            }
            
            public void Dispose()
            {
                Handler = null;

                foreach (var handle in _handles)
                {
                    handle.Release();
                }
            }
        }
    }
}
