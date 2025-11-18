using UnityEngine;

public class buttoncontroller : MonoBehaviour
{
    [SerializeField] private GameObject buttondoor;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        buttondoor.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(false);
    }
}
