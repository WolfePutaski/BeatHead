using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyMovement : MonoBehaviour
{
    Animator enemyAnim;
    SC_EnemyProperties enemyProperties;

    [Header("Movement")]
    public bool CanMove = true;
    public bool IsEngaging;
    public float DefaultSpeed;
    public float speedVariable;
    public float farDistanceMin;
    public float farDistanceMax;
    private float farDistance;
    public float closeDistance;
    public float distanceFromTarget;

    [Header("RandomFreeze")]
    public float FreezeDistance; //Minimum distance to freeze
    public float FreezeTimer;
    public float StayFreezeMin;
    public float StayFreezeMax;
    public float FreezeRate;
    public float FreezeRateMax;
    public float FreezeRateMin;
    public bool isFreezingMove;

    [Header("Attacking")]

    Rigidbody2D enemyPhysics;
    public GameObject Target;
    [HideInInspector]public float defaultScaleX;

    // Start is called before the first frame update
    void Start()
    {
        enemyProperties = gameObject.GetComponent<SC_EnemyProperties>();
        farDistance = Random.Range(farDistanceMin, farDistanceMax);
        Target = GameObject.Find("Player");
        enemyPhysics = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        defaultScaleX = transform.localScale.x;
        DefaultSpeed = Random.Range(DefaultSpeed - speedVariable, DefaultSpeed + speedVariable);
        FreezeRate = Random.Range(FreezeRateMin, FreezeRateMax);
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromTarget = transform.position.x - Target.transform.position.x;

        if (GameObject.Find("Player"))
        {
            RequestAttack();
        }

        if (CanMove)
        {


            if (!IsEngaging)
            {
                FollowPlayer();
            }

            if (!isFreezingMove)
            {

            }
            if (IsEngaging)
            {
                MoveToAttack();
            }
        }

        FreezingMovement();

    }

    public void AllowMoving()
    {
        CanMove = true;
    }

    public void NotAllowMoving()
    {
        CanMove = false;
    }

    //Freezing Movement
    public void FreezingMovement()
    {
        if (Mathf.Abs(distanceFromTarget) < FreezeDistance && FreezeRate > 0)
        {
            FreezeRate -= Time.deltaTime;
        }

        if (FreezeRate < 0)
        {
            FreezeRate = 0;
            FreezeTimer = Random.Range(StayFreezeMin, StayFreezeMax);
        }

        if (FreezeTimer > 0)
        {
            FreezeTimer -= Time.deltaTime;
        }

        if (FreezeTimer < 0)
        {
            FreezeTimer = 0;
            FreezeRate = Random.Range(FreezeRateMin, FreezeRateMax);
        }

        //if (enemyProperties.HP == enemyProperties.defaultHP)
        {
            isFreezingMove = (FreezeTimer > 0);

        }
        //else
        //{
        //    isFreezingMove = false;
        //}

    }
    //Attack Requesting
    public void RequestAttack()
    {
        Target.SendMessage("GetAttackRequest", gameObject);
        //Debug.Log("Attack Requested");

    }
    public void AllowtoAttack()
    {
        IsEngaging = true;
        //Debug.Log("Attack Allowed");

    }

    public void CancelAttack()
    {
        Target.SendMessage("CancelAttacker", gameObject);
        IsEngaging = false;
    }

    public void FollowPlayer()
    {
        float btwFarDistance = Random.Range(farDistanceMin, farDistanceMax);

        if (distanceFromTarget < 0)
        {
            transform.localScale = new Vector2(defaultScaleX, transform.localScale.y);
        }
        if (distanceFromTarget > 0)
        {
            transform.localScale = new Vector2(-defaultScaleX, transform.localScale.y);
        }

        if (!isFreezingMove)
        {
            if (Mathf.Abs(distanceFromTarget) > farDistance + btwFarDistance) //walk towards
            {
                enemyPhysics.velocity = new Vector2(transform.localScale.normalized.x * DefaultSpeed, enemyPhysics.velocity.y);
                enemyAnim.SetFloat("MoveDir", 1);
            }
            if (Mathf.Abs(distanceFromTarget) < farDistance - btwFarDistance) //walk back
            {
                enemyPhysics.velocity = new Vector2(-transform.localScale.normalized.x * DefaultSpeed, enemyPhysics.velocity.y);
                enemyAnim.SetFloat("MoveDir", -1);
            }
        }
        else
        {
            enemyPhysics.velocity = new Vector2(0, enemyPhysics.velocity.y);
            enemyAnim.SetFloat("MoveDir", 0);

        }


        if (enemyPhysics.velocity.x == 0)
        {
            enemyAnim.SetFloat("MoveDir", 0);
        }

    }

    public void MoveToAttack()
    {

        if (distanceFromTarget < 0)
        {
            transform.localScale = new Vector2(defaultScaleX, transform.localScale.y);
        }
        if (distanceFromTarget > 0)
        {
            transform.localScale = new Vector2(-defaultScaleX, transform.localScale.y);
        }

        if (!isFreezingMove)
        {


            if (Mathf.Abs(distanceFromTarget) > closeDistance)
            {
                enemyPhysics.velocity = new Vector2(transform.localScale.normalized.x * DefaultSpeed, enemyPhysics.velocity.y);
                enemyAnim.SetFloat("MoveDir", 1);

            }
            if (Mathf.Abs(distanceFromTarget) < closeDistance - 0.2f)
            {
                enemyPhysics.velocity = new Vector2(-transform.localScale.normalized.x * DefaultSpeed, enemyPhysics.velocity.y);
                enemyAnim.SetFloat("MoveDir", -1);

            }
        }
        else
        {
            enemyPhysics.velocity = new Vector2(0, enemyPhysics.velocity.y);
            enemyAnim.SetFloat("MoveDir", 0);

        }

        if (enemyPhysics.velocity.x == 0)
        {
            enemyAnim.SetFloat("MoveDir", 0);
        }
    }
}