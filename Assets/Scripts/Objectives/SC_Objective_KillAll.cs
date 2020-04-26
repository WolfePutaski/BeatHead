using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(SC_PlayerObjectiveController))]
public class SC_Objective_KillAll : MonoBehaviour
{
    public int enemyCount;

    // Start is called before the first frame update
    void Start()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCount > 0)
        {
            gameObject.GetComponent<SC_PlayerObjectiveController>().UpdateNewObjectiveText(string.Format("Eliminate All Enemies\nRemaining: {0}", enemyCount));

        }
    }

    public void EnemyDie()
    {
        if (enemyCount > 0)
        {
            enemyCount -= 1;
        }
        else
        {
            gameObject.GetComponent<SC_PlayerObjectiveController>().ObjectiveClear();

        }
    }
}
