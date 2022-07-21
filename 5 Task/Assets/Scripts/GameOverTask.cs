using GPP;
using UnityEngine;

public class GameOverTask : Task
{
    private readonly ParallelTasks _tasks;

    private readonly GameContext _gameContext;
    private GameObject _text;

    public GameOverTask(GameContext gameContext)
    {
        _gameContext = gameContext;
    }

    protected override void Init()
    {
        Debug.Log("Starting Game Over");
        var prefab = Resources.Load("prefabs/Game Over Text");
        _text = Object.Instantiate(prefab) as GameObject;
        _text.GetComponent<TextMesh>().text =
            "Game Over" +
            "\n" +
            $"Score:{_gameContext.Score}" +
            "\n" +
            "Press R to reset game";
    }

    protected override void CleanUp()
    {
        Object.Destroy(_text);
    }

    internal override void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetStatus(TaskStatus.Success);
        }
    }
}