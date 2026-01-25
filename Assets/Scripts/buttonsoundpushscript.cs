using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PushableBoxSound : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioClip pushLoopSound;
    [SerializeField] private float minMoveSpeed = 0.1f;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = pushLoopSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed > minMoveSpeed && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (speed <= minMoveSpeed && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}