using UnityEngine;
using DG.Tweening;

namespace FallingTiles.Editor
{
    [AddComponentMenu("Falling Tiles/Level Editor Tile")]
    [DisallowMultipleComponent]
    public class LevelEditorTile : MonoBehaviour
    {
        #region Fields and Properties
        private const float _positionTime = 0.15f;

        [SerializeField] private SpriteRenderer _renderer = null;

        public int Durability { get; private set; } = 1;
        #endregion

        #region Public Methods
        public void Place(Vector2Int gridPosition, Vector2Int gridSize)
        {
            Vector3 targetPosition = new Vector3(
                gridPosition.x - gridSize.x / 2 + (gridSize.x % 2 == 0 ? 0.5f : 0),
                gridPosition.y - gridSize.y / 2 + (gridSize.y % 2 == 0 ? 0.5f : 0),
                gridPosition.y);
            Vector3 startingPosition = new Vector3(
                gridPosition.x - gridSize.x / 2 + (gridSize.x % 2 == 0 ? 0.5f : 0),
                gridPosition.y - gridSize.y / 2 - 0.5f + (gridSize.y % 2 == 0 ? 0.5f : 0),
                gridPosition.y);
            transform.DOLocalMove(targetPosition, _positionTime).From(startingPosition);
        }

        public void IncreaseDurability(Sprite sprite)
        {
            Durability++;
            _renderer.sprite = sprite ?? _renderer.sprite;
        }
        #endregion
    }
}