using UnityEngine;
using DG.Tweening;

namespace FallingTiles
{
    [AddComponentMenu("Falling Tiles/Player")]
    [DisallowMultipleComponent]
    public class Player : MonoBehaviour
    {
        #region Fields and Properties
        private const float _spawnTime = 0.75f;
        private const float _spawnDelay = 0.25f;
        private const float _moveTime = 0.25f;
        private const float _fallTargetY = -5.5f;
        private const float _fallSpeed = 7.5f;

        private Vector2Int _cachedGridSize = new Vector2Int(1, 1);
        public Vector2Int GridPosition { get; private set; } = new Vector2Int();
        public bool IsDoneSpawning { get; private set; } = false;
        public bool IsFalling { get; private set; } = false;
        public bool IsDoneFalling { get; private set; } = false;
        public bool IsMoving { get; private set; } = false;
        #endregion

        #region Public Methods
        public void Spawn(Vector2Int gridPosition, Vector2Int gridSize)
        {
            gameObject.SetActive(true);
            transform.position = new Vector3(
                gridPosition.x - gridSize.x / 2 + (gridSize.x % 2 == 0 ? 0.5f : 0),
                gridPosition.y - gridSize.y / 2 + (gridSize.y % 2 == 0 ? 0.5f : 0),
                gridPosition.y - 0.2f);
            transform.DOScale(1, _spawnTime).From(0).SetDelay(_spawnDelay).OnComplete(() => IsDoneSpawning = true);
            _cachedGridSize = gridSize;
            GridPosition = gridPosition;
        }

        public void Move(Vector2Int gridPosition, bool shouldFall)
        {
            if (gridPosition.y < GridPosition.y)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, gridPosition.y - 0.2f);
            }

            var animation = transform.DOMove(
                new Vector3(gridPosition.x - _cachedGridSize.x / 2 + (_cachedGridSize.x % 2 == 0 ? 0.5f : 0),
                gridPosition.y - _cachedGridSize.y / 2 + (_cachedGridSize.y % 2 == 0 ? 0.5f : 0),
                transform.position.z),
                _moveTime);
            animation.onComplete += () => IsMoving = false;

            if (gridPosition.y > GridPosition.y)
            {
                animation.onComplete += () =>
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, gridPosition.y - 0.2f);
                };
            }

            GridPosition = gridPosition;
            IsFalling = shouldFall;
            IsMoving = true;
        }

        public void Fall()
        {
            transform.localPosition -= new Vector3(0, Time.deltaTime * _fallSpeed, 0);
            if (transform.localPosition.y < _fallTargetY)
            {
                IsDoneFalling = true;
                gameObject.SetActive(false);
            }
        }
        #endregion
    }
}