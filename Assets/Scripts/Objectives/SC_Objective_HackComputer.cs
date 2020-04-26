using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SC_PlayerObjectiveController))]
public class SC_Objective_HackComputer : MonoBehaviour
{
    public bool isCompleted;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<SC_InteractableObject_Computer>().isObjectiveActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCompleted)
        {
            GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText("Hack The Computer.");
        }
        
    }
}
