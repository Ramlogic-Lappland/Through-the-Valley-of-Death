using UnityEngine;

public class PlayerAudioSteps : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event stepsEvent;
    
    public void PlayStepAudio()
    {
        AkUnitySoundEngine.PostEvent(stepsEvent.Id, gameObject);
    }
}
