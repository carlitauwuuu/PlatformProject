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
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private Sprite[] formSprite = new Sprite[] {};
    [SerializeField] private int StartForm = 0;

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
            playerSprite.GetComponent<SpriteRenderer>().sprite = formSprite[formToActivate];
        }
        else if (formToActivate == 1)
        {
            gameObject.layer = LayerMask.NameToLayer("Green");
            playerSprite.GetComponent<SpriteRenderer>().sprite = formSprite[formToActivate];
        }
        else if (formToActivate == 2)
        {
            gameObject.layer = LayerMask.NameToLayer("Pink");
            playerSprite.GetComponent<SpriteRenderer>().sprite = formSprite[formToActivate];
        }           
        else if (formToActivate == 3)       
        {
            gameObject.layer = LayerMask.NameToLayer("Yellow");
            playerSprite.GetComponent<SpriteRenderer>().sprite = formSprite[formToActivate];
        }

        Debug.Log("Player layer set to: " + LayerMask.LayerToName(gameObject.layer));
    }

    private IEnumerator TemporarilyDisable(GameObject circle)
    {
        circle.SetActive(false);           
        yield return new WaitForSeconds(5f); 
        circle.SetActive(true);            
    }
}
