using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyProperties : MonoBehaviour
{
    [Header("Health")]
    public float defaultHP;
    [SerializeField]
    float HP;
    public int defaultDHP;
    int DHP;

    public GameObject HPBar;
    float defaultHPBarLength;
    public List<GameObject> DHPBlock;
    
    public float regenDelay;
    float regenDelayCount = 0;
    float regenRateCount = 0;
    public float regenPerSec;

    [Header("Posture")]
    public float defaultPosture;
    [SerializeField]
    float posture;
    public float stunKnockback;
    Transform statusGroup;

    public GameObject postureBar;
    float defaultpostureBarLength;

    [SerializeField]
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
        DHP = defaultDHP;




        ////HUD
        //HPBar = gameObject.transform.Find("HPBar").gameObject;
        //postureBar = gameObject.transform.Find("PostureBar").gameObject;

        DHPBlock = new List<GameObject>();

        foreach (Transform child in gameObject.transform.Find("Status Group").Find("HealthBlockGroup").gameObject.transform)
        {
            if (child.tag == "Enemy_HPBLock")
            {
                DHPBlock.Add(child.gameObject);
            }
        }

        defaultHPBarLength = HPBar.transform.localScale.x;
        defaultpostureBarLength = postureBar.transform.localScale.x;
        //DHPBlock = Add.
            //add each block to maximum HP
            //GameObject.FindGameObjectsWithTag("Enemy_HPBLock");
    }

    // Update is called once per frame
    void Update()
    {
        HUDUpdate();

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

        if (HP <= 0)
        {
            enemyAnim.SetTrigger("Die");
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            Destroy(gameObject, 1);
        }

        if (posture > 0)
        {
            harderned = true;
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
            posture = 0;
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

    void HUDUpdate()
    {
        if (gameObject.transform.localScale.x < 0)
        {
            gameObject.transform.Find("Status Group").gameObject.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            gameObject.transform.Find("Status Group").gameObject.transform.localScale = new Vector2(1, 1);
        }
        //PostureBar

        HPBar.transform.localScale = new Vector2(defaultHPBarLength * HP / defaultHP, HPBar.transform.localScale.y);

        //PostureBar

        if (postureBar.activeSelf == true)
        {
            if (defaultPosture <= 0)
            {
                postureBar.transform.localScale = Vector2.zero;
            }
            else
            {
                postureBar.transform.localScale = new Vector2(defaultpostureBarLength * posture / defaultPosture, postureBar.transform.localScale.y);
            }
        }

        //Death Blow HP
        for (int i = 1; i <= DHPBlock.Count; i++)
        {
            if (i > DHP)
            {
                DHPBlock[i -1].SetActive(false);
            }
            else
            {
                DHPBlock[i -1].SetActive(true);
            }
        }

    }

    void FindHUD()
    {

    }
}
