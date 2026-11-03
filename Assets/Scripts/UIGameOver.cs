using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{
   [SerializeField] private Button  buttonRestart;
   [SerializeField] private Button  buttonMainMenu;
   [SerializeField] private SceneField sceneMainMenu;
   [SerializeField] private SceneField  sceneLvl1;
   
   [SerializeField] private CanvasGroup gameOverPanel;

   private void Awake()
   {
      buttonRestart.onClick.AddListener(OnButtonRestart);
      buttonMainMenu.onClick.AddListener(OnButtonMainMenu);
      
   }

   private void OnButtonRestart()
   {
      SceneManager.LoadScene(sceneLvl1);
   }

   private void OnButtonMainMenu()
   {
      SceneManager.LoadScene(sceneMainMenu);
   }


   private void OnPlayerDeath()
   {
      SetPanel(gameOverPanel, true);
   }
   
   private void SetPanel(CanvasGroup panel, bool state)
   {
      panel.alpha = state ? 1 : 0;
      panel.interactable = state;
      panel.blocksRaycasts = state;
   }

   public void ActivePanel()
   {
      SetPanel(gameOverPanel, true);
   }
}
