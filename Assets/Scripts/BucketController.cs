using UnityEngine;

public class BucketController : MonoBehaviour
{
    BoxCollider2D triggerCollider;
    Animator bucketAnimator;
    void Start()
    {
        triggerCollider = GetComponent<BoxCollider2D>();
        bucketAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        { 
            triggerCollider.enabled = false;
            bucketAnimator.SetBool("IsFalling", true);
        }
    }
}
