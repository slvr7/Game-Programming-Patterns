using GPP;
using UnityEngine;

public class IntroTask : Task
{
    private readonly ParallelTasks _tasks = new ParallelTasks();
    private readonly GameContext _gameContext;
    private GameObject _text;

    public IntroTask(GameContext gameContext)
    {
        _gameContext = gameContext;
    }

    protected override void Init()
    {
        Debug.Log("Starting intro");
        var prefab = Resources.Load("prefabs/Intro Text");
        _text = Object.Instantiate(prefab) as GameObject;

        // Create a task to check for keyboard input
        _tasks.Add(new Do(() =>
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _gameContext.Difficulty = Difficulty.Easy;
                SetStatus(TaskStatus.Success);
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                _gameContext.Difficulty = Difficulty.Hard;
                SetStatus(TaskStatus.Success);
            }
        }));

        // Create a task to add a pulse task for the text if there
        // isn't one running
        _tasks.Add(new Do(() =>
        {
            if (!_tasks.HasTask<PulseTask>())
            {
                _tasks.Add(new PulseTask(_text, 0.2f, 1, 1));
            }
        }));
    }

    protected override void CleanUp()
    {
        Object.Destroy(_text);
        Debug.Log("Cleaning up intro");
    }

    internal override void Update()
    {
        _tasks.Update();
    }
}