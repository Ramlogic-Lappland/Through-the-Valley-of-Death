using UnityEngine;
using AK.Wwise;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public float maxTimer;

    public AK.Wwise.State combatOnState;
    public AK.Wwise.State combatOffState;

    private float combatTimer = 0f;
    private bool inCombat = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void RegisterCombat()
    {
        if (!inCombat)
        {
            inCombat = true;
            combatOnState.SetValue();
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
            combatOffState.SetValue();
        }
    }
}
