using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private DoorController door;
    [SerializeField] private Sprite buttonPushed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            door.OpenDoor();
            GetComponent<SpriteRenderer>().sprite = buttonPushed;
        }
    }
}