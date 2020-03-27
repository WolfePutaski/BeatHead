using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyAttack : MonoBehaviour
{
    Animator enemyAnim;
    SC_EnemyProperties enemyProperties;

    public bool OnScanning = true;
    public bool isAttacking= false;

    [SerializeField]
    float timeBtwAttack;
    public float startTimeBtwAttack;
    LayerMask whatIsPlayer;
    public float attackRadius;

    Rigidbody2D enemyPhysics;
    public GameObject Target;

    public Transform attackPos;
    float attackDash;
    float damage;
    float postureDamage;
    float attackPush;
    public Collider2D attackTarget;

    [Header("LightAttack")]
    public float lightDamage;
    public float lightPostureDmg;
    public float lightAttackDash;
    public float lightAttackPush;
    public float lightTBA;

    [Header("HeavyAttack")]
    public float heavyDamage;
    public float heavyPostureDmg;
    public float heavyAttackDash;
    public float heavyAttackPush;
    public float heavyTBA;
    SC_EnemyMovement enemyMovement;





    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.Find("Player");
        enemyProperties = GetComponent<SC_EnemyProperties>();
        enemyPhysics = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<SC_EnemyMovement>();
        enemyAnim = GetComponent<Animator>();
        whatIsPlayer = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBtwAttack <= 0 && enemyMovement.IsEngaging)
        {
            OnScanning = true;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        if (!enemyMovement.IsEngaging)
        {
            OnScanning = false;
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
        postureDamage = lightPostureDmg;
        attackDash = lightAttackDash;
        attackPush = lightAttackPush;
        startTimeBtwAttack = lightTBA;
    }

    void SetAttack_Heavy()
    {
        damage = heavyDamage;
        postureDamage = heavyPostureDmg;
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

        attackTarget = null;


        //Hitbox
        attackTarget = Physics2D.OverlapCircle(attackPos.position, attackRadius, whatIsPlayer);
        if (attackTarget != null)
        {
            attackTarget.transform.position = new Vector2(attackPos.position.x, attackTarget.transform.position.y);
            if (attackTarget.GetComponent<SC_PlayerProperties>().onDeflect)
            {
                enemyProperties.Deflected(1);
                Debug.Log("Deflect!");
                attackTarget.SendMessage("Deflect", SendMessageOptions.DontRequireReceiver);
                if (enemyProperties.posture <= 0)
                {
                    attackTarget.SendMessage("DeflectSuccess", SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                attackTarget.transform.position = new Vector2(attackPos.position.x, attackTarget.transform.position.y);
                attackTarget.GetComponent<SC_PlayerProperties>().Attacked(damage, postureDamage, attackPush * transform.localScale.x); //getcomponent and takedamage
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

    void NotAttacking()
    {
        isAttacking = false;
        enemyAnim.ResetTrigger("Attack");
    }
}



