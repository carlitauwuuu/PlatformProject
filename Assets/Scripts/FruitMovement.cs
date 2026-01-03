using UnityEngine;

public class FruitMovement : MonoBehaviour
{
    public float speed = 3f;
    private int direction = 1;
    private SpriteRenderer sr;

    // >>> CHANGE
    private Rigidbody2D rb;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // >>> CHANGE
        rb = GetComponent<Rigidbody2D>();
    }

    // >>> CHANGE: move in FixedUpdate, NOT Update
    void FixedUpdate()
    {
        rb.MovePosition(
            rb.position + Vector2.right * direction * speed * Time.fixedDeltaTime
        );
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
