using UnityEngine;

public class Enemy : MonoBehaviour
{   
    public FloatVariable speed;

    public IntRangeVariable pointRange;
    
    public int PointValue { get; private set; }

    private void Start()
    {
        PointValue = (int) ((pointRange.max - pointRange.min) * Random.value) + pointRange.min;
        EventManager.Instance.AddHandler<GameStateChanged>(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        // Always unregister for events when you're done...
        EventManager.Instance.RemoveHandler<GameStateChanged>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChanged evt)
    {
        if (evt.State == GameState.Over) Destroy(gameObject);
    }

    private void Update()
    {
        transform.position += transform.right * Time.deltaTime * speed.value;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // If the enemy was hit by a bullet let everyone know so we can do things like update points
        if (other.gameObject.GetComponent<Bullet>() != null)
        {
            EventManager.Instance.Fire(new EnemyDied(PointValue));
        }
        
        // Regardless of what was hit destroy the enemy
        Destroy(gameObject);   
    }
}