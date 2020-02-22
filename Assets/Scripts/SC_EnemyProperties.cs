using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyProperties : MonoBehaviour
{
    [Header("Health")]
    public float defaultHP;
    float HP;

    public float regenDelay;
    float regenDelayCount = 0;
    float regenRateCount = 0;
    public float regenPerSec;

    [Header("Posture")]
    public float defaultPosture;
    [SerializeField]
    float posture;
    public float stunKnockback;

    bool harderned;


    Rigidbody2D enemyPhysics;
    Animator enemyAnim;
    SC_EnemyMovement enemyMovement;

    // Start is called before the first frame update
    void Start()
    {
        enemyPhysics = gameObject.GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        enemyMovement = GetComponent<SC_EnemyMovement>();
        HP = defaultHP;
        posture = defaultPosture;
    }

    // Update is called once per frame
    void Update()
    {
        if (regenDelayCount <= 0)
        {
            if (HP > 0 && HP < defaultHP)
            {
                HealthRegen();
            }
        }
        else
        {
            regenDelayCount -= Time.deltaTime;
        }

        if (HP<= 0)
        {
            enemyAnim.SetTrigger("Die");
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    public void TakeDamage(float damage, float push)
    {
        enemyMovement.CanMove = false;
        regenDelayCount = regenDelay;
        HP -= damage;

        if (!harderned)
        {
            enemyAnim.SetTrigger("Hurt");
            enemyPhysics.velocity = new Vector2(0, enemyPhysics.velocity.y);
            enemyPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
        }


    }

    public void HealthRegen()
    {
        if (regenRateCount <= 0)
        {
            HP += regenPerSec;
            regenRateCount = 1;
        }
        else
        {
            regenRateCount -= Time.deltaTime;
        }


    }

    public void Die()
    {
       Destroy(gameObject);
    }

    void Harderned()
    {
        harderned = true;
    }
    void StopHarderned()
    {
        harderned = false;
    }

    public void Deflected(float postureDamage)
    {
        posture -= postureDamage;
        if (posture <= 0)
        {
            Stunned();
        }
    }

    void Stunned()
    {
        enemyAnim.SetTrigger("Stunned");
        enemyPhysics.AddForce(Vector2.left * stunKnockback * transform.localScale.x, ForceMode2D.Impulse);
    }

    void ResetPosture()
    {
        posture = defaultPosture;
    }
}
