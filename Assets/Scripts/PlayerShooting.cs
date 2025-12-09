using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 cursorPosition;

    [SerializeField] private GameObject pointer;

    [Header("Player Forms (child objects)")]
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private Sprite[] formSprite = new Sprite[] {};
    [SerializeField] private int StartForm = 0;
    [Header ("Colors")]
    [SerializeField] private Color orangeForm;
    [SerializeField] private Color greenForm;
    [SerializeField] private Color pinkForm;
    [SerializeField] private Color yellowForm;
    private Color currentColor;

    void Start()
    {
        if (pointer == null)
            pointer = GameObject.Find("Pointer");

        mainCamera = Camera.main;

        //orange default
       
        ActivateForm(StartForm);
     
    }

    void Update()
    {
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

   
    private void ActivateForm(int formToActivate)
    {

      
        if (formToActivate == 0)
        {
            gameObject.layer = LayerMask.NameToLayer("Orange");
            playerSprite.GetComponent<SpriteRenderer>().color = orangeForm;
        }
        else if (formToActivate == 1)
        {
            gameObject.layer = LayerMask.NameToLayer("Green");
            playerSprite.GetComponent<SpriteRenderer>().color = greenForm;
        }
        else if (formToActivate == 2)
        {
            gameObject.layer = LayerMask.NameToLayer("Pink");
            playerSprite.GetComponent<SpriteRenderer>().color = pinkForm;
        }           
        else if (formToActivate == 3)       
        {
            gameObject.layer = LayerMask.NameToLayer("Yellow");
            playerSprite.GetComponent<SpriteRenderer>().color = yellowForm;
        }

        Debug.Log("Player layer set to: " + LayerMask.LayerToName(gameObject.layer));
    }

    private IEnumerator TemporarilyDisable(GameObject circle)
    {
        circle.SetActive(false);
        yield return null;  
        Destroy(circle);    
    }

}
