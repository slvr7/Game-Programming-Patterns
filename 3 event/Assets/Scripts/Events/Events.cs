public class ScoreChanged : GameEvent
{
    public int NewScore { get; }
    
    public ScoreChanged(int newScore)
    {
        NewScore = newScore;
    }
}

public class PlayerDied : GameEvent {}

public class EnemyDied : GameEvent
{
    public int PointValue { get; }
    
    public EnemyDied(int value)
    {
        PointValue = value;
    }
}

public class GameStateChanged : GameEvent
{
    public GameState State { get; }

    public GameStateChanged(GameState state)
    {
        State = state;
    }
}
