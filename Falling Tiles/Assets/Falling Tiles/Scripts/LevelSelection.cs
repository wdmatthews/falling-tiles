using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FallingTiles
{
    [AddComponentMenu("Falling Tiles/Level Selection")]
    [DisallowMultipleComponent]
    public class LevelSelection : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private TextMeshProUGUI _numberText = null;
        [SerializeField] private TextMeshProUGUI _progressText = null;
        [SerializeField] private Button _selectButton = null;

        private int _index = 0;
        #endregion

        #region Public Methods
        public void Initialize(int level, string progress, LevelSelect levelSelect)
        {
            _index = level - 1;
            _numberText.text = level.ToString();
            _progressText.text = progress;
            _selectButton.onClick.AddListener(() => levelSelect.Select(_index));
        }
        #endregion
    }
}