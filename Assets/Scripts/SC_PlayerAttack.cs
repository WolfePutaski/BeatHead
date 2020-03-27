using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SC_PlayerProperties))]
//[RequireComponent(typeof(Animator))]//[RequireComponent(typeof(Animator))]

public class SC_PlayerAttack : MonoBehaviour
{


    //bool canAttack = true;


    Animator playerAnim;
    Rigidbody2D playerPhysics;
    SC_PlayerProperties playerProperties;
    SC_CameraController cameraController;
    bool isExecuting;

    //public float attackDashForce;
    //public float attackPushForce;

    float timeBtwAttack;
    //public float startTimeBtwAttack;

    LayerMask whatIsEnemies;
    LayerMask executeLayer;
    //public float attackRadius;
    //public float damage;
    Collider2D[] enemiesToDamage;
    Collider2D enemyToExecute;


    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        playerPhysics = GetComponent<Rigidbody2D>();
        playerProperties = GetComponent<SC_PlayerProperties>();
        cameraController = FindObjectOfType<SC_CameraController>();
        whatIsEnemies = LayerMask.GetMask("Enemies");
        executeLayer = LayerMask.GetMask("Enemies_ToExecute");
        enemiesToDamage = null;

    }

    // Update is called once per frame
    void  Update()
    {

        if (timeBtwAttack <= 0)
        {
            playerProperties.canAttack = true;

            if (playerProperties.canAttack && Input.GetMouseButtonDown(0))
            {
                //Check Execution
                enemyToExecute = null;
                if (Physics2D.OverlapCircle(playerProperties.attackPos.position, playerProperties.attackRadius, executeLayer))
                {
                    enemyToExecute = Physics2D.OverlapCircle(playerProperties.attackPos.position, playerProperties.attackRadius, executeLayer);
                    TriggerExecuteAttack(enemyToExecute.gameObject);
                }
                //if not, play normal attack
                else
                {

                    playerAnim.SetTrigger("Pressed Attack");

                }

            }
        }
        else
        {
            playerProperties.canAttack = false;
            timeBtwAttack -= Time.deltaTime;
        }

    }

    void Attack()
    {
        enemiesToDamage = null;
        playerProperties.canMove = false;
        playerProperties.canBlock = false;

        timeBtwAttack = playerProperties.startTimeBtwAttack;

        Debug.Log("Pressed Attack");
        AttackDash();

        //Hitbox
        if (isExecuting)
        {
            enemyToExecute = Physics2D.OverlapCircle(playerProperties.attackPos.position, playerProperties.attackRadius, executeLayer);
            if (enemyToExecute != null)
            {
                cameraController.Shake();

                GameObject enemy = enemyToExecute.gameObject;

                Debug.Log("We execute " + enemy.name);
                enemy.transform.position = new Vector2(playerProperties.attackPos.position.x, enemy.transform.position.y);
                enemy.GetComponent<SC_EnemyProperties>().TakeDamage(playerProperties.damage, playerProperties.attackPushForce * gameObject.transform.localScale.x,true); //getcomponent and takedamage

                if (enemy.GetComponent<SC_EnemyProperties>().HP <= 0)
                {
                    playerProperties.BigHPRefillCount += 0.4f;
                    playerProperties.HP = Mathf.Clamp(playerProperties.HP + 1.5f, playerProperties.maxHP/2, playerProperties.maxHP);
                }
            }
        }
    
        else
        {

        enemiesToDamage = Physics2D.OverlapCircleAll(playerProperties.attackPos.position, playerProperties.attackRadius, whatIsEnemies);
            if (enemiesToDamage.Length > 0)
            {
                cameraController.Shake();

                foreach (Collider2D enemy in enemiesToDamage)
                {
                    var e = enemy.GetComponent<SC_EnemyProperties>();
                    Debug.Log("We hit " + enemy.name);
                    //if (!e.harderned)
                    //{
                    //    enemy.transform.position = new Vector2(playerProperties.attackPos.position.x, enemy.transform.position.y);
                    //}

                    e.TakeDamage(playerProperties.damage, playerProperties.attackPushForce * gameObject.transform.localScale.x,false); //getcomponent and takedamage
                }
            }
        }

        isExecuting = false;
    }

    void AttackDash()
    {
        playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
        playerPhysics.AddForce(Vector2.right * transform.localScale.x * playerProperties.attackDashForce, ForceMode2D.Impulse);
    }

    void TriggerExecuteAttack(GameObject enemy)

    {
        cameraController.activeCam = "Zoom Cam";
        isExecuting = true;
        //cameraController
        enemy.SendMessage("OnExecuted");
        Debug.Log("Execute " + enemy.name);
        enemy.transform.position = playerProperties.executionPos.transform.position;
        playerAnim.SetTrigger("Execution");

    }




    void Active_canAttack()
    {
        playerProperties.canAttack = false;
    }

    void Deactive_canAttack()
    {
        playerProperties.canAttack = true;
    }
}
