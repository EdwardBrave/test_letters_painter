using TMPro;
using UnityEngine;

namespace UI.Menu
{
    public class LevelCategoryView : MonoBehaviour
    {
        [SerializeField] private Transform levelsContainer;
        [SerializeField] private TMP_Text text;
        public Transform LevelsContainer => levelsContainer;
        public TMP_Text Text => text;
        
    }
}