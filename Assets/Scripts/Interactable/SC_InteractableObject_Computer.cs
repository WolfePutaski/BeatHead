using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SC_InteractableObject_Computer : MonoBehaviour
{
    public enum CompueterState
    {
        Inactive, Active, Broken, Done
    }

    public Animator objAnim;
    public CompueterState state;
    public GameObject textHelp;
    public GameObject InteractionGroup;
    public GameObject holdFill;
    public bool isObjectiveActive;

    public string toActiveText;
    public float secondsToActive;
    public string toFixText;
    public float secondsToFix;
    float secondsHeldMax = 99;
    [HideInInspector] public float secondsHeld = 0;
    bool triggerUsed = false;
    public bool interactable;
    bool onPlayer = false;

    public bool canBeBroken;
    public float maxTimeToBeBroken;
    public float TimeToBeBroken;
    public GameObject enemy;
    AudioSource audioSource;
    public AudioClip alert;
    public bool soundOn;
    public bool toBeBroken;

    public Renderer otherEntrance;

    [Header("Progress")]
    public GameObject progressStateGroup;
    public GameObject progressFill;
    public float progressMaxCount;
    public float progressCount;
    public GameObject progressText;
    public GameObject brokenIcon;

    // Start is called before the first frame update
    void Start()
    {
        objAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        TimeToBeBroken = maxTimeToBeBroken;
    }

    // Update is called once per frame
    void Update()
    {
        CheckUse();
        UpdateHUD();
        WaitToBeBroken();

        if (otherEntrance != null)
        {
            OtherEntranceVisible();
        }
    }

    void OtherEntranceVisible()
    {
        if (otherEntrance.isVisible)
        {
            if (state == CompueterState.Active)
            {
                {
                    if (TimeToBeBroken <= 0 && state == CompueterState.Broken)
                    {
                        Instantiate(enemy, gameObject.transform.position, Quaternion.identity);

                    }
                    toBeBroken = false;
                    TimeToBeBroken = maxTimeToBeBroken;
                }


            }
        }
    }

    void WaitToBeBroken()
    {
        if (state == CompueterState.Active)
        {
            audioSource.Stop();
            if (toBeBroken)
            {
                if (otherEntrance != null)
                {
                    if (!otherEntrance.isVisible)
                    {
                        TimeToBeBroken -= Time.deltaTime;
                    }
                }
                else
                {
                    TimeToBeBroken -= Time.deltaTime;
                }

                if (TimeToBeBroken <= 0)
                {
                    Debug.Log("Computer is Broken!");
                    state = CompueterState.Broken;
                    if (soundOn)
                    {
                        audioSource.clip = alert;
                        audioSource.Play();
                    }
                }
            }
        }

    }
    void CheckUse()
    {
        if (isObjectiveActive)
        {
            gameObject.tag = "InteractableObject";
            if (state == CompueterState.Inactive || state == CompueterState.Broken)
            {
                interactable = true;
            }
            else
            {
                interactable = false;
            }
        }
        else
        {
            gameObject.tag = "Untagged";
            interactable = false; 
        }





        if (onPlayer)
        {
            if (state == CompueterState.Inactive)
            {
                secondsHeldMax = secondsToActive;
            }

            if (state == CompueterState.Broken)
            {
                secondsHeldMax = secondsToFix;
            }

            if (interactable)
            {

                if (Input.GetKey(KeyCode.E))
                {
                    //if (secondsHeld < secondsHeldMax)
                    //{
                    //    secondsHeld += Time.deltaTime;
                    //}
                    //if (secondsHeld >= secondsHeldMax)
                    //{
                    //    triggerUse = true;

                    //    secondsHeld = 0;
                    //}
                }
            }


            if (triggerUsed)
            {
                if(state == CompueterState.Inactive)
                {
                    state = CompueterState.Active;
                    progressCount = 0;

                }

                if (state == CompueterState.Broken)
                {
                    state = CompueterState.Active;
                    TimeToBeBroken = maxTimeToBeBroken;
                }

                triggerUsed = false;

            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                ResetHold();
            }
        }

    }

    void UpdateHUD()
    {
        objAnim.SetBool("isOn", state == CompueterState.Active || state == CompueterState.Broken);


        var helpText = textHelp.GetComponent<TextMeshPro>();
        holdFill.transform.localScale = new Vector3((secondsHeld / secondsHeldMax), 1,1);
        progressFill.transform.localScale = new Vector3((progressCount / progressMaxCount), 1, 1);

        InteractionGroup.SetActive(secondsHeld > 0);
        progressStateGroup.SetActive(state == CompueterState.Active || state == CompueterState.Broken);

        brokenIcon.SetActive(state == CompueterState.Broken);

        if (interactable)
        {
            if (state == CompueterState.Inactive)
            {
                helpText.SetText(toActiveText);
            }

            if (state == CompueterState.Broken)
            {
                helpText.SetText(toFixText);
                    progressText.SetActive(!onPlayer);
                progressFill.GetComponent<SpriteRenderer>().color = Color.red;


            }

        }
        else
        {
            helpText.SetText("");
            progressFill.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        if (state == CompueterState.Active)
        {
            secondsHeld = 0;
            progressText.SetActive(true);
            progressCount += Time.deltaTime;
            progressText.GetComponent<TextMeshPro>().SetText(string.Format("{0}s", Mathf.RoundToInt(progressCount)));

            if (progressCount >= progressMaxCount)
            {
                //playerObjective.ObjectiveClear();
                state = CompueterState.Done;
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            textHelp.SetActive(interactable);
            onPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (interactable)
        {
            secondsHeld = 0;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            onPlayer = false;

            textHelp.SetActive(false);

        }
    }

    public void PlayerInteract()
    {
        if (interactable)
        {
            if (secondsHeld < secondsHeldMax)
            {
                secondsHeld += Time.deltaTime;
            }
            if (secondsHeld >= secondsHeldMax)
            {
                triggerUsed = true;

                secondsHeld = 0;
            }

        }

    }

    public void ResetHold()
    {
        StartCoroutine("ResetHold2");
    }

    private IEnumerator ResetHold2()
    {
        secondsHeld = -1f;
        yield return new WaitForSeconds(.3f);
        interactable = true;
        secondsHeld = 0;

    }

    //AutoBrokenPC
    private void OnBecameInvisible()
    {
        if (state == CompueterState.Active)
        {
            {
                toBeBroken = true;
            }
        }


    }

    private void OnBecameVisible()
    {

                if (TimeToBeBroken <= 0 && state == CompueterState.Broken)
                {
                    Instantiate(enemy, gameObject.transform.position, Quaternion.identity);

                }
                toBeBroken = false;
                TimeToBeBroken = maxTimeToBeBroken;
            
    }

}
