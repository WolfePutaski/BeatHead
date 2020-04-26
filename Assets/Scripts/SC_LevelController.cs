using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LevelController : MonoBehaviour
{
    public List<string> objectiveList;
    GameObject player;
    public int objectiveNumber;
    // Start is called before the first frame update

    void Start()
    {
        player = FindObjectOfType<SC_PlayerProperties>().gameObject;
        objectiveNumber = 0;
        player.GetComponent<SC_PlayerObjectiveController>().newObjectiveScriptName = objectiveList[0];

        player.GetComponent<SC_PlayerObjectiveController>().GetNewObjective();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
