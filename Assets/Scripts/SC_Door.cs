using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SC_Door : MonoBehaviour
{
    public GameObject exitDoor;
    public GameObject teleportPos;
    LineRenderer lineRenderer;
    public bool isActive;
    GameObject player;

    public List<GameObject> enemyEntryList;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, teleportPos.transform.position);
        lineRenderer.SetPosition(1, exitDoor.GetComponent<SC_Door>().teleportPos.transform.position);

        if (enemyEntryList[0] == null)
        {
            enemyEntryList[0] = gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
            lineRenderer.enabled = isActive;

        if (isActive && Input.GetKeyDown(KeyCode.E) && !SC_Cheats.isPause)
        {
            if (!FindObjectOfType<SC_PlayerProperties>().isDowned)
            {
                Teleport(player);
            }
        }
    }

    void Teleport(GameObject obj)
    {
        StartCoroutine(TeleportWait(obj));
    }

    public IEnumerator TeleportWait(GameObject obj)
    {
        obj.GetComponent<Collider2D>().enabled = false;
        obj.GetComponent<Rigidbody2D>().constraints  = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        obj.GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(0.5f);
        obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        obj.GetComponent<Collider2D>().enabled = true;
        obj.GetComponent<SpriteRenderer>().enabled = true;
        obj.transform.position = exitDoor.transform.position;
        Debug.Log("Player Teleported at " + gameObject.name + " to " + exitDoor.name);

        
        if (FindObjectOfType<SC_EnemySquadSpawner>() != null)
        {
            var spawner = FindObjectOfType<SC_EnemySquadSpawner>();
            spawner.pooledEnemies.Clear();
            foreach (GameObject nme in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                spawner.pooledEnemies.Add(nme);
                //nme.SetActive(false);

            }
            foreach (GameObject nme in GameObject.FindGameObjectsWithTag("Enemy_Boss"))
            {
                spawner.pooledEnemies.Add(nme);
                //nme.SetActive(false);

            }
            spawner.GetNewDoorSpawner(exitDoor.GetComponent<SC_Door>());
        }
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
            player = null;
        }
    }

}
