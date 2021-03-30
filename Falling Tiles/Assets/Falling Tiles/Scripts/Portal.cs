using UnityEngine;
using DG.Tweening;

namespace FallingTiles
{
    [AddComponentMenu("Falling Tiles/Portal")]
    [DisallowMultipleComponent]
    public class Portal : MonoBehaviour
    {
        #region Fields and Properties
        private const float _animationTime = 0.75f;
        private const float _animationDelay = 0.25f;

        [SerializeField] private Transform _leftDoor = null;
        [SerializeField] private Transform _rightDoor = null;
        #endregion

        #region Public Methods
        public void Initialize(Vector2Int gridPosition, Vector2Int gridSize)
        {
            gameObject.SetActive(true);
            transform.position = new Vector3(
                gridPosition.x - gridSize.x / 2 + (gridSize.x % 2 == 0 ? 0.5f : 0),
                gridPosition.y - gridSize.y / 2 + (gridSize.y % 2 == 0 ? 0.5f : 0),
                gridPosition.y - 0.1f);
        }

        public void Open()
        {
            _leftDoor.DOLocalMoveX(-0.75f, _animationTime).From(-0.25f);
            _rightDoor.DOLocalMoveX(0.75f, _animationTime).From(0.25f);
        }

        public void Close()
        {
            _leftDoor.DOLocalMoveX(-0.25f, _animationTime).From(-0.75f).SetDelay(_animationDelay);
            _rightDoor.DOLocalMoveX(0.25f, _animationTime).From(0.75f).SetDelay(_animationDelay);
        }
        #endregion
    }
}