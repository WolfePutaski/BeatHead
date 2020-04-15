using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum State
{
    Inactive, Active, Broken, Done
}
public class SC_InteractableObject : MonoBehaviour
{
    

    public State state;
    public GameObject textHelp;
    public GameObject InteractionGroup;
    public GameObject holdFill;
    public SC_PlayerObjective playerObjective;

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
        playerObjective = FindObjectOfType<SC_PlayerObjective>();
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
        if (state == State.Inactive || state == State.Broken)
        {
            interactable = true;
        }
        else
        {
            interactable = false;
        }
        if (triggerUse)
        {
            if (state == State.Inactive)
            {
                state = State.Active;
            }
        }

        if (onPlayer)
        {
            if (state == State.Inactive)
            {
                secondsHeldMax = secondsToActive;
            }

            if (state == State.Broken)
            {
                secondsHeldMax = secondsToFix;
            }

            if (interactable)
            {

                if (Input.GetKey(KeyCode.E))
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


            if (triggerUse)
            {
                if(state == State.Inactive)
                {
                    state = State.Active;
                    progressCount = 0;
                }

                if (state == State.Broken)
                {
                    state = State.Active;
                }

                triggerUse = false;

            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                secondsHeld = 0;
            }
        }

    }

    void UpdateHUD()
    {
        var helpText = textHelp.GetComponent<TextMeshPro>();
        holdFill.transform.localScale = new Vector3((secondsHeld / secondsHeldMax), 1,1);
        progressFill.transform.localScale = new Vector3((progressCount / progressMaxCount), 1, 1);

        InteractionGroup.SetActive(secondsHeld > 0);
        progressStateGroup.SetActive(state == State.Active || state == State.Broken);

        brokenIcon.SetActive(state == State.Broken);

        if (interactable)
        {
            if (state == State.Inactive)
            {
                helpText.SetText(toActiveText);
            }

            if (state == State.Broken)
            {
                helpText.SetText(toFixText);
                    progressText.SetActive(!onPlayer);
            }

        }
        else
        {
            helpText.SetText("");
        }

        if (state == State.Active)
        {
            progressText.SetActive(true);
            progressCount += Time.deltaTime;
            progressText.GetComponent<TextMeshPro>().SetText(string.Format("{0}s", Mathf.RoundToInt(progressCount)));

            if (progressCount >= progressMaxCount)
            {
                playerObjective.ObjectiveClear();
                state = State.Done;
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            textHelp.SetActive(interactable);
            collision.GetComponent<SC_PlayerProperties>().interactionObject = this.gameObject;

            onPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        secondsHeld = 0;

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<SC_PlayerProperties>().interactionObject = null;

            onPlayer = false;

            textHelp.SetActive(false);

        }
    }

}
