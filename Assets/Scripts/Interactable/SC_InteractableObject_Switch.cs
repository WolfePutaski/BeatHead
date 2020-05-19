using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_InteractableObject_Switch : MonoBehaviour
{

    public bool isOn;
    public Animator animator;
    public GameObject promptText;
    public AudioClip switchSound;
    public AudioSource audioSource;

    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        promptText.SetActive(false);
    }

    public void PlayerInteract()
    {
        
        if (!isOn)
        {
            animator.SetBool("isOn", true);
            isOn = true;
            audioSource.PlayOneShot(switchSound);
        }
        else
        {
            promptText.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            promptText.SetActive(!isOn);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            promptText.SetActive(false);

        }
    }
}
