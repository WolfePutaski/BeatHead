using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SC_PlayerProperties))]
//[RequireComponent(typeof(Animator))]//[RequireComponent(typeof(Animator))]

public class SC_PlayerAttack : MonoBehaviour
{
    bool canAttack = true;

    Animator playerAnim;
    Rigidbody2D playerPhysics;
    SC_PlayerProperties playerProperties;
    SC_CameraController cameraController;

    public float attackDashForce;
    public float attackPushModifier;

    float timeBtwAttack;
    public float startTimeBtwAttack;

    public Transform attackPos;
    LayerMask whatIsEnemies;
    public float attackRadius;
    public float damage;
    Collider2D[] enemiesToDamage;


    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
        playerPhysics = GetComponent<Rigidbody2D>();
        playerProperties = GetComponent<SC_PlayerProperties>();
        attackPos = GameObject.Find("AttackPos").GetComponent<Transform>();
        whatIsEnemies = LayerMask.GetMask("Enemies");
        cameraController = FindObjectOfType<SC_CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBtwAttack <= 0)
        {
            if (canAttack && Input.GetMouseButtonDown(0))
            {
                playerAnim.SetTrigger("Pressed Attack");
            }
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

    }

    void Attack()
    {

        timeBtwAttack = startTimeBtwAttack;

        Debug.Log("Pressed Attack");
        playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
        playerPhysics.AddForce(Vector2.right * gameObject.transform.localScale.x * attackDashForce, ForceMode2D.Impulse);

        //Hitbox
        enemiesToDamage = null;
        enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRadius, whatIsEnemies);
        if (enemiesToDamage.Length > 0)
        {
            cameraController.Shake();

            foreach (Collider2D enemy in enemiesToDamage)
            {
                Debug.Log("We hit " + enemy.name);
                enemy.transform.position = new Vector2(attackPos.position.x, enemy.transform.position.y);
                enemy.GetComponent<SC_EnemyProperties>().TakeDamage(damage,attackDashForce * attackPushModifier * gameObject.transform.localScale.x); //getcomponent and takedamage
            }
        }


    }



    void NotAllowAttack()
    {
        canAttack = false;
    }

    void AllowAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }

}
