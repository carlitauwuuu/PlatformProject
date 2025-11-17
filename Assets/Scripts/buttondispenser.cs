using UnityEngine;

public class buttondispenser : MonoBehaviour
{
    public GameObject Box;
    public Transform spawn;
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Spawnitem();
            gameObject.SetActive(false);
        }
        
    }
    void Spawnitem()
    {
        Instantiate(Box, spawn.position, Quaternion.identity);
    }
}


