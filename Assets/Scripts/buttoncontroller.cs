using UnityEngine;

public class buttoncontroller : MonoBehaviour
{
    [SerializeField] private GameObject door;
 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        door.GetComponent<CapsuleCollider2D>().enabled = true;
        gameObject.SetActive(false);
    }
}
