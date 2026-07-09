using System;
using Services;

namespace Tools
{
    /// <summary>
    /// Lifecycle-only contract for objects that announce their own end-of-life.
    /// Disposing raises <see cref="OnDispose"/> exactly once; the object does not
    /// own or release any external resources itself — interested infrastructure
    /// (e.g. <see cref="AssetLoadingService"/>) subscribes to react to disposal.
    /// </summary>
    public interface IDisposeNotifier : IDisposable
    {
        event Action OnDispose;
    }
}
