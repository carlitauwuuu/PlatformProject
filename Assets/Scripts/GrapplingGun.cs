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
    [SerializeField] private Sprite[] formSprite = new Sprite[] { };
    [SerializeField] private int StartForm = 0;
    [Header("Colors")]
    [SerializeField] private Color orangeForm;
    [SerializeField] private Color greenForm;
    [SerializeField] private Color pinkForm;
    [SerializeField] private Color yellowForm;
    private Color currentColor;

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

    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;

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
        // Safety check: only act when the grappling hook is active
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

            // Vector matemático, NO rotación del player
            Vector2 clampedDir =
                Quaternion.Euler(0, 0, clampedAngle) * vertical;

            // Recolocación limpia en el arco permitido
            playerRB.MovePosition(anchor + clampedDir * distance);

            // Limpieza de velocidad solo fuera del arco
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
            m_springJoint2D.enabled = false;
            isGrappling = false;
        }
        else
        {
            RotateGun(m_camera.ScreenToWorldPoint(Input.mousePosition), true);
        }

        // Handle max-distance pointer logic
        UpdateMaxDistancePointer();

        if (m_springJoint2D.enabled)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                m_springJoint2D.distance -= scroll * 2f;
                m_springJoint2D.distance = Mathf.Clamp(m_springJoint2D.distance, 1f, maxDistance);
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
                if (distance > 3f)
                {
                    Debug.Log("muy lejos.");
                    return;
                }

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Orange") && hit.collider.CompareTag("Fly"))
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
                }
            }
            else
            {
                pointer.transform.position = cursorPosition;
            }
        }
    }

    // Method to update the max distance pointer position
    private void UpdateMaxDistancePointer()
    {
        Vector3 mousePosition = m_camera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure it's 2D

        // Get the distance from the player to the mouse
        Vector3 playerPosition = transform.position;
        float distanceToMouse = Vector3.Distance(playerPosition, mousePosition);

        // Clamp the max-distance pointer based on the maxDistance
        if (distanceToMouse > maxDistance)
        {
            Vector3 direction = (mousePosition - playerPosition).normalized;
            maxDistancePointer.transform.position = playerPosition + direction * maxDistance;
        }
        else
        {
            maxDistancePointer.transform.position = mousePosition;
        }

        // Optional: Change the color of the max-distance pointer if it's clamped
        maxDistancePointer.GetComponent<SpriteRenderer>().color = (distanceToMouse > maxDistance) ? Color.red : Color.white;
    }

    private void ActivateForm(int formToActivate)
    {
        GameObject playerObject = transform.root.gameObject;

        if (formToActivate == 0)
        {
            SetLayer(playerObject, LayerMask.NameToLayer("Orange"));
            playerSprite.GetComponent<SpriteRenderer>().color = orangeForm;
        }
        else if (formToActivate == 1)
        {
            SetLayer(playerObject, LayerMask.NameToLayer("Green"));
            playerSprite.GetComponent<SpriteRenderer>().color = greenForm;
        }
        else if (formToActivate == 2)
        {
            SetLayer(playerObject, LayerMask.NameToLayer("Pink"));
            playerSprite.GetComponent<SpriteRenderer>().color = pinkForm;
        }
        else if (formToActivate == 3)
        {
            SetLayer(playerObject, LayerMask.NameToLayer("Yellow"));
            playerSprite.GetComponent<SpriteRenderer>().color = yellowForm;
        }

        Debug.Log("Player layer set to: " + LayerMask.LayerToName(playerObject.layer));
    }

    // Set layer to all children of the player object
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

    private IEnumerator TemporarilyDisable(GameObject circle)
    {
        circle.SetActive(false);
        yield return null;
        Destroy(circle);
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            Quaternion startRotation = gunPivot.rotation;
            gunPivot.rotation = Quaternion.Lerp(startRotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void SetGrapplePoint()
    {
        RaycastHit2D hit = Physics2D.Raycast(
        firePoint.position,
        Mouse_FirePoint_DistanceVector.normalized,
        hasMaxDistance ? maxDistance : Mathf.Infinity
    );

        if (!hit) return;

        // Only grapple to objects on the SAME layer as the player
        if (hit.collider.gameObject.layer != gameObject.layer || hit.collider.CompareTag("Player"))
            return;

        isGrappling = true;
        grapplePoint = hit.point;
        DistanceVector = grapplePoint - (Vector2)gunPivot.position;
        grappleRope.enabled = true;
    }

    public void Grapple()
    {
        if (!launchToPoint && !autoCongifureDistance)
        {
            m_springJoint2D.distance = targetDistance;
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
