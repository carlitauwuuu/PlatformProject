using UnityEngine;

public class MouseController : MonoBehaviour
{
    private Camera cameraMouse;
    private Vector3 mousePosition;
    [SerializeField] GameObject pointer;
    [SerializeField] float pointerspeed = 0.2f;
    void Start()
    {
        cameraMouse = Camera.main;
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = cameraMouse.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;
        }
        pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, mousePosition, pointerspeed);
        if (Input.GetMouseButtonDown(1))
        {

        }
    }
}
