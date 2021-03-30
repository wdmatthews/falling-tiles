using UnityEngine;

namespace FallingTiles
{
    [CreateAssetMenu(fileName = "New Tile", menuName = "Falling Tiles/Tile")]
    public class TileData : ScriptableObject
    {
        public string Symbol = "";
        public Tile Prefab = null;
        public int Durability = 1;
    }
}