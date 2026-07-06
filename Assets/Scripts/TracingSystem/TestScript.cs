using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TracingSystem
{
    public class TestScript : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        
        public AssetReferenceSprite spriteRef;
        
        public Sprite sprite;
        
        private async Task Start()
        {
            await spriteRef.LoadAssetAsync<Sprite>().Task;
            spriteRenderer.sprite = spriteRef.Asset as Sprite;
        }
        
        [ContextMenu("Test")]
        public void TestMethod()
        {
            spriteRenderer.sprite = spriteRef.editorAsset as Sprite;
        }   
    }
}