
// We need some place to store any state that outlives
// a task or is used by multiple tasks like the
// selected difficulty and the score
public class GameContext
{
    public int Score;
    public Difficulty Difficulty;
}