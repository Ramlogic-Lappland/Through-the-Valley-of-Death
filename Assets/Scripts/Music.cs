using UnityEngine;

public class Music : MonoBehaviour
{
    private static Music _instancia;
    
    void Awake()
    {
        
        if (_instancia == null)
        {
            _instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
