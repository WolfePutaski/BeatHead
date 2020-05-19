using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LevelController : MonoBehaviour
{
    public List<string> objectiveList;
    GameObject player;
    public int objectiveNumber;
    SC_PlayerObjectiveController objectiveController;

    //[System.Serializable]
    //public class BossTrigger
    //{
    //    public GameObject Boss;
    //    public GameObject BossDoorTrigger;
    //    public bool bossTriggered;
    //}

    //public BossTrigger bossTrigger;

    // Start is called before the first frame update

    void Start()
    {
        player = FindObjectOfType<SC_PlayerProperties>().gameObject;

        objectiveNumber = 0;

        objectiveController = player.GetComponent<SC_PlayerObjectiveController>();

        player.GetComponent<SC_PlayerObjectiveController>().newObjectiveScriptName = objectiveList[objectiveNumber];

        if (player.GetComponent<SC_PlayerObjectiveController>().hasObjective == false)
        {
            player.GetComponent<SC_PlayerObjectiveController>().GetNewObjective();
        }

        //bossTrigger.Boss = GameObject.FindObjectOfType<SC_EnemyBossDrugGuy_Properties>().gameObject;

        //bossTrigger.Boss.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //if (Physics2D.OverlapCircle(bossTrigger.BossDoorTrigger.transform.position, 1,LayerMask.GetMask("Player"))
        //    &&!bossTrigger.bossTriggered)
        //{
        //    bossTrigger.Boss.SetActive(true);
        //    bossTrigger.bossTriggered = true;
        //}
    }

    public void GoToNextObjective()
    {

        if(objectiveNumber +1 < objectiveList.Count)
        {
            objectiveNumber++;
            player.GetComponent<SC_PlayerObjectiveController>().newObjectiveScriptName = objectiveList[objectiveNumber];
            player.GetComponent<SC_PlayerObjectiveController>().GetNewObjective();
        }
        else
        {
            player.GetComponent<SC_PlayerObjectiveController>().RemoveScript();

        }
    }
}
