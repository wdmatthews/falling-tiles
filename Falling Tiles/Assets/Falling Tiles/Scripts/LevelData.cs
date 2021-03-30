using System.Collections.Generic;
using UnityEngine;

namespace FallingTiles
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Falling Tiles/Level")]
    public class LevelData : ScriptableObject
    {
        #region Fields and Properties
        public Vector2Int GridSize = new Vector2Int(3, 3);
        public TileData[] Tiles = { };
        [Tooltip("Rows are separated by a new line. Columns are represented by a single character.")]
        [TextArea] public string Tilemap = "";
        public int TileCount = 0;
        public Vector2Int PlayerSpawn = new Vector2Int(0, 2);
        public Vector2Int ExitPortal = new Vector2Int(2, 0);

        private Dictionary<string, TileData> _tileDataBySymbol = null;
        #endregion

        #region Public Methods
        public TileData GetTile(string symbol)
        {
            if (_tileDataBySymbol == null) GenerateTileDataBySymbol();
            return _tileDataBySymbol[symbol];
        }
        #endregion

        #region Private Methods
        private void GenerateTileDataBySymbol()
        {
            _tileDataBySymbol = new Dictionary<string, TileData>();
            foreach (TileData tile in Tiles)
            {
                _tileDataBySymbol.Add(tile.Symbol, tile);
            }
        }
        #endregion
    }
}