using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(SC_EnemyProperties))]


public class SC_EnemyBossDrugGuy_Properties : MonoBehaviour
{
    public enum DrugBossState
    {
        Normal, Mad
    }

    SC_EnemyProperties enemyProperties;
    Animator enemyAnim;
    public RuntimeAnimatorController normalAnimator;
    public RuntimeAnimatorController madAnimator;
    public int currentPhase; // 0,1,2,3
    public bool isInjected;
    public DrugBossState state;
    public bool DHPLocked;
    public int DHPFreezeNum;
    // Start is called before the first frame update
    void Start()
    {
        enemyProperties = gameObject.GetComponent<SC_EnemyProperties>();
        enemyAnim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        PhaseCheck();

        if (state == DrugBossState.Mad)
        {
            MadFunctions();
        }

    }

    void MadFunctions()
    {
        if (!DHPLocked)
        {
            DHPFreezeNum = enemyProperties.DHP;
            //DHPLocked = true;
        }

        if (!isInjected)

        {
            DHPLocked = true;

            if (DHPLocked)
            {
                if (DHPFreezeNum != enemyProperties.DHP)
                {
                    enemyProperties.DHP = DHPFreezeNum;
                    enemyAnim.SetTrigger("StunAfterExecute");
                }
            }

        }

        if (isInjected)
        {
            if (DHPFreezeNum != enemyProperties.DHP)
            {
                DHPFreezeNum = enemyProperties.DHP;
                KnockedDown();
                isInjected = false;
            }
        }

        enemyProperties.HP = Mathf.Clamp(enemyProperties.HP, 0.01f, enemyProperties.defaultHP);
        if (enemyProperties.HP <= 0.01f)
        {
            enemyProperties.HP = enemyProperties.defaultHP;
            enemyAnim.SetTrigger("StunAfterExecute");
        }
    }

    void PhaseCheck()
    {
        if (state == DrugBossState.Normal)
        {
            enemyAnim.runtimeAnimatorController = normalAnimator;
            currentPhase = 0;
        }
        if (state == DrugBossState.Mad)
        {
            enemyAnim.runtimeAnimatorController = madAnimator;
            currentPhase = 5 - enemyProperties.DHP;
        }


    }

    void KnockedDown()
    {

        enemyAnim.SetTrigger("Die");
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        if (enemyProperties.DHP > 0)
        {
            GetUp();
            enemyAnim.SetTrigger("StunAfterExecute");
        }

    }

    void GetUp()
    {
        enemyProperties.HP = enemyProperties.defaultHP;
        enemyAnim.SetTrigger("GetUp");
    }

    //When Mad, DHP can be depleted only when Injected!
}
