using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class SC_PlayerProperties : MonoBehaviour
{

    //public bool isInvincible; //NoHealthReduction
    public bool OnBlock;

    Rigidbody2D playerPhysics;
    SC_CameraController cameraController;
    Animator playerAnim;
    RuntimeAnimatorController defaultAnimator;
    AudioSource audioSource;

    public bool allowInput;

    [Header("Health")]
    public int maxBigHP;
    public int BigHP;
    public float BigHPKillRefillAmount;
    public float BigHPRefillCount;
    public float BigHPDegenRate;
    public float getUpTimerDefault;
    public float downTimer = 0;
    public GameObject getUpBar; 

    public float HP; //execution will gain health or when all enemy is gone;
    public float maxHP;
    public float defaultRegenDelay;
    public float HPRegenRate;
    public float mashCount;
    public bool isDowned;
    float HPregenDelayCount;
    bool onRecovering;

    float timeSinceDying = 0;

    GameObject HPBar;
    public List<GameObject> HPBlock;
    public GameObject getUpText;

    [Header("Posture")]
    public float maxPosture;
    public float posture;
    public float postureRegenRate_default;
    float postureRegenRate;
    float postureRegenDelayCount;

    public GameObject postureBar;

    Color postureBarDefaultColor;

    [Header("Movement")]

    public int dashCountMax;
    public float dashRegenTime;
    public float dashForce;
    public float defaultSpeed = 0;
    public float slowWalkSpeed = 0;
    public float playerSpeed = 0;
    public bool canMove;

    [Header("Interaction")]
    public GameObject interactionObject;
    public LayerMask interactableObjectLayer;

    [System.Serializable]
    public struct PlayerAttackProperties
    {
        public float attackDashForce;
        public float attackPushForce;
    }

    [Header("Attacking")]
    public bool canAttack = true;
    public Transform attackPos;
    public Transform executionPos;
    public float attackDashForce;
    public float attackPushForce;
    public float startTimeBtwAttack;

    public PlayerAttackProperties currentPlayerAttackProperties;

    public PlayerAttackProperties jabAttack;
    public PlayerAttackProperties pushAttack;
    public PlayerAttackProperties kickAttack;

    public float attackRadius;
    public float damage;

    public float attackSpeed;
    public float attackDefaultSpeed;
    public float attackSlowSpeed;

    public float maxStamina;
    public float stamina;
    public float staminaRegenCountdown;
    public bool isTired;
    public GameObject staminaFillBar;
    public Color normalStaminaColor;
    public Color alertStaminaColor;
    public AudioClip tiredSound;

    [Header("Blocking")]
    public bool canBlock;
    public bool isBlocking;

    [Header("Deflection")]
    public ParticleSystem deflectPart;
    public ParticleSystem deflectSuccessPart;
    public float deflectionWindow;
    public float deflectDelay;
    public bool onDeflect;

    [Header("Audio")]
    public List<AudioClip> audioClips;
    public AudioClip radioSound;
    //public float deflectTimer;

    // Start is called before the first frame update
    void Start()
    {
        playerPhysics = GetComponent<Rigidbody2D>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        cameraController = FindObjectOfType<SC_CameraController>();
        audioSource = GetComponent<AudioSource>();
        defaultAnimator = playerAnim.runtimeAnimatorController;
        GameObject deflectP = GameObject.Find("Deflect Part");
        deflectPart = deflectP.GetComponent<ParticleSystem>();

        BigHP = maxBigHP;
        HP = maxHP;
        posture = Mathf.Clamp(maxPosture,0, maxPosture);
        postureRegenRate = postureRegenRate_default;
        stamina = maxStamina;
        //HUD
        postureBar = GameObject.Find("Player_PostureBar");
        postureBarDefaultColor = postureBar.GetComponent<Image>().color;
        HPBar = GameObject.Find("Player_HPBar");
        staminaFillBar = GameObject.Find("Player_StaminaBar");

    
        //HPBlock = new List<GameObject>();
        //HPBlock.AddRange(GameObject.FindGameObjectsWithTag("Player_HPBlock"));

    }

    // Update is called once per frame
    void Update()
    {

        BigHPRegen();
        UpdateHealth();
        PostureRegen();
        StaminaRegen();
        ItemInteraction();
        HUDUpdate();
        UpdateAttackProperties();


    }

    private void LateUpdate()
    {
        ColorUpdate();
    }

    public void ItemInteraction()
    {
        if (!isDowned)
        {
            if (interactionObject != null)
            {
                if (Input.GetKey(KeyCode.E) && !SC_Cheats.isPause)
                {
                    interactionObject.SendMessage("PlayerInteract", SendMessageOptions.DontRequireReceiver);
                    canMove = false;
                    playerAnim.SetFloat("Moving", 0);

                }



            }
        }



    }


    public void Attacked(float damage,float postureDamage, float push) //get attacked
    {
        if (interactionObject != null)
        {
            interactionObject.SendMessage("ResetHold", SendMessageOptions.DontRequireReceiver);

        }
        canMove = false;

        if (isBlocking)
        {
            if (onDeflect) //can deflect
            {
                //Debug.Log("Deflect!");
                //playerAnim.SetTrigger("Deflected");
            }
            else //weak block
            {
                posture -= postureDamage;
                if (posture <= 0)
                {
                    PlaySound("Player_GuardBreak");
                    playerAnim.SetTrigger("Stunned");
                    //posture = maxPosture / 3;
                }
                else
                {
                    playerAnim.SetTrigger("AttackBlocked");
                }
                playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
                playerPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
            }
            

        }
        else //take damage
        {
            PlaySound("ExecutionHit");
            playerAnim.SetTrigger("IsHurt");
            cameraController.Shake();
            playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
            playerPhysics.AddForce(Vector2.right * push, ForceMode2D.Impulse);
            HP -= damage;
            HPregenDelayCount = defaultRegenDelay;

        }
    }

    public void Shot(float damage) //get attacked
    {
        if (interactionObject != null)
        {
            interactionObject.SendMessage("ResetHold", SendMessageOptions.DontRequireReceiver);

        }
        canMove = false;

        {
            PlaySound("Player_Shot");
            playerAnim.SetTrigger("IsHurt");
            cameraController.Shake();
            playerPhysics.velocity = new Vector2(0, playerPhysics.velocity.y);
            HP -= damage;
            HPregenDelayCount = defaultRegenDelay;

        }
    }


    void BigHPRegen()
    {
        if (BigHP < maxBigHP)
        {
            if (BigHPRefillCount < 1 && BigHPRefillCount > 0)
            {
                BigHPRefillCount -= Time.deltaTime * BigHPDegenRate;
            }
            if (BigHPRefillCount >= 1)
            {
                BigHPRefillCount = 0;
                BigHP++;
            }
            if (BigHPRefillCount <= 0)
            {
                BigHPRefillCount = 0;
            }
        }
        else
        {
            BigHPRefillCount = 0;
        }

        
    }

    void BigHPRefillUpdate()
    {
        BigHPRefillCount += BigHPKillRefillAmount;
    }

    void UpdateHealth()
    {
        if (HP < maxHP && HPregenDelayCount <= 0)
        {
            HP += Time.deltaTime * HPRegenRate;
        }

        if (HPregenDelayCount > 0)
        {
            HPregenDelayCount -= Time.deltaTime;
        }

        if (HP <= 0)
        {
            HP = 0.01f;
            BigHP--;
            BigHPRefillCount = 0;
            isDowned = true;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            playerAnim.SetTrigger("WentDown");



        }

        //Downed
        if (isDowned)
        {

            downTimer += Time.deltaTime;

            if (downTimer > getUpTimerDefault && BigHP > 0)
            {
                BigHP = 0;
            }
            
            if (mashCount > 0)
            {
                mashCount -= Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.Space) && BigHP > 0 && !SC_Cheats.isPause)
            {
                mashCount += 1;
            }

            if (mashCount >= 10)
            {
                isDowned = false;
                downTimer = 0;
                onRecovering = true;
                playerAnim.SetTrigger("GetUp");
                HP = maxHP;
                mashCount = 0;
                StartCoroutine(GetUpDelay());
 
            }


        }

        if (onRecovering)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    void StaminaRegen()
    {
        if (staminaRegenCountdown > 0)
        {
            staminaRegenCountdown -= Time.deltaTime;
        }
        if (staminaRegenCountdown <= 0)
        {
            if (stamina < maxStamina)
            {
                stamina = Mathf.Clamp(stamina + Time.deltaTime * 2, 0, maxStamina);
            }



        }

        if (stamina <= 0)
        {
            if (!isTired)
            {
                audioSource.PlayOneShot(tiredSound);
            }

            isTired = true;

        }
        if (stamina >= maxStamina*0.5f)
        {
            isTired = false;
        }


        if (isTired)
        {
            playerAnim.SetFloat("AttackSpeedModifier", attackSlowSpeed);
        }
        else
        {
            playerAnim.SetFloat("AttackSpeedModifier", attackDefaultSpeed);

        }

    }

    IEnumerator GetUpDelay()
    {

        yield return new WaitForSeconds(3);
 
        onRecovering = false;
        gameObject.layer = LayerMask.NameToLayer("Player");

    }

    void PostureRegen()
    {
        if (posture < 0)
        {
            posture = 0;
        }
        if (posture < maxPosture && postureRegenDelayCount <= 0
            && !isBlocking)
        {
            posture += Time.deltaTime * postureRegenRate;
        }

        if (isBlocking)
        {
            postureRegenRate = postureRegenRate_default / 2;
            postureRegenDelayCount = defaultRegenDelay;

        }
        else
        {
            postureRegenRate = postureRegenRate_default;
            postureRegenDelayCount -= Time.deltaTime;
        }
    }
    
    void HUDUpdate()
    {
        //HP
        if (!isDowned)
        {
            getUpBar.GetComponent<Image>().fillAmount = 0;
            float HPPercentage = (HP / maxHP);
            HPBar.GetComponent<Image>().fillAmount = HPPercentage;
            getUpText.SetActive(false);
        }
        if (isDowned)
        {
            getUpBar.GetComponent<Image>().fillAmount = 1 - downTimer / getUpTimerDefault;
            HPBar.GetComponent<Image>().fillAmount = mashCount/10;
            getUpText.SetActive(BigHP > 0);
        }


        //BigHP
        for (int i = 0; i < maxBigHP; i++)
        {
            Image img = HPBlock[i].GetComponent<Image>();

            if (i < BigHP) //active
            {
                img.color = new Color(1f, 1f, 1f);
                img.fillAmount = 1;
            }
            //last one
            if (i == BigHP)
            {
                img.color = new Color(0.6f, 0.6f, 0.6f);
                img.fillAmount = BigHPRefillCount;
            }
            //more than last
            if (i > BigHP)
            {
                img.fillAmount = 0;
            }
        }
        //Posture
        float posturePercentage = (1 - posture / maxPosture);
        postureBar.GetComponent<Image>().fillAmount = posturePercentage;

        if (posture < maxPosture)
        {
            foreach (GameObject a in GameObject.FindGameObjectsWithTag("Player_PostureBar"))
            {
                a.GetComponent<RectTransform>().sizeDelta = new Vector2(100, a.GetComponent<RectTransform>().sizeDelta.y);
            }

            if (posture <= 0)
            {
                postureBar.GetComponent<Image>().color = alertStaminaColor;

            }
            else
            {
                postureBar.GetComponent<Image>().color = postureBarDefaultColor;

            }
        }
        else
        {
            foreach (GameObject a in GameObject.FindGameObjectsWithTag("Player_PostureBar"))
            {
                a.GetComponent<RectTransform>().sizeDelta = new Vector2(0, a.GetComponent<RectTransform>().sizeDelta.y);
            }


        }

        staminaFillBar.GetComponent<Image>().fillAmount = 1 - (stamina / maxStamina);

        if (stamina < maxStamina)
        {
            foreach (GameObject a in GameObject.FindGameObjectsWithTag("Player_StaminaBar"))
            {
                a.GetComponent<RectTransform>().sizeDelta = new Vector2(100, a.GetComponent<RectTransform>().sizeDelta.y);
            }


            if (isTired)
            {
                staminaFillBar.GetComponent<Image>().color = alertStaminaColor;

            }
            else
            {
                staminaFillBar.GetComponent<Image>().color = normalStaminaColor;

            }
        }

        else
        {
            foreach (GameObject a in GameObject.FindGameObjectsWithTag("Player_StaminaBar"))
            {
                a.GetComponent<RectTransform>().sizeDelta = new Vector2(0, a.GetComponent<RectTransform>().sizeDelta.y);
            }
        }


        if (BigHP <= 0)
        {
            GameObject.Find("GAME OVER").GetComponent<TextMeshProUGUI>().enabled = true;
            timeSinceDying += Time.deltaTime;
            if (timeSinceDying > 1)
            {
                if (Input.anyKeyDown)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }

        }
    }

    void ColorUpdate()
    {
        if (onRecovering)
        {
            Color tmp = gameObject.GetComponent<SpriteRenderer>().color;
            tmp.a = .5f;
            gameObject.GetComponent<SpriteRenderer>().color = tmp; //beware, no function to return to default color yet
        }
    }

    void ResetHP()
    {
        HP = maxHP;
    }

    void ResetPosture()
    {
        posture = maxPosture;
    }

    void Return_Camera()
    {
        //cameraController.activeCam = "Main Camera";
        cameraController.isZoom = false;

    }

    void ResetAllAnimTrigger()
    {
        foreach (AnimatorControllerParameter p in playerAnim.parameters)
        {

            if (p.type == AnimatorControllerParameterType.Trigger)
                playerAnim.ResetTrigger(p.name);
        }
    }

    public void DecreaseStamina()
    {
        stamina -= 1;
        staminaRegenCountdown = 1;
    }


    public void ReturnDefaultAnimator()
    {
        playerAnim.runtimeAnimatorController = defaultAnimator;
    }

    public void PlaySound(string audioName)
    {
        audioSource.PlayOneShot(audioClips.Find(x => x.name == audioName));
    }

    public  void  UpdateAttackProperties()
    {
        attackDashForce = currentPlayerAttackProperties.attackDashForce;
        attackPushForce = currentPlayerAttackProperties.attackPushForce;
    }

    public void SetJabAttack()
    {
        currentPlayerAttackProperties = jabAttack;
    }

    public void SetPushAttack()
    {
        currentPlayerAttackProperties = pushAttack;
    }
    public void SetKickAttack()
    {
        currentPlayerAttackProperties = kickAttack;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractableObject"))
        {
            interactionObject = collision.gameObject;
        }
    }


    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractableObject"))
        {
            interactionObject = null;
        }
    }


}
