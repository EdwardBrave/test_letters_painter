using UnityEngine;

namespace UI
{
    public class LevelCategoryView : MonoBehaviour
    {
        [SerializeField] private Transform levelsContainer;
        public Transform LevelsContainer => levelsContainer;
        
    }
}