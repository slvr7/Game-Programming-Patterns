using GPP;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private readonly SerialTasks _tasks = new SerialTasks();
    public GameConfig Config;
    
    private void Start()
    {
        transform.position = GetRandomPoint();
    }

    private void Update()
    {
        // If there's no more tasks queue up a new move sequence
        if (!_tasks.HasTasks) MoveToRandomPosition();
        _tasks.Update();
    }

    private void MoveToRandomPosition()
    {
        // Pulse first...
        const float maxScale = 0.5f;
        const int times = 3;
        _tasks.Add(new PulseTask(gameObject, maxScale, Config.PulseDuration, times));

        // Then move to a random spot
        var timeToMove = Config.BallMoveDuration;
        var randomPoint = GetRandomPoint();
        _tasks.Add(new MoveTask(gameObject, randomPoint, timeToMove));
    }

    private Vector3 GetRandomPoint()
    {
        var p = Camera.main.ViewportToWorldPoint(new Vector3(Random.value, Random.value, 0));
        p.z = 0;
        return p;
    }

}