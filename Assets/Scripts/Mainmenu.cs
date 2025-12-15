using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    public void Playgame()
    {

        SceneManager.LoadScene("Level1");
    }

    public void ExitGame()
    {
        Debug.Log("Ha salido del juego");
        Application.Quit();
    }
   
}
