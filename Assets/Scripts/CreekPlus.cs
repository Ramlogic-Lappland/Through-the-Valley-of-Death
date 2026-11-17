using UnityEngine;
using AK.Wwise;

public class CreekPlus : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El evento de Wwise que suena en bucle infinito (con el RTPC ya configurado).")]
    public AK.Wwise.Event creakLoopEvent;

    [Tooltip("El RTPC 'DoorSpeed' expuesto en Wwise.")]
    public AK.Wwise.RTPC doorSpeedRtpc;

    [Tooltip("Transform del sensor en la punta de la puerta (el que gira). Si está vacío, usa este GameObject.")]
    public Transform tipSensor;

    [Header("Mapeo de velocidad → RTPC")]
    [Tooltip("Velocidad (m/s) que corresponde al valor máximo del RTPC.")]
    public float maxSpeed = 2f;

    [Tooltip("Valor máximo del RTPC en Wwise (debe coincidir con el rango del Game Parameter).")]
    public float rtpcMaxValue = 100f;

    [Tooltip("Velocidad por debajo de la cual forzamos RTPC = 0 (evita ruido residual).")]
    public float deadZoneSpeed = 0.02f;

    [Header("Suavizado")]
    [Tooltip("Tiempo aproximado de suavizado del RTPC. 0 = sin suavizado.")]
    public float smoothingTime = 0.08f;

    [Header("Debug")]
    public bool logRtpc = false;

    // --- Estado interno ---
    private Vector3 _lastTipPos;
    private float _smoothedRtpc;
    private float _rtpcVelocity; // para SmoothDamp
    private uint _playingId = 0;

    void Start()
    {
        // Sensor por defecto: este GameObject
        if (tipSensor == null) tipSensor = transform;

        // Lanzar el loop UNA sola vez. En Wwise el sonido es Loop Infinite.
        // Se queda sonando siempre; el RTPC decide si se oye o no.
        _playingId = creakLoopEvent.Post(gameObject);

        // Posición inicial del sensor
        _lastTipPos = tipSensor.position;
        _smoothedRtpc = 0f;
    }

    void Update()
    {
        // 1) Velocidad absoluta del sensor (m/s)
        Vector3 currentTip = tipSensor.position;
        float dt = Mathf.Max(Time.deltaTime, 1e-5f);
        float speed = (currentTip - _lastTipPos).magnitude / dt;
        _lastTipPos = currentTip;

        // 2) Dead zone: si está prácticamente quieta, RTPC = 0
        float targetRtpc;
        if (speed <= deadZoneSpeed)
        {
            targetRtpc = 0f;
        }
        else
        {
            // 3) Normalizar y mapear al rango del RTPC
            float normalized = Mathf.Clamp01(speed / maxSpeed);
            targetRtpc = normalized * rtpcMaxValue;
        }

        // 4) Suavizado para evitar saltos bruscos de pitch
        if (smoothingTime > 0f)
        {
            _smoothedRtpc = Mathf.SmoothDamp(
                _smoothedRtpc, targetRtpc, ref _rtpcVelocity, smoothingTime);
        }
        else
        {
            _smoothedRtpc = targetRtpc;
        }

        // 5) Enviar al RTPC
        // SetValue = local a este GameObject (recomendado si hay varias puertas).
        doorSpeedRtpc.SetValue(gameObject, _smoothedRtpc);

        if (logRtpc)
            Debug.Log($"[DoorCreak] speed={speed:F3} m/s → RTPC={_smoothedRtpc:F1}");
    }

    void OnDisable()
    {
        // Opcional: al desactivar la puerta, para el loop.
        if (_playingId != 0)
        {
            AkUnitySoundEngine.StopPlayingID(_playingId);
            _playingId = 0;
        }
    }
}