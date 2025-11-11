using UnityEngine;

public class buttoncontroller : MonoBehaviour
{
    [SerializeField] private GameObject dispenser;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        dispenser.GetComponent<CapsuleCollider2D>().enabled = true;
        gameObject.SetActive(false);
    }
}
