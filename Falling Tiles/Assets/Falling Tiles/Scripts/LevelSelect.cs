using UnityEngine;
using UnityEngine.SceneManagement;

namespace FallingTiles
{
    [AddComponentMenu("Falling Tiles/Level Select")]
    [DisallowMultipleComponent]
    public class LevelSelect : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private LevelOrderData _levels = null;
        [SerializeField] private Transform _levelSelectList = null;
        [SerializeField] private LevelSelection _levelSelectionPrefab = null;
        #endregion

        #region Unity Events
        private void Start()
        {
            Account.ShowLevelsIfAuthenticated();
        }
        #endregion

        #region Public Methods
        public void GenerateSelectionList()
        {
            int levelCount = _levels.Levels.Count;
            for (int i = 0; i < levelCount; i++)
            {
                LevelSelection selection = Instantiate(_levelSelectionPrefab, _levelSelectList);
                selection.Initialize(i + 1, $"{Account.SaveData.Scores[i]} / {_levels.Levels[i].TileCount} Tiles", this);
            }
        }

        public void Select(int index)
        {
            Level.Data = _levels.Levels[index];
            SceneManager.LoadScene("Level");
        }
        #endregion
    }
}