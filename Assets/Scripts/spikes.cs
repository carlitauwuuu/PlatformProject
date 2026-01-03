using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : Gamemanager
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Death"))
        {
            RestartGame();
        }
    }
}
