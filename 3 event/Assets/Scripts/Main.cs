using UnityEngine;

public enum GameState
{
    Playing,
    Over
}

public class Main : MonoBehaviour
{
    public int numStartingEnemies = 5;
    public GameObject turretPrefab;
    public GameObject enemyPrefab;

    private Turret _turret;

    private int _score;
    private int Score
    {
        get => _score;
        set 
        { 
            _score = value; 
            EventManager.Instance.Fire(new ScoreChanged(_score)); 
        }
    }

    private GameState _gameState = GameState.Over;
    private GameState State
    {
        get => _gameState;
        set
        {
            _gameState = value; EventManager.Instance.Fire(new GameStateChanged(_gameState));
        }
    }
    
    private void Start()
    {
        StartGame();        
        // When an enemy dies we need to replace it and update the score, so we'll register a 
        // handler for the EnemyDied event.
        EventManager.Instance.AddHandler<EnemyDied>(OnEnemyDied);
        EventManager.Instance.AddHandler<PlayerDied>(OnPlayerDied);
    }

    private void OnDestroy()
    {
        // Always unregister event handlers...
        EventManager.Instance.RemoveHandler<EnemyDied>(OnEnemyDied);
        EventManager.Instance.RemoveHandler<PlayerDied>(OnPlayerDied);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && State == GameState.Over)
        {
            StartGame();
        }
    }
    
    //////////////////////////////////////////////////
    // EVENT HANDLERS
    //////////////////////////////////////////////////
    private void OnPlayerDied(PlayerDied evt)
    {
        EndGame();
    }
    
    private void OnEnemyDied(EnemyDied evt)
    {
        Score += evt.PointValue;
        CreateEnemy();
    }

    //////////////////////////////////////////////////
    // GAME STATE MANAGEMENT
    //////////////////////////////////////////////////
    private void StartGame()
    {
        _turret = Instantiate(turretPrefab, Vector3.zero, Quaternion.identity).GetComponent<Turret>();
        
        // Create a number of enemies to start with
        for (var i = 0; i < numStartingEnemies; i++)
        {
            CreateEnemy();
        }

        Score = 0;
        State = GameState.Playing;
    }

    private void EndGame()
    {
        State = GameState.Over;
    }
    
    private void CreateEnemy()
    {
        // Create the enemy at a random point at the edge of the screen
        var side = Random.Range(0, 4);
        var viewportPos = new Vector2();
        
        switch (side)
        {
            case 0: // Top
                viewportPos.x = Random.value;
                viewportPos.y = 1;
                break;
            case 1: // Bottom
                viewportPos.x = Random.value;
                viewportPos.y = 0;
                break;
            case 2: // Left
                viewportPos.x = 0;
                viewportPos.y = Random.value;
                break;
            case 3: // Right
                viewportPos.x = 1;
                viewportPos.y = Random.value;
                break;
        }

        var pos = Camera.main.ViewportToWorldPoint(viewportPos);
        pos.z = 0;
        var enemy = Instantiate(enemyPrefab);
        enemy.transform.position = pos;
        
        // Point the enemy towards the player
        var angle = Vector3.SignedAngle(_turret.transform.position - pos, enemy.transform.right, Vector3.back);
        enemy.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
