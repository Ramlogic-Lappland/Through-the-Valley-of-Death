using UnityEngine;
using AK.Wwise; // Necesario para usar AK.Wwise.Event

public class DoorCollisionSound : MonoBehaviour
{
    [Header("Eventos de Wwise")]
    [Tooltip("Evento que suena cuando el jugador choca con la puerta")]
    public AK.Wwise.Event playerHitEvent;

    [Tooltip("Evento que suena cuando un enemigo choca con la puerta")]
    public AK.Wwise.Event enemyHitEvent;

    // Se llama automáticamente cuando otro collider2D choca con el de la puerta
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Comprobamos la tag del objeto con el que hemos chocado
        if (collision.gameObject.CompareTag("Player"))
        {
            // Reproduce el evento para el jugador
            playerHitEvent.Post(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Reproduce el evento para el enemigo
            enemyHitEvent.Post(gameObject);
        }
    }
}
