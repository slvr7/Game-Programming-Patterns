
using UnityEngine;

public enum Difficulty
{
    Easy,
    Hard
}

[CreateAssetMenu(fileName = "GameConfig", menuName = "Config")]
public class GameConfig : ScriptableObject
{
    public float Duration;
    public int NumBalls;
    public float BallMoveDuration;
    public float PulseDuration;
}