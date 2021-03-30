using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

namespace FallingTiles
{
    [AddComponentMenu("Falling Tiles/Level")]
    [DisallowMultipleComponent]
    public class Level : MonoBehaviour
    {
        #region Fields and Properties
        private const float _endScreenTime = 0.25f;
        public static LevelData Data = null;

        [SerializeField] private LevelOrderData _orderData = null;
        [SerializeField] private LevelData _data = null;
        [SerializeField] private Player _player = null;
        [SerializeField] private Portal _entrancePortal = null;
        [SerializeField] private Portal _exitPortal = null;
        [SerializeField] private TextMeshProUGUI _instructionsText = null;
        [SerializeField] private RectTransform _endWindow = null;
        [SerializeField] private TextMeshProUGUI _scoreText = null;

        private bool _loaded = false;
        private Tile[] _tiles = null;
        private int _tileCount = 0;
        private bool _gameOver = false;
        #endregion

        #region Unity Events
        private void Awake()
        {
            Load(Data);
            if (_data.name == "Level 1") _instructionsText.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_loaded) return;

            foreach (Tile tile in _tiles)
            {
                if (tile && tile.AnimateDurability) tile.Animate();
            }

            if (_player.IsFalling) _player.Fall();
            if (_player.IsDoneFalling) Retry();
        }
        #endregion

        #region Public Methods
        public void Load(LevelData data)
        {
            if (data) _data = data;
            GenerateTiles();
            SpawnPlayer();
            _loaded = true;
        }

        public void MovePlayerLeft(InputAction.CallbackContext context)
        {
            if (!context.performed || !_player.IsDoneSpawning) return;
            MovePlayer(new Vector2Int(-1, 0));
        }

        public void MovePlayerRight(InputAction.CallbackContext context)
        {
            if (!context.performed || !_player.IsDoneSpawning) return;
            MovePlayer(new Vector2Int(1, 0));
        }

        public void MovePlayerDown(InputAction.CallbackContext context)
        {
            if (!context.performed || !_player.IsDoneSpawning) return;
            MovePlayer(new Vector2Int(0, -1));
        }

        public void MovePlayerUp(InputAction.CallbackContext context)
        {
            if (!context.performed || !_player.IsDoneSpawning) return;
            MovePlayer(new Vector2Int(0, 1));
        }

        public void RetryFromKey()
        {
            if (!_gameOver && _player.IsDoneSpawning) Retry();
        }

        public void Retry() => SceneManager.LoadScene("Level");
        public void Menu() => SceneManager.LoadScene("Main Menu");
        public void Next()
        {
            int currentIndex = _orderData.Levels.IndexOf(_data);
            if (currentIndex + 1 < _orderData.Levels.Count)
            {
                Data = _orderData.Levels[currentIndex + 1];
                SceneManager.LoadScene("Level");
            }
            else Menu();
        }
        #endregion

        #region Private Methods
        private void GenerateTiles()
        {
            int width = _data.GridSize.x;
            int height = _data.GridSize.y;
            _tiles = new Tile[width * height];
            string[] rows = _data.Tilemap.Split('\n');

            for (int y = 0; y < height; y++)
            {
                string row = rows[height - y - 1];
                for (int x = 0; x < width; x++)
                {
                    string cell = row[x].ToString();
                    if (cell != " ")
                    {
                        TileData tileData = _data.GetTile(cell);
                        Tile tile = Instantiate(tileData.Prefab, transform);
                        tile.Initialize(tileData, x, y, _data.GridSize);
                        _tiles[GridToIndex(x, y)] = tile;
                        _tileCount++;
                    }
                }
            }
        }

        private void SpawnPlayer()
        {
            _player.Spawn(_data.PlayerSpawn, _data.GridSize);
            _entrancePortal.Initialize(_data.PlayerSpawn, _data.GridSize);
            _entrancePortal.transform.SetParent(_tiles[GridToIndex(_data.PlayerSpawn.x, _data.PlayerSpawn.y)].transform);
            _entrancePortal.Close();
            _exitPortal.Initialize(_data.ExitPortal, _data.GridSize);
            _exitPortal.transform.SetParent(_tiles[GridToIndex(_data.ExitPortal.x, _data.ExitPortal.y)].transform);
        }

        /// <summary>
        /// Converts a grid position to an index in the tiles array.
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        private int GridToIndex(int x, int y)
        {
            return y * _data.GridSize.x + x;
        }

        private void MovePlayer(Vector2Int direction)
        {
            if (_player.IsFalling || _player.IsDoneFalling || _player.IsMoving || _gameOver) return;
            Vector2Int oldPosition = _player.GridPosition;
            Vector2Int newPosition = oldPosition + direction;
            bool outsideGrid = newPosition.x < 0 || newPosition.x >= _data.GridSize.x
                || newPosition.y < 0 || newPosition.y >= _data.GridSize.y;

            if (outsideGrid) _player.Move(newPosition, true);
            else
            {
                var tile = _tiles[GridToIndex(newPosition.x, newPosition.y)];
                _player.Move(newPosition, !tile || tile.Durability == 0);
            }

            var oldTile = _tiles[GridToIndex(oldPosition.x, oldPosition.y)];
            oldTile.DecreaseDurability();

            if (newPosition.x == _data.ExitPortal.x
                && newPosition.y == _data.ExitPortal.y)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            _gameOver = true;
            int score = 0;

            foreach (Tile tile in _tiles)
            {
                if (tile && tile.Durability == 0) score++;
            }

            _endWindow.gameObject.SetActive(true);
            _scoreText.text = $"{score}/{_tileCount - 1}\nTiles";
            _endWindow.DOScale(1, _endScreenTime).From(0);

            Account.SaveData.Scores[_orderData.Levels.IndexOf(_data)] = score;
            Account.Save();
        }
        #endregion
    }
}