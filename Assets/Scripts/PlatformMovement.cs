using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float speed = 2f;
    private Vector2 direction = Vector2.right;
    private Rigidbody2D rb;
    private Vector2 previousPosition;
    public Vector2 delta { get; private set; }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        previousPosition = rb.position;
    }

    private void FixedUpdate()
    {
        // Move the platform
        Vector2 newPos = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // Store delta movement
        delta = newPos - previousPosition;

        previousPosition = newPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("RightLimit"))
            direction = Vector2.left;
        else if (other.CompareTag("LeftLimit"))
            direction = Vector2.right;


    }

}
