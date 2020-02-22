using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyMovement : MonoBehaviour
{
    Animator enemyAnim;

    [Header("Movement")]
    public bool CanMove = true;
    public bool IsEngaging;
    public float DefaultSpeed;
    public float farDistanceMin;
    public float farDistanceMax;
    private float farDistance;
    public float closeDistance;
    public float distanceFromTarget;

    [Header("Attacking")]




    Rigidbody2D enemyPhysics;
    public GameObject Target;
    private float defaultScaleX;

    // Start is called before the first frame update
    void Start()
    {
        farDistance = Random.Range(farDistanceMin, farDistanceMax);
        Target = GameObject.Find("Player");
        enemyPhysics = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        defaultScaleX = transform.localScale.x;
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

            if (IsEngaging)
            {
                MoveToAttack();
            }
        }

    }

    public void AllowMoving()
    {
        CanMove = true;
    }

    public void NotAllowMoving()
    {
        CanMove = false;
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
        IsEngaging = false;
        Target.SendMessage("CancelAttacker", gameObject);
    }

    public void FollowPlayer()
    {
        if (distanceFromTarget < 0)
        {
            transform.localScale = new Vector2(defaultScaleX, transform.localScale.y);
        }
        if (distanceFromTarget > 0)
        {
            transform.localScale = new Vector2(-defaultScaleX, transform.localScale.y);
        }

        if (Mathf.Abs(distanceFromTarget) > farDistanceMax) //walk towards
        {
            enemyPhysics.velocity = new Vector2(transform.localScale.normalized.x * DefaultSpeed, enemyPhysics.velocity.y);
            enemyAnim.SetFloat("MoveDir", 1);
        }
        if (Mathf.Abs(distanceFromTarget) < farDistanceMin) //walk back
        {
            enemyPhysics.velocity = new Vector2(-transform.localScale.normalized.x * DefaultSpeed, enemyPhysics.velocity.y);
            enemyAnim.SetFloat("MoveDir", -1);
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
        if (enemyPhysics.velocity.x == 0)
        {
            enemyAnim.SetFloat("MoveDir", 0);
        }
    }
}