using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    private void Start()
    {
        Hide();
        EventManager.Instance.AddHandler<GameStateChanged>(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveHandler<GameStateChanged>(OnGameStateChanged);
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void OnGameStateChanged(GameStateChanged evt)
    {
        if (evt.State == GameState.Over)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
}