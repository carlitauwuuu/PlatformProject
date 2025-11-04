using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 cursorPosition;

    [SerializeField] private GameObject pointer;

    [Header("Player Forms (child objects)")]
    [SerializeField] private GameObject orangeForm;
    [SerializeField] private GameObject greenForm;
    [SerializeField] private GameObject pinkForm;
    [SerializeField] private GameObject yellowForm;

    private GameObject currentForm;

    void Start()
    {
        if (pointer == null)
            pointer = GameObject.Find("Pointer");

        mainCamera = Camera.main;

        //orange default
        currentForm = orangeForm;
        ActivateForm(currentForm);
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

               
                if (hit.collider.CompareTag("Orange"))
                {
                    ActivateForm(orangeForm);
                    StartCoroutine(TemporarilyDisable(hit.collider.gameObject));
                }
                else if (hit.collider.CompareTag("Green"))
                {
                    ActivateForm(greenForm);
                    StartCoroutine(TemporarilyDisable(hit.collider.gameObject));
                }
                else if (hit.collider.CompareTag("Pink"))
                {
                    ActivateForm(pinkForm);
                    StartCoroutine(TemporarilyDisable(hit.collider.gameObject));
                }
                else if (hit.collider.CompareTag("Yellow"))
                {
                    ActivateForm(yellowForm);
                    StartCoroutine(TemporarilyDisable(hit.collider.gameObject));
                }
            }
            else
            {
                pointer.transform.position = cursorPosition;
            }
        }
    }

   
    private void ActivateForm(GameObject formToActivate)
    {
        
        orangeForm.SetActive(false);
        greenForm.SetActive(false);
        pinkForm.SetActive(false);
        yellowForm.SetActive(false);

        
        formToActivate.SetActive(true);
        currentForm = formToActivate;

        
        if (formToActivate == orangeForm)
            gameObject.layer = LayerMask.NameToLayer("Orange");
           
        else if (formToActivate == greenForm)
            gameObject.layer = LayerMask.NameToLayer("Green");
        else if (formToActivate == pinkForm)
            gameObject.layer = LayerMask.NameToLayer("Pink");
        else if (formToActivate == yellowForm)
            gameObject.layer = LayerMask.NameToLayer("Yellow");

        Debug.Log("Player layer set to: " + LayerMask.LayerToName(gameObject.layer));
    }

    private IEnumerator TemporarilyDisable(GameObject circle)
    {
        circle.SetActive(false);           
        yield return new WaitForSeconds(5f); 
        circle.SetActive(true);            
    }
}
