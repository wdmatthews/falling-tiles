using System.Collections.Generic;
using UnityEngine;

namespace FallingTiles
{
    [CreateAssetMenu(fileName = "Level Order", menuName = "Falling Tiles/Level Order")]
    public class LevelOrderData : ScriptableObject
    {
        public List<LevelData> Levels = new List<LevelData>();
    }
}