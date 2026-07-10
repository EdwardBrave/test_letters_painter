using System.Collections.Generic;
using Game.Model;

namespace UI.Menu
{
    public class LevelCategoryModel
    {
        public LevelCategory Category { get; private set ; }
        public List<LightLevelModel> Levels { get; private set; }
        
        public LevelCategoryModel(LevelCategory category)
        {
            Category = category;
            Levels = new List<LightLevelModel>();
        }
        
        public void AddLevel(LightLevelModel level)
        {
            Levels.Add(level);
        }
    }
}