using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(SC_PlayerObjectiveController))]
public class SC_Objective_HackComputer : MonoBehaviour
{
    public bool isCompleted;

    public string text_Start = "Hack the Computer.";
    public string text_Fix = "Fix the Computer!";

    public bool isDoneTrigger;



    public SC_InteractableObject_Computer computerObjectScript;
    public SC_PlayerObjectiveController playerObjectiveController;
    // Start is called before the first frame update
    void Start()
    {
        computerObjectScript = FindObjectOfType<SC_InteractableObject_Computer>();
        FindObjectOfType<SC_InteractableObject_Computer>().isObjectiveActive = true;
        GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(text_Start);
        playerObjectiveController = GetComponent<SC_PlayerObjectiveController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (computerObjectScript.state == SC_InteractableObject_Computer.CompueterState.Done && !isDoneTrigger)
        {
            isDoneTrigger = true;
            playerObjectiveController.ObjectiveClear();
        }
    }
}
