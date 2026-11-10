using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;


public class UIPause : MonoBehaviour
{
    [SerializeField] private Button buttonResume;
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private Button buttonRestart;
    [SerializeField] private Button buttonMainMenu;
    [SerializeField] private SceneField mainMenuScene;
    
    //[SerializeField] private Button buttonSettings;

    //[SerializeField] private Button buttonSettingsBack;

    [SerializeField] private CanvasGroup panelPause;
    //[SerializeField] private CanvasGroup panelSettings;

    private bool _isPause = false;

    public event Action GameTogglePouse;

    private void Awake()
    {
        versionText.text = $"{Application.version}";
        buttonResume.onClick.AddListener(OnButtonResumeClicked);
        buttonMainMenu.onClick.AddListener(OnButtonExitClicked);
        buttonRestart.onClick.AddListener(RestartScene);
        //buttonSettings.onClick.AddListener(OnButtonSettingsClicked);
        //buttonSettingsBack.onClick.AddListener(OnButtonSettingsBackClicked);

    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
    }

    private void OnDestroy()
    {
        buttonResume.onClick.RemoveAllListeners();
        buttonMainMenu.onClick.RemoveAllListeners();
        //buttonSettings.onClick.RemoveAllListeners();
        //buttonMainMenu.onClick.RemoveAllListeners();
      
    }

    private void OnButtonResumeClicked()
    {
        TogglePause();
    }

    private void OnButtonExitClicked()
    {
        AkSoundEngine.SetState("Game_State",  "Menu");
        SceneManager.LoadScene(mainMenuScene);
    }
    /*private void OnButtonSettingsClicked()
    {
        SetPanel(panelPause, false);
        SetPanel(panelSettings, true);
    }
    private void OnButtonMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    private void OnButtonSettingsBackClicked()
    {
        SetPanel(panelPause, true);
        SetPanel(panelSettings, false);
    }*/

    private void SetPanel(CanvasGroup panel, bool state)
    {
        panel.alpha = state ? 1 : 0;
        panel.interactable = state;
        panel.blocksRaycasts = state;
    }
    void TogglePause()
    {
        _isPause = !_isPause;
        Time.timeScale = _isPause ? 0 : 1;
        SetPanel(panelPause, _isPause);
        GameTogglePouse?.Invoke();
        EventManager.Instance.ChangeCursor();

        string gameState = _isPause ? "Pause" : "Lvl";
        AkSoundEngine.SetState("Game_State",  gameState);
        
        //Cursor.lockState = _isPause ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = _isPause;
    }

    private void RestartScene()
    {
        TogglePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
