using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(SC_EnemyProperties))]


public class SC_EnemyBossDrugGuy_Properties : MonoBehaviour
{
    public enum DrugBossState
    {
        Normal, Mad, Dead
    }

    [HideInInspector] public SC_EnemyProperties enemyProperties;
    Animator enemyAnim;
    public RuntimeAnimatorController normalAnimator;
    public RuntimeAnimatorController madAnimator;
    public int currentPhase; // 0,1,2,3, 4== dead
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

        if (state == DrugBossState.Normal)
        {
            if (enemyProperties.HP <= 0)
            {
                state = DrugBossState.Dead;
            }
        }

        if (state == DrugBossState.Mad)
        {
            MadFunctions();
        }

        if (state == DrugBossState.Dead)
        {
            enemyProperties.HP = 0;
            enemyProperties.DHP = 0;
            gameObject.tag = "Untagged";
            enemyAnim.ResetTrigger("GetUp");
        }
        else
        {
            gameObject.tag = "Enemy_Boss";
        }

    }

    void MadFunctions()
    {

        // When Executed
        if (DHPFreezeNum != enemyProperties.DHP)
        {
            if (isInjected)

            {
                DHPLocked = false;
                DHPFreezeNum = enemyProperties.DHP;
                KnockedDown();
                isInjected = false;
            }

            else
            {
                DHPLocked = true;

                if (enemyProperties.DHP >= 0)
                {
                    enemyProperties.DHP = Mathf.Clamp(DHPFreezeNum, 1, enemyProperties.defaultDHP);

                }

                if (DHPLocked)
                {
                    // getexecuted
                    
                    enemyAnim.SetTrigger("Die");
                    enemyAnim.SetTrigger("GetUp");
                    enemyProperties.harderned = false;
                    enemyAnim.SetTrigger("StunAfterExecute");
                    gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");


                }


            }
        }

        if (enemyProperties.DHP > 1)
        {
            enemyProperties.HP = Mathf.Clamp(enemyProperties.HP, 0.01f, enemyProperties.defaultHP);

        }
        else if (enemyProperties.DHP == 1)
        {
            enemyProperties.HP = enemyProperties.defaultHP;

        }
        else if (enemyProperties.DHP <= 0)
        {
            enemyProperties.HP = 0;
        }

        if (enemyProperties.HP <= 0.01f)
        {
            enemyProperties.HP = enemyProperties.defaultHP;


            if (enemyProperties.DHP > 0)
            {
                enemyAnim.ResetTrigger("Hurt");
                enemyProperties.Stunned();
                enemyProperties.PlaySound("Player_DeflectSuccess");
                FindObjectOfType<SC_PlayerBlock>().PlayParticle();

            }


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
        }
        else
        {
            state = DrugBossState.Dead;

            enemyAnim.ResetTrigger("GetUp");


        }

    }

    void GetUp()
    {
        enemyAnim.SetTrigger("StunAfterExecute");
        enemyProperties.HP = enemyProperties.defaultHP;
        enemyAnim.SetTrigger("GetUp");
        enemyProperties.harderned = false;

    }

    //When Mad, DHP can be depleted only when Injected!
}
