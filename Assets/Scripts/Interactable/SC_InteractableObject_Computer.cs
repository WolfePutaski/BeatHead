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
    float secondsHeld = 0;
    bool triggerUse = false;
    public bool interactable;
    bool onPlayer = false;

    [Header("Progress")]
    public GameObject progressStateGroup;
    public GameObject progressFill;
    public float progressMaxCount;
    float progressCount;
    public GameObject progressText;
    public GameObject brokenIcon;

    // Start is called before the first frame update
    void Start()
    {
        objAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckUse();
        UpdateProgression();
        UpdateHUD();
    }

    void UpdateProgression()
    {

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




        if (triggerUse)
        {
            if (state == CompueterState.Inactive)
            {
                state = CompueterState.Active;
            }
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


            if (triggerUse)
            {
                if(state == CompueterState.Inactive)
                {
                    state = CompueterState.Active;
                    progressCount = 0;
                }

                if (state == CompueterState.Broken)
                {
                    state = CompueterState.Active;
                }

                triggerUse = false;

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

        secondsHeld = 0;

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
                triggerUse = true;

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
        secondsHeld = -1;
        yield return new WaitForSeconds(1);
        interactable = true;
    }

}
