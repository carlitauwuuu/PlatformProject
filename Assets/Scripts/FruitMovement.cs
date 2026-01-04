using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    private Vector2 direction = Vector2.right;
    private Rigidbody2D rb;
    private Vector2 previousPosition;

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
        Vector2 delta = newPos - previousPosition;

        previousPosition = newPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlatformLimits"))
            return;

        if (other.CompareTag("RightLimit"))
            direction = Vector2.left;
        else if (other.CompareTag("LeftLimit"))
            direction = Vector2.right;
    }

    // Player carry logic
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.attachedRigidbody;
            if (playerRb != null)
            {
                // Add horizontal delta to player without touching vertical velocity
                playerRb.position += new Vector2((rb.position - previousPosition).x, 0f);
            }
        }
    }
}
