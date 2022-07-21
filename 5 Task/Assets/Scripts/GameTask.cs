using System.Collections.Generic;
using GPP;
using UnityEngine;

public class GameTask : Task
{
    private readonly ParallelTasks _tasks = new ParallelTasks();

    private List<GameObject> _balls;
    private Object _ballPrefab;
    private GameConfig _config;

    private readonly GameContext _gameContext;

    public GameTask(GameContext gameContext)
    {
        _gameContext = gameContext;
    }

    protected override void Init()
    {
        Debug.Log("Initializing Game Task");

        _ballPrefab = Resources.Load("prefabs/Ball");

        var configPath = $"config/{_gameContext.Difficulty}";

        _config = Resources.Load(configPath) as GameConfig;

        _balls = new List<GameObject>();
        _gameContext.Score = 0;

        // Add a task to destroy any clicked balls
        _tasks.Add(new Do(() =>
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                for (var i = _balls.Count-1; i >= 0; i--)
                {
                    var ball = _balls[i];
                    if (ball.GetComponent<Collider>().bounds.Contains(mousePos))
                    {
                        DestroyBall(ball);
                    }
                }
            }
        }));

        // Add a task to create more balls if there's less than the minimum
        _tasks.Add(new Do(() =>
        {
            if (_balls.Count < _config.NumBalls)
            {
                CreateBall();
            }
        }));

        // Add a task to end the game after time runs out
        var timeLeft = _config.Duration;
        _tasks.Add(new Do(() =>
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                SetStatus(TaskStatus.Success);
            }
        }));
    }

    protected override void CleanUp()
    {
        foreach (var ball in _balls)
        {
            Object.Destroy(ball);
        }
        _balls.Clear();
        _tasks.Clear();
        Debug.Log("Game Task cleaning up.");
    }

    private void CreateBall()
    {
        var ball = Object.Instantiate(_ballPrefab) as GameObject;
        ball.GetComponent<Ball>().Config = _config;
        _balls.Add(ball);
    }

    private void DestroyBall(GameObject ball)
    {
        _balls.Remove(ball);
        Object.Destroy(ball);
        _gameContext.Score += 1;
    }

    internal override void Update()
    {
        _tasks.Update();
    }
}