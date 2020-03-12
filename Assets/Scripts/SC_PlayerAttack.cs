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


    //public float attackDashForce;
    //public float attackPushForce;

    float timeBtwAttack;
    //public float startTimeBtwAttack;

    LayerMask whatIsEnemies;
    //public float attackRadius;
    //public float damage;
    Collider2D[] enemiesToDamage;


    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        playerPhysics = GetComponent<Rigidbody2D>();
        playerProperties = GetComponent<SC_PlayerProperties>();
        cameraController = FindObjectOfType<SC_CameraController>();
        whatIsEnemies = LayerMask.GetMask("Enemies");
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
                playerAnim.SetTrigger("Pressed Attack");
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
        playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
        playerPhysics.AddForce(Vector2.right * gameObject.transform.localScale.x * playerProperties.attackDashForce);

        //Hitbox
        enemiesToDamage = Physics2D.OverlapCircleAll(playerProperties.attackPos.position, playerProperties.attackRadius, whatIsEnemies);
        if (enemiesToDamage.Length > 0)
        {
            cameraController.Shake();

            foreach (Collider2D enemy in enemiesToDamage)
            {
                Debug.Log("We hit " + enemy.name);
                enemy.transform.position = new Vector2(playerProperties.attackPos.position.x, enemy.transform.position.y);
                enemy.GetComponent<SC_EnemyProperties>().TakeDamage(playerProperties.damage, playerProperties.attackPushForce * gameObject.transform.localScale.x ); //getcomponent and takedamage
            }
        }
        



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
