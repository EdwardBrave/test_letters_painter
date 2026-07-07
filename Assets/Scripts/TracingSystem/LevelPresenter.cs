using System;
using TracingSystem.Model;
using TracingSystem.View;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace TracingSystem
{
    public class LevelPresenter : IInitializable, IDisposable
    {
        private LevelView _view;
        private LevelModel _model;
        
        private AssetReferenceSprite _shapeAssetRef;
        private AssetReferenceT<AudioClip> _goalAudioRef;
        
        public LevelPresenter(LevelModel levelModel, LevelView levelView)
        {
            _view = levelView;
            _model = levelModel;
            
            _shapeAssetRef = new AssetReferenceSprite(_model.shapeAssetGUID);
            _goalAudioRef = new AssetReferenceT<AudioClip>(_model.goalAudioAssetGUID);
        }
        
        public void Initialize()
        {
            _shapeAssetRef.LoadAssetAsync<Sprite>().Completed += HandleShapeAssetLoaded;
        }

        private void HandleShapeAssetLoaded(AsyncOperationHandle<Sprite> handle)
        {
            _view.shapeMaskView.UpdateSprite(handle.Result);
        }
        
        public void Dispose()
        {
            _shapeAssetRef.ReleaseAsset();
            _goalAudioRef.ReleaseAsset();
        }
    }
}