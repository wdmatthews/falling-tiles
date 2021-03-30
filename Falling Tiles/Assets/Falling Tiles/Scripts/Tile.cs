using UnityEngine;
using DG.Tweening;

namespace FallingTiles
{
    [AddComponentMenu("Falling Tiles/Tile")]
    [DisallowMultipleComponent]
    public class Tile : MonoBehaviour
    {
        #region Fields and Properties
        private const float _totalScaleLoss = 0.5f;
        private const float _shrinkTime = 0.15f;
        private const float _fallTargetY = -5.5f;
        private const float _fallSpeed = 7.5f;

        public int Durability { get; private set; }  = 1;
        private int _maxDurability = 1;
        public bool AnimateDurability { get; private set; } = false;
        #endregion

        #region Public Methods
        public void Initialize(TileData data, int x, int y, Vector2Int gridSize)
        {
            transform.position = new Vector3(
                x - gridSize.x / 2 + (gridSize.x % 2 == 0 ? 0.5f : 0),
                y - gridSize.y / 2 + (gridSize.y % 2 == 0 ? 0.5f : 0),
                y);
            _maxDurability = Durability = data.Durability;
        }

        public void Animate()
        {
            if (Durability == 0)
            {
                transform.localPosition -= new Vector3(0, Time.deltaTime * _fallSpeed, 0);
                if (transform.localPosition.y < _fallTargetY)
                {
                    AnimateDurability = false;
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Decreases the durability of the tile.
        /// If there is no more durability, then the tile will fall.
        /// Returns if the tile fell.
        /// </summary>
        /// <returns></returns>
        public bool DecreaseDurability()
        {
            Durability--;

            if (Durability == 0) AnimateDurability = true;
            else
            {
                float targetScale = 1 - _totalScaleLoss * (_maxDurability - Durability + 1) / _maxDurability;
                transform.DOScale(targetScale, _shrinkTime);
            }

            return Durability == 0;
        }
        #endregion
    }
}