using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button buttonPlay;
    [SerializeField] private Button buttonSettings;
    [SerializeField] private Button buttonExit;
    [SerializeField] private string sceneToLoad;

    private bool _isTransitioning;


    private void Awake()
    {
        buttonPlay.onClick.AddListener(OnButtonPlayClicked);
        buttonSettings.onClick.AddListener(OnButtonSettingsClicked);
        buttonExit.onClick.AddListener(OnButtonExitClicked);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnButtonPlayClicked()
    {
        if (_isTransitioning)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError("No destination scene assigned!", this);
            return;
        }

        _isTransitioning = true;
        Time.timeScale = 1;
        EventManager.Instance?.ChangeCursor();
        AkSoundEngine.SetState("Game_State", "Lvl");
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnButtonSettingsClicked()
    {
        
    }
    
    private void OnButtonExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit(0);
#endif
    }
    
}
