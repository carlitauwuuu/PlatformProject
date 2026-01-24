using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

//tom hanks 2 Ahora es personal
public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts:")]
    public GrappleRope grappleRope;
    [Header("Layer Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private LayerMask grappableLayerNumber;

    [Header("Main Camera")]
    public Camera m_camera;

    [Header("Transform Refrences:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 80)][SerializeField] private float rotationSpeed = 4;
    [SerializeField] float maxAngle = 90f;
    private bool isGrappling = false;
    private Vector2 vertical = Vector2.down;
    private float currAngle;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = true;
    [SerializeField] private float maxDistance = 4;

    [Header("Launching")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType Launch_Type = LaunchType.Transform_Launch;
    [Range(0, 5)][SerializeField] private float launchSpeed = 5;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoCongifureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequency = 3;


    [Header("PlayerShooting Change color ")]

    private Camera mainCamera;
    private Vector3 cursorPosition;

    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject maxDistancePointer; // New max-distance pointer
    [Header("Player Forms (child objects)")]
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private GameObject head;
    [SerializeField] private Sprite[] formSprite = new Sprite[] { };
    [SerializeField] private int StartForm = 0;
    [Header("Colors")]
    [SerializeField] private Color orangeForm;
    [SerializeField] private Color greenForm;
    [SerializeField] private Color pinkForm;
    [SerializeField] private Color yellowForm;
    private Color currentColor;
    [HideInInspector] public GameObject grabbedFly;
    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch,
    }

    [Header("Component Refrences:")]
    public SpringJoint2D m_springJoint2D;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 DistanceVector;
    Vector2 Mouse_FirePoint_DistanceVector;

    public Rigidbody2D playerRB;

    [HideInInspector] public Transform grappleTarget;
    private Vector2 grappleLocalOffset;
    private float desiredDistance;

    public FruitMovement fruitMovement { get; private set; }
    private PlayerMovement playerMovement;

    [Header("Sound")]
    [SerializeField] private AudioClip tongueShootSound;
    [SerializeField] private AudioClip grappleHitSound;   
    [SerializeField] private AudioClip eatFlySound;       

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        playerMovement = GetComponentInParent<PlayerMovement>();

        if (pointer == null)
            pointer = GameObject.Find("Pointer");

        if (maxDistancePointer == null)
            maxDistancePointer = GameObject.Find("MaxDistancePointer");

        mainCamera = Camera.main;

        //orange default
        ActivateForm(StartForm);
    }

    private void FixedUpdate()
    {
        if (grappleTarget != null && grabbedFly == null)
        {
            grapplePoint = (Vector2)grappleTarget.position + grappleLocalOffset;

            if (m_springJoint2D.enabled)
            {
                m_springJoint2D.connectedAnchor = grapplePoint;
                m_springJoint2D.distance = desiredDistance;
            }
        }

        if (!m_springJoint2D.enabled) return;

        Vector2 anchor = m_springJoint2D.connectedAnchor;
        Vector2 toPlayer = playerRB.position - anchor;

        float distance = toPlayer.magnitude;
        if (distance < 0.001f) return;

        Vector2 dir = toPlayer / distance;
        Vector2 vertical = Vector2.down;

        float angle = Vector2.SignedAngle(vertical, dir);

        if (Mathf.Abs(angle) > maxAngle)
        {
            float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);

            Vector2 clampedDir =
                Quaternion.Euler(0, 0, clampedAngle) * vertical;

            playerRB.MovePosition(anchor + clampedDir * distance);

            Vector2 tangent = Vector2.Perpendicular(clampedDir);

            if (tangent.sqrMagnitude > 0.0001f)
            {
                playerRB.linearVelocity =
                    Vector2.Dot(playerRB.linearVelocity, tangent)
                    / tangent.sqrMagnitude
                    * tangent;
            }
        }
    }

    private void Update()
    {
        Mouse_FirePoint_DistanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }
            else
            {
                RotateGun(m_camera.ScreenToWorldPoint(Input.mousePosition), false);
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (Launch_Type == LaunchType.Transform_Launch)
                {
                    gunHolder.position = Vector3.Lerp(gunHolder.position, grapplePoint, Time.deltaTime * launchSpeed);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            grappleRope.enabled = false;
            grabbedFly = null;
            m_springJoint2D.enabled = false;
            isGrappling = false;
            fruitMovement = null;
            grappleTarget = null;
        }
        else
        {
            RotateGun(m_camera.ScreenToWorldPoint(Input.mousePosition), true);
        }

        UpdateMaxDistancePointer();

        if (m_springJoint2D.enabled)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                desiredDistance -= scroll * 2f;
                desiredDistance = Mathf.Clamp(desiredDistance, 1f, maxDistance);

                m_springJoint2D.distance = desiredDistance;
            }
        }

        if (grabbedFly != null)
        {
            Vector3 targetPos = gunPivot.position;
            float speed = 10f;

            grabbedFly.transform.position = Vector3.MoveTowards(
                grabbedFly.transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(grabbedFly.transform.position, targetPos) < 0.3f)
            {
                int layer = grabbedFly.layer;

                if (layer == LayerMask.NameToLayer("Orange"))
                    ActivateForm(0);
                else if (layer == LayerMask.NameToLayer("Green"))
                    ActivateForm(1);
                else if (layer == LayerMask.NameToLayer("Pink"))
                    ActivateForm(2);
                else if (layer == LayerMask.NameToLayer("Yellow"))
                    ActivateForm(3);

                if (eatFlySound != null)
                    SoundManager.Instance.PlaySFX(eatFlySound);

                Destroy(grabbedFly);
                grabbedFly = null;
                grappleTarget = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            cursorPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            cursorPosition.z = transform.position.z;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                pointer.transform.position = hit.point;

                float distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                if (distance > maxDistance)
                {
                    Debug.Log("muy lejos.");
                    return;
                }

                /*if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Orange") && hit.collider.CompareTag("Fly"))
                {
                    ActivateForm(0);
                    StartCoroutine(TemporarilyDisable(hit.collider.gameObject));
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Green") && hit.collider.CompareTag("Fly"))
                {
                    ActivateForm(1);
                    StartCoroutine(TemporarilyDisable(hit.collider.gameObject));
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Pink") && hit.collider.CompareTag("Fly"))
                {
                    ActivateForm(2);
                    StartCoroutine(TemporarilyDisable(hit.collider.gameObject));
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Yellow") && hit.collider.CompareTag("Fly"))
                {
                    ActivateForm(3);
                    StartCoroutine(TemporarilyDisable(hit.collider.gameObject));
                }*/
            }
            else
            {
                pointer.transform.position = cursorPosition;
            }
        }
    }

    private void UpdateMaxDistancePointer()
    {
        Vector3 mousePosition = m_camera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 playerPosition = transform.position;
        float distanceToMouse = Vector3.Distance(playerPosition, mousePosition);

        if (distanceToMouse > maxDistance)
        {
            Vector3 direction = (mousePosition - playerPosition).normalized;
            maxDistancePointer.transform.position = playerPosition + direction * maxDistance;
        }
        else
        {
            maxDistancePointer.transform.position = mousePosition;
        }

        maxDistancePointer.GetComponent<SpriteRenderer>().color = (distanceToMouse > maxDistance) ? Color.red : Color.white;
    }

    private void ActivateForm(int formToActivate)
    {
        GameObject playerObject = transform.root.gameObject;

        if (formToActivate == 0)
        {
            SetLayer(playerObject, LayerMask.NameToLayer("Orange"));
            playerSprite.GetComponent<SpriteRenderer>().color = orangeForm;
            head.GetComponent<SpriteRenderer>().color = orangeForm;
        }
        else if (formToActivate == 1)
        {
            SetLayer(playerObject, LayerMask.NameToLayer("Green"));
            playerSprite.GetComponent<SpriteRenderer>().color = greenForm;
            head.GetComponent<SpriteRenderer>().color = greenForm;
        }
        else if (formToActivate == 2)
        {
            SetLayer(playerObject, LayerMask.NameToLayer("Pink"));
            playerSprite.GetComponent<SpriteRenderer>().color = pinkForm;
            head.GetComponent<SpriteRenderer>().color = pinkForm;
        }
        else if (formToActivate == 3)
        {
            SetLayer(playerObject, LayerMask.NameToLayer("Yellow"));
            playerSprite.GetComponent<SpriteRenderer>().color = yellowForm;
            head.GetComponent<SpriteRenderer>().color = yellowForm;
        }

        Debug.Log("Player layer set to: " + LayerMask.LayerToName(playerObject.layer));
    }

    //cambiar layer a todos los hijos el playernewsys
    private void SetLayer(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayer(child.gameObject, newLayer);
        }
    }

    private void SetLayerTodo(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerTodo(child.gameObject, newLayer);
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        if (gunPivot == null || playerMovement == null) return;

        Vector2 dir = lookPoint - gunPivot.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float forward = playerMovement.isFacingRight ? 0f : 180f;
        float relativeAngle = angle - forward;
        relativeAngle = Mathf.Repeat(relativeAngle + 180f, 360f) - 180f;
        relativeAngle = Mathf.Clamp(relativeAngle, -maxAngle, maxAngle);
        float finalAngle = forward + relativeAngle;

        Quaternion targetRot = Quaternion.Euler(0f, 0f, finalAngle);

        if (rotateOverTime && allowRotationOverTime)
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, targetRot, Time.deltaTime * rotationSpeed);
        else
            gunPivot.rotation = targetRot;

        Vector3 gpScale = gunPivot.localScale;
        gpScale.y = playerMovement.isFacingRight ? 1f : -1f;
        gunPivot.localScale = gpScale;
    }


    void SetGrapplePoint()
    {
        Vector2 direction = Mouse_FirePoint_DistanceVector.normalized;
        float distance = hasMaxDistance ? Mathf.Min(Mouse_FirePoint_DistanceVector.magnitude, maxDistance) : Mouse_FirePoint_DistanceVector.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, distance);

        fruitMovement = null;
        grappleTarget = null;
        grabbedFly = null;


        if (hit.collider != null)
        {
            grapplePoint = hit.point;

            if (hit.collider.CompareTag("FruitAttacheable") && hit.collider.gameObject.layer == gameObject.layer)
            {
                fruitMovement = hit.collider.GetComponent<FruitMovement>();
                grappleTarget = hit.collider.transform;
                grappleLocalOffset = hit.point - (Vector2)grappleTarget.position;

                if (grappleHitSound != null)
                    SoundManager.Instance.PlaySFX(grappleHitSound);
            }
            else if (hit.collider.CompareTag("Fly"))
            {
                grabbedFly = hit.collider.gameObject;
                grappleTarget = null;
                m_springJoint2D.enabled = false;

                grapplePoint = hit.point;

                if (grappleHitSound != null)
                    SoundManager.Instance.PlaySFX(grappleHitSound);
            }
            else if (hit.collider.gameObject.layer != gameObject.layer || hit.collider.CompareTag("Player"))
            {
                return;
            }
        }
        else
        {

            grapplePoint = (Vector2)firePoint.position + direction * distance;

            if (tongueShootSound != null)
                SoundManager.Instance.PlaySFX(tongueShootSound);
        }


        isGrappling = true;
        DistanceVector = grapplePoint - (Vector2)gunPivot.position;
        grappleRope.enabled = true;
    }


    public void Grapple()
    {
        if (grappleTarget == null)
            return;

        float actualDistance = Vector2.Distance(playerRB.position, grapplePoint);

        if (!launchToPoint && !autoCongifureDistance)
        {
            m_springJoint2D.distance = actualDistance;
            m_springJoint2D.frequency = targetFrequency;
        }

        if (!launchToPoint)
        {
            if (autoCongifureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
            desiredDistance = actualDistance;
        }
    }

    private void OnDrawGizmos()
    {
        if (hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }
}