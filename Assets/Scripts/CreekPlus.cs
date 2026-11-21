using UnityEngine;
using AK.Wwise;
using Unity.VisualScripting;

public class CreekPlus : MonoBehaviour
{
    //Evento y variable de Wwise
    public AK.Wwise.Event wCreakLoop;
    public AK.Wwise.RTPC DoorSpeed;

    //Velocidad de la puerta
    private Rigidbody2D rb;
    private float speed;
    
    void Awake()
    {
        wCreakLoop.Post(gameObject);
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        speed = rb.linearVelocity.magnitude;
        DoorSpeed.SetValue(gameObject, speed);
        Debug.Log(speed);
    }

    void OnDisable()
    {
        wCreakLoop.Stop(gameObject);
    }
}