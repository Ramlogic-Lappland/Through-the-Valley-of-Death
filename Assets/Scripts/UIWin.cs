using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWin : MonoBehaviour
{
    //[SerializeField] private Button  buttonRestart;
    [SerializeField] private Button  buttonMainMenu;
    [SerializeField] private SceneField sceneMainMenu;
    //[SerializeField] private SceneField  sceneLvl1;

    private void Awake()
    {
        //buttonRestart.onClick.AddListener(OnButtonRestart);
        buttonMainMenu.onClick.AddListener(OnButtonMainMenu);
        AkSoundEngine.SetState("Game_State",  "Win");
    }

    /*private void OnButtonRestart()
    {
        SceneManager.LoadScene(sceneLvl1);
    }*/

    private void OnButtonMainMenu()
    {
        SceneManager.LoadScene(sceneMainMenu);
    }
}
