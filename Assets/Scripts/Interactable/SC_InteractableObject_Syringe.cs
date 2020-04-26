using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_InteractableObject_Syringe : MonoBehaviour
{
    public GameObject textHelp;
    public GameObject marker;
    public SC_Objective_KillBoss1 objective_KillBoss1;
    public bool isObjectiveActive;
    public bool isInteractable;
    bool onPlayer = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isObjectiveActive)
        {
            isInteractable = true;
        }

        if (onPlayer)
        {
            textHelp.SetActive(isInteractable);
            marker.SetActive(false);
        }
        else
        {
            textHelp.SetActive(false);
            marker.SetActive(isInteractable);
        }

        if (FindObjectOfType<SC_Objective_KillBoss1>() != null)
        {
            //FindObjectOfType<SC_Objective_KillBoss1>().SyringeFound = true;
            isObjectiveActive = true;
        }

    }

    public void PlayerInteract()
    {
        if (isInteractable)
        {
            FindObjectOfType<SC_Objective_KillBoss1>().PickSyringe();
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {

            onPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            onPlayer = false;

        }
    }
}
