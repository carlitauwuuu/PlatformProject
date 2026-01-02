using UnityEngine;

public class MousePointer : MonoBehaviour
{
    void Update()
    {
        
        Vector3 mousePosition = Input.mousePosition;

       
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

       
        mousePosition.z = 0;

        
        transform.position = mousePosition;
    }
}
