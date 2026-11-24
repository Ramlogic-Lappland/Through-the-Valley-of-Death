using UnityEngine;
using AK.Wwise;

public class DoorAudio : MonoBehaviour
{

    //Evento y variable de Wwise
    public AK.Wwise.Event wCreakLoop;
    public AK.Wwise.Event wDoorCollision;
    public AK.Wwise.RTPC wDoorSpeed;

    //Velocidad de la puerta
    private HingeJoint2D hj;
    private float speed;
    //private Collider2D collider;

    void Start()
    {
        hj = GetComponent<HingeJoint2D>();
        wCreakLoop.Post(gameObject);
    }

    void FixedUpdate()
    {
        speed = Mathf.Abs(hj.jointSpeed);
        wDoorSpeed.SetValue(gameObject, speed);
        //Debug.Log(speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        wDoorCollision.Post(gameObject);
    }

    private void OnDestroy()
    {
        wCreakLoop.Stop(gameObject);
    }
}
