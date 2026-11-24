using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWin : MonoBehaviour
{
    [SerializeField] private Button  buttonCredits;
    [SerializeField] private Button  buttonMainMenu;
    [SerializeField] private Button  buttonBack;
    [SerializeField] private SceneField sceneMainMenu;
    [SerializeField] private CanvasGroup creditsPanel;
    [SerializeField] private CanvasGroup winPanel;
    

    private void Awake()
    {
        buttonCredits.onClick.AddListener(OnButtonCredits);
        buttonMainMenu.onClick.AddListener(OnButtonMainMenu);
        buttonBack.onClick.AddListener(OnButtonBack);
        AkSoundEngine.SetState("Game_State",  "Win");
    }

    private void OnDisable()
    {
        buttonCredits.onClick.RemoveListener(OnButtonCredits);
        buttonMainMenu.onClick.RemoveListener(OnButtonMainMenu);
        buttonBack.onClick.RemoveListener(OnButtonBack);
    }

    private void OnButtonCredits()
    {
        SetPanel(winPanel, false);
        SetPanel(creditsPanel, true);
    }

    private void OnButtonMainMenu()
    {
        SceneManager.LoadScene(sceneMainMenu);
    }

    private void OnButtonBack()
    {
        SetPanel(winPanel, true);
        SetPanel(creditsPanel, false);
    }
    
    private void SetPanel(CanvasGroup panel, bool state)
    {
        panel.alpha = state ? 1 : 0;
        panel.interactable = state;
        panel.blocksRaycasts = state;
    }
}
