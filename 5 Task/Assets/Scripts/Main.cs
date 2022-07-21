using GPP;
using UnityEngine;

public class Main : MonoBehaviour
{
    // We're going to represent the game as a sequence of
    // tasks so we'll use a serial task to ensure each
    // phase of the game happens in sequence...
    private readonly SerialTasks _game = new SerialTasks();

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        var context = new GameContext();

        // First we show the difficulty selection "screen"...
        _game.Add(new IntroTask(context));

        // Then we run the the game...
        _game.Add(new GameTask(context));

        // Then we show the final score...
        _game.Add(new GameOverTask(context));
    }

    private void Update()
    {
        if (_game.IsFinished) StartGame();
        _game.Update();
    }
}
