using UnityEngine;

public class buttondispenser : MonoBehaviour
{
    [SerializeField] private GameObject buttondispen;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        buttondispen.GetComponent<CapsuleCollider2D>().enabled = true;
       // Instantiate.(false);
    }
}

