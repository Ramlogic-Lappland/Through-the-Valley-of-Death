using UnityEngine;

public class Music : MonoSingleton<Music>
{

    public void SetHP(int HP)
    {
        AkSoundEngine.SetRTPCValue("HP", HP, gameObject);
        Debug.Log("HP: " + HP);
    }
}
