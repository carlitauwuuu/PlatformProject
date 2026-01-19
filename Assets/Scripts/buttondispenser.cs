using UnityEngine;

public class buttondispenser : MonoBehaviour
{
    public GameObject Box;
    public Transform spawn;
    [SerializeField] private Sprite BoxPushed;

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Spawnitem();
            GetComponent<SpriteRenderer> ().sprite = BoxPushed;
        }
        
    }
    void Spawnitem()
    {
        Instantiate(Box, spawn.position, Quaternion.identity);

    }
}


