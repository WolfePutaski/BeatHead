using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct Objectives
{
    public string scriptName;
    public string description;
}

public class SC_PlayerObjective : MonoBehaviour
{
    public Objectives currentObjective;
    public string currentObjectiveName;
    public GameObject objectiveText;
    public int enemyCount;
    // Start is called before the first frame update
    void Start()
    {
        objectiveText.GetComponent<TextMeshProUGUI>().text = string.Format("Wait Until The Drill is Done");

    }

    // Update is called once per frame
    void Update()
    {
        //objectiveText.GetComponent<TextMeshProUGUI>().text = string.Format("Eliminate All Enemies\nRemaining: {0}", enemyCount);
    }

    void UpdateObjectives()
    {
        //if (currentObjective)
    }


    public void EnemyDie()
    {
        if (enemyCount > 0)
        {
            enemyCount -= 1;
        }
        else
        {
            objectiveText.GetComponent<TextMeshProUGUI>().text = string.Format("Objective is Clear!");

        }
    }

    public void ObjectiveClear()
    {
        objectiveText.GetComponent<TextMeshProUGUI>().text = string.Format("Objective is Clear!");
    }
}
