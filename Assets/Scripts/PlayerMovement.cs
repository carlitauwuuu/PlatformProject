using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] GameObject spritePlayer;
    private Rigidbody2D playerRigidbody2d;
    private Vector2 playerDirection;
    private ParticleSystem walkParticles;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Grounded")]
    [SerializeField] Transform groundCheckPos;
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] LayerMask groundLayer;

    [Header("Sound")]
    [SerializeField] private AudioClip jumpSound;
    // SoundManager soundManager; 

    Animator animator;


    void Start()
    {
       playerRigidbody2d = GetComponent<Rigidbody2D>();
      // soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
      animator = GetComponent<Animator>();
      walkParticles = GetComponentInChildren<ParticleSystem>();

    }

    void FixedUpdate()
    {

        playerRigidbody2d.AddForce(new Vector2(playerDirection.x, 0) * playerSpeed, ForceMode2D.Force);
        //playerRigidbody2d.linearVelocity = new Vector2(playerDirection.x * playerSpeed, playerRigidbody2d.linearVelocityY);
        animator.SetFloat("xVel", Mathf.Abs(playerRigidbody2d.linearVelocityX));

        if(playerDirection.x < -0.1)
        {
            spritePlayer.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if(playerDirection.x > 0.1)
        {
            spritePlayer.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            spritePlayer.transform.localScale = transform.localScale;
        }

    }

    public void Move(InputAction.CallbackContext context)
    {
        playerDirection = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        
        if (IsGrounded())
        {
            if (context.performed)
            {
                playerRigidbody2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                
                //playerRigidbody2d.linearVelocity = new Vector2(playerRigidbody2d.linearVelocityX, jumpForce);
                SoundManager.Instance.PlayerSound(jumpSound);
                animator.SetBool("isJumping", true);
            }
        }     
    }
    public bool IsGrounded()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0 , groundLayer))
        {
            return true;
        }
        
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    public void EmitWalkParticles()
    {
        walkParticles.Emit(1);
    }
}
