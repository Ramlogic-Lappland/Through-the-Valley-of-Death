using UnityEngine;
using AK.Wwise;

public class CombatManager : MonoSingleton<CombatManager>
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private float maxTimer;

    [SerializeField] private AK.Wwise.State combatOnState;
    [SerializeField] private AK.Wwise.State combatOffState;

    private float combatTimer = 0f;
    private bool inCombat = false;

    private void Awake()
    {
        AkSoundEngine.SetState("Combat", "Off");
    }
    
    
    public void RegisterCombat()
    {
        if (!inCombat)
        {
            inCombat = true;
            AkSoundEngine.SetState("Combat", "On");
        }

        combatTimer = maxTimer;
    }
    void Update()
    {
        if (!inCombat) return; //todo lo que está abajo es para caso combate = true

        // bajar timer
        combatTimer -= Time.deltaTime;

        // Si el timer se acaba se actualiza el estado y combate = false
        if (combatTimer <= 0f)
        {
            inCombat = false;
            AkSoundEngine.SetState("Combat", "Off");
        }
    }
}
