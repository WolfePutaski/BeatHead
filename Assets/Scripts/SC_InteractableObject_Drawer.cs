using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_InteractableObject_Drawer : MonoBehaviour
{
    /// <summary>
    /// isSearched >> isContained = faalse;
    /// </summary>
    
    public GameObject textHelp;
    public GameObject holdBarGroup;
    public GameObject holdFill;
    public GameObject highlight;

    public GameObject levelController;
    public bool isObjectiveActive;

    public float secondsHeldMax;
    float secondsHeld = 0;

    public bool isSearched;
    public bool interactable;

    public bool isContained;

    public bool onPlayer;


    // Start is called before the first frame update
    void Start()
    {
        textHelp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isObjectiveActive)
        {
            interactable = false;
            highlight.SetActive(false);
            gameObject.tag = "Untagged";
        }
        else
        {
            if (!isSearched)
            {
                interactable = true;

                gameObject.tag = "InteractableObject";

            }
        }

        UpdateHUD();

        if (isSearched)
        {
            interactable = false;
            highlight.SetActive(false);
            gameObject.tag = "Untagged";
            textHelp.SetActive(false);
        }
        else
        {
            if (isObjectiveActive)
            {
                highlight.SetActive(true);
            }
            else
            {
                highlight.SetActive(false);
            }
        }

        if (onPlayer)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                ResetHold();
            }
        }

        ItemTaken();
    }

    void UpdateHUD()
    {
        holdFill.transform.localScale = new Vector3((secondsHeld / secondsHeldMax), 1, 1);

        holdBarGroup.SetActive(secondsHeld > 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            textHelp.SetActive(interactable);

            onPlayer = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        secondsHeld = 0;

        if (collision.gameObject.CompareTag("Player"))
        {
            textHelp.SetActive(false);

            onPlayer = false;
        }


        
    }

    public void PlayerInteract()
    {
        if (interactable)
        {
            if (secondsHeld < secondsHeldMax)
            {
                secondsHeld += Time.deltaTime;
            }
            if (secondsHeld >= secondsHeldMax)
            {
                isSearched = true;

                secondsHeld = 0;
            }

        }

    }

    public void ResetHold()
    {
        StartCoroutine("ResetHold2");
    }

    private IEnumerator ResetHold2()
    {
        interactable = false;
        secondsHeld = -1f;
        yield return new WaitForSeconds(.3f);
        interactable = true;
        secondsHeld = 0;

    }

    public void ItemTaken()
    {
        if (isSearched)
        {
            if (isContained)
            {
                Debug.Log("Item Found!");
                levelController.SendMessage("ItemFound", SendMessageOptions.DontRequireReceiver);
                isContained = false;
            }
            else
            {
                Debug.Log("Not Found!");
                levelController.SendMessage("ItemNotFound", SendMessageOptions.DontRequireReceiver);
            }
            Destroy(gameObject);
        }
    }
}

