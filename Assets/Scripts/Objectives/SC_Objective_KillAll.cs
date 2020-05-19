using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(SC_PlayerObjectiveController))]
public class SC_Objective_KillAll : MonoBehaviour
{
    public int enemyCount;
    SC_PlayerObjectiveController objectiveController;

    // Start is called before the first frame update
    void Start()
    {
        objectiveController = FindObjectOfType<SC_PlayerObjectiveController>();
        if (enemyCount == 0)
        {
            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            objectiveController.objectiveText.GetComponent<TextMeshProUGUI>().text = string.Format("Eliminate All Enemies\nRemaining: {0}", enemyCount);
        }
    }

    public void EnemyDie()
    {
        if (enemyCount > 0)
        {
            enemyCount -= 1;

            if (enemyCount <= 0)
            {
                gameObject.GetComponent<SC_PlayerObjectiveController>().ObjectiveClear();

            }
            else
            {
                gameObject.GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(string.Format("Eliminate All Enemies\nRemaining: {0}", enemyCount));

            }
        }

    }
}
