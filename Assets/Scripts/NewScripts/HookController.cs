using UnityEngine;

public class HookController : MonoBehaviour
{
    [SerializeField] private float grappleLength = 10f;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer line;

    private Vector3 grapplePoint;
    private DistanceJoint2D joint;

    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;

        if (line != null)
            line.enabled = false;
    }

    void Update()
    {
        // Shoot grapple
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (worldMouse - (Vector2)transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, grappleLength, grappleLayer);

            if (hit.collider != null)
            {
                grapplePoint = hit.point;
                joint.connectedAnchor = grapplePoint;
                joint.distance = Vector2.Distance(transform.position, grapplePoint);
                joint.enabled = true;

                if (line != null)
                {
                    line.enabled = true;
                    line.SetPosition(0, grapplePoint);
                    line.SetPosition(1, transform.position);
                }
            }
        }

        // Release grapple
        if (Input.GetMouseButtonDown(1))
        {
            joint.enabled = false;
            if (line != null)
                line.enabled = false;
        }

        // Update line position
        if (line != null && line.enabled)
            line.SetPosition(1, transform.position);

        //  Adjust grapple length dynamically
        if (joint.enabled)
        {
            // Scroll wheel control
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                joint.distance -= scroll * 2f;
                joint.distance = Mathf.Clamp(joint.distance, 1f, grappleLength);
            }

            // Middle mouse hold control
            if (Input.GetMouseButton(2))
            {
                joint.distance -= Time.deltaTime * 5f;
                joint.distance = Mathf.Clamp(joint.distance, 1f, grappleLength);
            }
        }
    }
}
