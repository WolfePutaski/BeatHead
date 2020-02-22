using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyAttack : MonoBehaviour
{
    Animator enemyAnim;

    public bool OnScanning = true;
    bool isAttacking= false;

    float timeBtwAttack;
    public float startTimeBtwAttack;
    LayerMask whatIsPlayer;
    public float attackRadius;

    Rigidbody2D enemyPhysics;
    public GameObject Target;

    public Transform attackPos;
    public float attackDash;
    public float damage;
    public float attackPush;
    public Collider2D attackTarget;

    [Header("LightAttack")]
    public float lightDamage;
    public float lightAttackDash;
    public float lightAttackPush;
    public float lightTBA;

    [Header("HeavyAttack")]
    public float heavyDamage;
    public float heavyAttackDash;
    public float heavyAttackPush;
    public float heavyTBA;


    SC_CameraController cameraController;
    SC_EnemyMovement enemyMovement;





    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.Find("Player");
        enemyPhysics = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<SC_EnemyMovement>();
        enemyAnim = GetComponent<Animator>();
        whatIsPlayer = LayerMask.GetMask("Player");
        cameraController = FindObjectOfType<SC_CameraController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (timeBtwAttack <= 0)
        {
            OnScanning = true;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        if (OnScanning && enemyMovement.IsEngaging)
        {
            ScanForAttack();
        }


    }

    public void ScanForAttack()
    {
        if (!isAttacking && timeBtwAttack <= 0 && OnScanning && Mathf.Abs(enemyMovement.distanceFromTarget) < enemyMovement.closeDistance)
        {
            isAttacking = true;
            enemyAnim.SetTrigger("Attack");
            Debug.Log("Enemy Attack Triggered");

        }
    }

    void SetAttack_Light()
    {
        damage = lightDamage;
        attackDash = lightAttackDash;
        attackPush = lightAttackPush;
        startTimeBtwAttack = lightTBA;
    }

    void SetAttack_Heavy()
    {
        damage = heavyDamage;
        attackDash = heavyAttackDash;
        attackPush = heavyAttackPush;
        startTimeBtwAttack = heavyTBA;
    }


    public void Attack()
    {
        isAttacking = false;
        timeBtwAttack = startTimeBtwAttack;
        enemyAnim.SetInteger("AttackSequence", enemyAnim.GetInteger("AttackSequence") + 1);
        enemyPhysics.velocity = new Vector2(0, enemyPhysics.velocity.y);
        enemyPhysics.AddForce(Vector2.right * gameObject.transform.localScale.x * attackDash);

        enemyMovement.CancelAttack(); 
        attackTarget = null;


        //Hitbox
        attackTarget = Physics2D.OverlapCircle(attackPos.position, attackRadius, whatIsPlayer);
        if (attackTarget != null)
        {
            attackTarget.transform.position = new Vector2(attackPos.position.x, attackTarget.transform.position.y);
            if (attackTarget.GetComponent<SC_PlayerBlock>().onDeflect)
            {
                GetComponent<SC_EnemyProperties>().Deflected(1);
            }
            else
            {
                attackTarget.transform.position = new Vector2(attackPos.position.x, attackTarget.transform.position.y);
                attackTarget.GetComponent<SC_PlayerProperties>().Attacked(damage, attackPush * transform.localScale.x); //getcomponent and takedamage
                Debug.Log("Player Attacked");
            }

        }


    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
    

    void AllowtoAttack()
    {
        OnScanning = true;

    }

    void NotAllowtoAttack()
    {
        OnScanning = false;
    }

    void ResetCombo()
    {
        enemyAnim.SetInteger("AttackSequence", 0);
    }
}



