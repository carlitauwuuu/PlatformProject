using UnityEngine;

public class FruitMovement : MonoBehaviour
{
  

        public float speed = 3f;   // Velocidad del movimiento
        private int direction = 1; // 1 = derecha, -1 = izquierda
        private SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
        {
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("RightLimit"))
            {
            Debug.Log("hay coliisom");
                direction = -1;
            }
       
            else if (collision.CompareTag("LeftLimit"))
            {
                direction = 1;
            }
        }
    
}
