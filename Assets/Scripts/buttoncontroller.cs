using UnityEngine;

public class buttoncontroller : MonoBehaviour
{
    [SerializeField] private GameObject buttondoor;
    [SerializeField] private Sprite DoorPushed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        buttondoor.GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().sprite = DoorPushed;
    }
}
