using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Door : MonoBehaviour
{
    public GameObject exitDoor;
    public GameObject teleportPos;
    LineRenderer lineRenderer;
    public bool isActive;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, exitDoor.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
            lineRenderer.enabled = isActive;

        if (isActive && Input.GetKeyDown(KeyCode.Space))
        {
            Teleport(player);
        }
    }

    void Teleport(GameObject obj)
    {
        obj.transform.position = exitDoor.GetComponent<SC_Door>().teleportPos.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isActive = true;
            player = collision.gameObject;

        }
        //Teleport(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isActive = false;
        }
        player = null;
    }

}
