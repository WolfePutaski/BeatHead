using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackMode
{
    Chasing, Shooting
}

public class SC_EnemyRanged : MonoBehaviour
{
    SC_EnemyProperties enemyProperties;
    Animator enemyAnim;
    SC_EnemyMovement enemyMovement;
    
    public float shootDamage;
    public float timeBtwShot;
    public float timeBtwShotRandom;
    private float timeBtwShotCount;

    public float reloadTime;

    public int maxAmmo;
    private int ammoCount;

    public float chaseRange;
    public float minShootRange;
    public float minShootRangeRandom;

    Transform muzzlePos;

    //public GameObject bulletPrefab;
    //public Collider2D attackTarget;

    public AttackMode attackMode;

    //Shooting
    bool isAiming;
    public LayerMask whatToHit;
    


    // Start is called before the first frame update
    void Start()
    {
        enemyProperties = GetComponent<SC_EnemyProperties>();
        enemyAnim = GetComponent<Animator>();
        enemyMovement = GetComponent<SC_EnemyMovement>();
        minShootRange -= Random.Range(0, minShootRangeRandom);
        
    }

    // Update is called once per frame
    void Update()
    {

        AimingCheck();
        ShootCondition();
    }

    void AimingCheck()
    {
        enemyAnim.SetBool("isAiming", isAiming);


        if (attackMode == AttackMode.Shooting)
        {
            Debug.DrawLine(gameObject.transform.position + Vector3.up, gameObject.transform.position + Vector3.up + Vector3.right * transform.localScale.x * minShootRange, Color.green);

            if (Mathf.Abs(enemyMovement.distanceFromTarget) <= minShootRange && Mathf.Abs(enemyMovement.distanceFromTarget) > chaseRange)
            {

                if (isAiming == false && Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 1), Vector2.right * transform.localScale.x, minShootRange, whatToHit))
                {
                    isAiming = true;
                    enemyProperties.PlaySound("EnemyHandgun_Ready");

                }
            }
            else
            {
                enemyMovement.CanMove = true;
                attackMode = AttackMode.Chasing;
            }
        }

        if (attackMode == AttackMode.Chasing)
        {
            isAiming = false;
           if(  Mathf.Abs(enemyMovement.distanceFromTarget) > chaseRange )
            {
                attackMode = AttackMode.Shooting;
            }
        }
    }

    void ShootCondition()
    {
        if (isAiming)
        {

            if (enemyMovement.distanceFromTarget < 0)
            {
                transform.localScale = new Vector2(enemyMovement.defaultScaleX, transform.localScale.y);
            }
            if (enemyMovement.distanceFromTarget > 0)
            {
                transform.localScale = new Vector2(-enemyMovement.defaultScaleX, transform.localScale.y);
            }

            Debug.DrawLine(gameObject.transform.position + Vector3.up, gameObject.transform.position + Vector3.up + Vector3.right * transform.localScale.x * minShootRange, Color.cyan);
            enemyMovement.CanMove = false;

            if (timeBtwShotCount <= 0)
            {
                timeBtwShotCount = timeBtwShot;
                Shoot();

            }
            else
            {
                timeBtwShotCount -= Time.deltaTime;
            }
        }
        else
        {
            timeBtwShotCount = timeBtwShot;
        }
       
    }


    void Shoot()
    {
        enemyAnim.SetTrigger("Shoot");
    }

    void HitCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 1), Vector2.right * transform.localScale.x, minShootRange, whatToHit);
        if (hit.collider != null)
        {
            Debug.Log("Player is SHOT!");
            hit.collider.gameObject.GetComponent<SC_PlayerProperties>().Shot(shootDamage);
        }

    }
}
