using System.Collections.Generic;
using StringBuilder = System.Text.StringBuilder;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

namespace FallingTiles.Editor
{
    [AddComponentMenu("Falling Tiles/Level Editor")]
    [DisallowMultipleComponent]
    public class LevelEditor : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private LevelData _data = null;
        [SerializeField] private Player _player = null;
        [SerializeField] private Transform _entrancePortal = null;
        [SerializeField] private Transform _exitPortal = null;
        [SerializeField] private LevelEditorTile _tilePrefab = null;

        private SerializedObject _serializedData = null;
        private SerializedProperty _serializedPlayerSpawn = null;
        private SerializedProperty _serializedExitPortal = null;
        private SerializedProperty _serializedTilemap = null;
        private SerializedProperty _serializedTileCount = null;
        private Dictionary<Vector2Int, LevelEditorTile> _tilesByPosition = new Dictionary<Vector2Int, LevelEditorTile>();
        private bool _entrancePlaced = false;
        private bool _exitPlaced = false;
        private int _tileCount = 0;
        #endregion

        #region Unity Events
        private void Awake()
        {
            _entrancePortal.SetParent(_player.transform);
            _exitPortal.SetParent(_player.transform);
            _player.Spawn(Vector2Int.zero, _data.GridSize);
            _serializedData = new SerializedObject(_data);
            _serializedPlayerSpawn = _serializedData.FindProperty(nameof(LevelData.PlayerSpawn));
            _serializedExitPortal = _serializedData.FindProperty(nameof(LevelData.ExitPortal));
            _serializedTilemap = _serializedData.FindProperty(nameof(LevelData.Tilemap));
            _serializedTileCount = _serializedData.FindProperty(nameof(LevelData.TileCount));
        }
        #endregion

        #region Public Methods
        public void OnSpace(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (!_exitPlaced) PlaceExit();
            else if (!_entrancePlaced) PlaceEntrance();
        }

        public void MovePlayerLeft(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            MovePlayer(new Vector2Int(-1, 0));
        }

        public void MovePlayerRight(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            MovePlayer(new Vector2Int(1, 0));
        }

        public void MovePlayerDown(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            MovePlayer(new Vector2Int(0, -1));
        }

        public void MovePlayerUp(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            MovePlayer(new Vector2Int(0, 1));
        }
        #endregion

        #region Private Methods
        private void PlaceEntrance()
        {
            _entrancePlaced = true;
            _entrancePortal.SetParent(null);
            _entrancePortal.gameObject.SetActive(true);

            _serializedPlayerSpawn.vector2IntValue = _player.GridPosition;
            _serializedTilemap.stringValue = GenerateTilemap();
            _serializedTileCount.intValue = _tileCount - 1;
            _serializedData.ApplyModifiedProperties();
        }

        private void PlaceExit()
        {
            _exitPlaced = true;
            Vector2Int position = _player.GridPosition;
            _exitPortal.localPosition -= new Vector3(0, 0.5f, 0);
            _exitPortal.SetParent(PlaceTile(position).transform);
            _exitPortal.gameObject.SetActive(true);
            _serializedExitPortal.vector2IntValue = position;
        }

        private LevelEditorTile PlaceTile(Vector2Int position)
        {
            LevelEditorTile newTile = Instantiate(_tilePrefab, transform);
            newTile.Place(position, _data.GridSize);
            _tilesByPosition.Add(position, newTile);
            _tileCount++;
            return newTile;
        }

        private void MovePlayer(Vector2Int direction)
        {
            if (_player.IsMoving) return;

            Vector2Int oldPosition = _player.GridPosition;
            Vector2Int newPosition = oldPosition + direction;

            if (newPosition.x < 0 || newPosition.x >= _data.GridSize.x
                || newPosition.y < 0 || newPosition.y >= _data.GridSize.y) return;

            _player.Move(newPosition, false);
            
            if (_exitPlaced && !_entrancePlaced)
            {
                if (_tilesByPosition.ContainsKey(newPosition))
                {
                    LevelEditorTile tile = _tilesByPosition[newPosition];
                    tile.IncreaseDurability(_data.Tiles[tile.Durability].Prefab
                        .GetComponent<SpriteRenderer>().sprite);
                }
                else PlaceTile(_player.GridPosition);
            }
        }

        private string GenerateTilemap()
        {
            StringBuilder tilemap = new StringBuilder();
            int width = _data.GridSize.x;
            int height = _data.GridSize.y;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int position = new Vector2Int(x, height - y - 1);
                    LevelEditorTile tile = _tilesByPosition.ContainsKey(position) ? _tilesByPosition[position] : null;
                    tilemap.Append(tile ? _data.Tiles[tile.Durability - 1].Symbol : " ");
                }

                if (y < height - 1) tilemap.AppendLine();
            }

            return tilemap.ToString();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_data.GridSize.x, _data.GridSize.y, 0.1f));
        }
        #endregion
    }
}