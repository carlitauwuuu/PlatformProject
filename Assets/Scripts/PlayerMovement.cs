using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 2f;
    [SerializeField] GameObject spritePlayer;
    [SerializeField] private float maxVel = 10f;
    [SerializeField] private float pDeceleration = 30.0f;
    private Rigidbody2D playerRigidbody2d;
    private Vector2 playerDirection;
    private ParticleSystem walkParticles;
    private float minVelocityPlayer = 0.01f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float airDampening = 0.8f;
    [SerializeField] private float airGravityMod = 1.2f;
    [SerializeField] private float normalGravity = 1.3f;

    [Header("Grounded")]
    [SerializeField] Transform groundCheckPos;
    [SerializeField] Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] LayerMask groundLayer;

    [Header("Sound")]
    [SerializeField] private AudioClip jumpSound;

 
    // SoundManager soundManager; 

    public bool isFacingRight = true;
    PlatformMovement platformMovement;
    Animator animator;

    //para mover fruta
    [Header("Fruta mueve")]
    [SerializeField] GrapplingGun grapplingGun;

    void Start()
    {
       playerRigidbody2d = GetComponent<Rigidbody2D>();
      // soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
      animator = GetComponent<Animator>();
      walkParticles = GetComponentInChildren<ParticleSystem>();

    }

    void FixedUpdate()
    {
        FlipPlayer();

        if (Mathf.Abs(playerDirection.x) > minVelocityPlayer)
        {
            playerRigidbody2d.AddForce(new Vector2(playerDirection.x, 0) * playerSpeed, ForceMode2D.Force);
            //playerRigidbody2d.linearVelocity = new Vector2(playerDirection.x * playerSpeed, playerRigidbody2d.linearVelocityY);
        }
        else
        {
            playerRigidbody2d.linearVelocityX = Mathf.MoveTowards(playerRigidbody2d.linearVelocityX, 0.0f, pDeceleration);
        }

        animator.SetFloat("xVel", Mathf.Abs(playerRigidbody2d.linearVelocityX));

        if (!IsGrounded())
        {
            playerRigidbody2d.AddForce(new Vector2(playerDirection.x, 0) * playerSpeed * airDampening, ForceMode2D.Force);
        }

        if(playerRigidbody2d.linearVelocityY < 0.0f)
        {
            playerRigidbody2d.gravityScale = airGravityMod;
        }
        else
        {
            playerRigidbody2d.gravityScale = normalGravity;
        }

        if (IsGrounded())
        {
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", true);
        }


            playerRigidbody2d.linearVelocityX = Mathf.Clamp(playerRigidbody2d.linearVelocityX, -maxVel, maxVel);
        if (grapplingGun.fruitMovement != null)
        {
            playerRigidbody2d.position += grapplingGun.fruitMovement.delta;
        }
        else if (platformMovement != null)
        {
            playerRigidbody2d.position += platformMovement.delta;
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
                //SoundManager.Instance.PlayerSound(jumpSound);
                
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


    private void FlipPlayer()
    {
        float localScaleX;
        localScaleX = spritePlayer.transform.localScale.x;
        if(isFacingRight && playerDirection.x < 0 || !isFacingRight && playerDirection.x > 0)
        {
            localScaleX = localScaleX * (-1);
            spritePlayer.transform.localScale = new Vector2 (localScaleX, spritePlayer.transform.localScale.y);
            isFacingRight = !isFacingRight; 
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            Gamemanager.instance.RestartGame();
        }
        if (collision.gameObject.CompareTag("Platform"))
        {
            gameObject.transform.parent = collision.gameObject.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            gameObject.transform.parent = null;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
            platformMovement = collision.gameObject.GetComponent<PlatformMovement>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
            platformMovement = null;
    }
}
