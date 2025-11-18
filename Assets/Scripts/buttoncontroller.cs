using UnityEngine;

public class buttoncontroller : MonoBehaviour
{
    [SerializeField] private GameObject buttondoor;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        buttondoor.GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(false);
    }
}
