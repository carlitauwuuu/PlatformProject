using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Sprite doorClosed;
    [SerializeField] private Sprite doorOpen;

    private SpriteRenderer spriteRenderer;
    private bool isOpen = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = doorClosed;
        GetComponent<Collider2D>().enabled = false; // empieza cerrada
    }

    public void OpenDoor()
    {
        isOpen = true;
        spriteRenderer.sprite = doorOpen;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen && collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}