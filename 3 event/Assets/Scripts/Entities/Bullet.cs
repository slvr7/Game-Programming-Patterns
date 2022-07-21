using UnityEngine;

public class Bullet : MonoBehaviour
{
    public FloatVariable speed;

    private void Start()
    {
        EventManager.Instance.AddHandler<GameStateChanged>(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveHandler<GameStateChanged>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChanged evt)
    {
        if (evt.State == GameState.Over) Destroy(gameObject);
    }
    
    private void Update()
    {
        transform.position += transform.right * Time.deltaTime * speed.value;
        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);   
    }

}