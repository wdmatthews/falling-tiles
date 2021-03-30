using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace FallingTiles
{
    [AddComponentMenu("Falling Tiles/Account")]
    [DisallowMultipleComponent]
    public class Account : MonoBehaviour
    {
        #region Fields and Properties
        private static Account _instance = null;
        private static bool _authenticated = false;
        private static bool _authenticationSkipped = false;
        private static string _username = "";
        private static string _password = "";
        public static SaveData SaveData = null;

        [SerializeField] private Canvas _mainMenu = null;
        [SerializeField] private Canvas _levelSelectMenu = null;
        [SerializeField] private LevelSelect _levelSelect = null;
        [SerializeField] private TMP_InputField _usernameInput = null;
        [SerializeField] private TMP_InputField _passwordInput = null;
        [SerializeField] private TextMeshProUGUI _errorText = null;
        [SerializeField] private LevelOrderData _levels = null;
        #endregion

        #region Public Methods
        private void Awake()
        {
            if (_instance && _instance != this) Destroy(_instance.gameObject);
            else if (_instance == this) return;

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Login()
        {
            _username = _usernameInput.text;
            _password = _passwordInput.text;
            if (_username.Length == 0 || _password.Length == 0) return;
            StartCoroutine(AuthenticateAsync(true));
        }

        public void Register()
        {
            _username = _usernameInput.text;
            _password = _passwordInput.text;
            if (_username.Length == 0 || _password.Length == 0) return;
            StartCoroutine(AuthenticateAsync(false));
        }

        public void Skip()
        {
            _authenticationSkipped = true;
            Load();
            ShowLevels();
        }

        public static void Save()
        {
            if (_authenticated) _instance.StartCoroutine(SaveAsync());
            else
            {
                PlayerPrefs.SetString("Save", JsonUtility.ToJson(SaveData));
                PlayerPrefs.Save();
            }
        }

        public static void ShowLevelsIfAuthenticated()
        {
            if (_authenticated || _authenticationSkipped) _instance.ShowLevels();
        }
        #endregion

        #region Private Methods
        private void ShowLevels()
        {
            _mainMenu.enabled = false;
            _levelSelectMenu.enabled = true;
            _levelSelect.GenerateSelectionList();
        }

        private void Load()
        {
            if (_authenticated) StartCoroutine(LoadAsync());
            else
            {
                if (PlayerPrefs.HasKey("Save"))
                {
                    SaveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("Save"));
                }
                else
                {
                    SaveData = new SaveData();
                    SaveData.Scores = new int[_levels.Levels.Count];
                }
            }
        }

        private static IEnumerator AuthenticateAsync(bool login)
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("username", _username));
            formData.Add(new MultipartFormDataSection("password", _password));

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/" + (login ? "login" : "register"), formData))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success) Debug.Log(www.error);
                else if (www.downloadHandler.text == "fail")
                {
                    _instance._errorText.gameObject.SetActive(true);
                    _instance._errorText.text = "Authentication Failed";
                }
                else if (www.downloadHandler.text == "success")
                {
                    _authenticated = true;
                    _instance.Load();
                }
            }
        }

        private static IEnumerator LoadAsync()
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("username", _username));
            formData.Add(new MultipartFormDataSection("password", _password));

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/load", formData))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success) Debug.Log(www.error);
                else if (www.downloadHandler.text == "fail") Debug.Log("Load failed.");
                else
                {
                    if (www.downloadHandler.text.Length == 0)
                    {
                        SaveData = new SaveData();
                        SaveData.Scores = new int[_instance._levels.Levels.Count];
                    }
                    else SaveData = JsonUtility.FromJson<SaveData>(www.downloadHandler.text);
                    _instance.ShowLevels();
                }
            }
        }

        private static IEnumerator SaveAsync()
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("username", _username));
            formData.Add(new MultipartFormDataSection("password", _password));
            formData.Add(new MultipartFormDataSection("save", JsonUtility.ToJson(SaveData)));

            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/save", formData))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success) Debug.Log(www.error);
                else if (www.downloadHandler.text == "fail") Debug.Log("Save failed.");
            }
        }
        #endregion
    }
}