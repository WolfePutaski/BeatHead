using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemySquadSpawner : MonoBehaviour
{
    public GameObject spawnerObj;
    public List<GameObject> spawners;
    public float spawnCooldown;
    public float spawnCooldownCount;

    public int enemyOnScreenCount;
    public int activeEnemyCount;
    public int maxEnemies;
    public int squadListCount;

    public List<int> squadList; //this contain what squad to spawn;
    public List<GameObject> pooledEnemies;
    public bool spawningPooledEnemy;

    public bool unlimitedSpawn;
    public bool allowNewEnemy;
    public int countingPooledEnemySpawned;


    [System.Serializable]
    public class EnemySquad
    {
        public List<GameObject> enemyToSpawns;
    }

    public EnemySquad[] squadType;
    public int squadToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        spawners.AddRange(GameObject.FindGameObjectsWithTag("EnemySpawnPoint"));
        spawnCooldownCount = spawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        enemyOnScreenCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        activeEnemyCount = enemyOnScreenCount /*+ pooledEnemies.Count*/;

        spawningPooledEnemy = pooledEnemies.Count > 0;

        if (!spawningPooledEnemy)
        {
            if (allowNewEnemy)
            {
                if (unlimitedSpawn)
                {
                    if (spawnCooldownCount <= 0)
                    {
                        spawnCooldownCount = spawnCooldown;
                        int squadTypeToSpawn = Random.Range(0, squadType.Length);

                        StartCoroutine(SpawnRandomSquad(squadTypeToSpawn));
                        Debug.Log("Spawned Random Squad Type " + squadTypeToSpawn);
                    }
                }
                else
                {
                    if (squadListCount < squadList.Capacity)
                        if (spawnCooldownCount <= 0)
                        {
                            SpawnSquad();
                            Debug.Log("Spawned Squad Type " + squadToSpawn);
                        }
                }
            }



        }

        if (spawningPooledEnemy)
        {
            if (spawnCooldownCount <= 0)
            {
                SpawnPooledEnemy();
                Debug.Log("Spawned Squad Type " + squadToSpawn);
            }

            //foreach (GameObject nme in pooledEnemies)
            //{
            //    if (nme.GetComponent<SC_EnemyProperties>().isOnScreen)
            //    {
            //        pooledEnemies.Remove(nme);
            //    }
            //}
        }

        spawners.RemoveAll(item => item == null);

        if (spawnCooldownCount > 0 && activeEnemyCount < maxEnemies) // stop cooldown when max enemy reached
        {
            spawnCooldownCount -= Time.deltaTime;
        }

    }

    public void SpawnSquad()
    {
        if (!unlimitedSpawn)
        {
            squadToSpawn = squadList[squadListCount];
        }
        else
        {
            squadToSpawn = squadList[Random.Range(0, squadList.Count)];
        }
        

        int spawnerPoint = Random.Range(0, spawners.Count);
        //spawn everything in EnemyToSpawn
            StartCoroutine(SpawnSquadEnemy(spawnerPoint));
            //nme.transform.parent = null;
            //nme.transform.position = spawners[Random.Range(0, spawners.Count)].transform.position;

        

        spawnCooldownCount = spawnCooldown;

        if (!unlimitedSpawn)
        {
            squadListCount++;
        }

    }

    public void SpawnPooledEnemy() //spawn the random enemy in the pool
    {
        int spawnerPoint = Random.Range(0, spawners.Count);

        if (countingPooledEnemySpawned > 0)
        {
            spawnerPoint = 0;
        }

        int randomNum = Random.Range(0, pooledEnemies.Count);
        GameObject enemy = pooledEnemies[randomNum];

        //enemy.SetActive(true);
        //enemy.GetComponent<Animator>().speed = 1;
        if (!enemy.GetComponent<SC_EnemyProperties>().isOnScreen)
        {
            enemy.transform.position = spawners[spawnerPoint].GetComponentInChildren<Transform>().position;
            countingPooledEnemySpawned--;
        }

        pooledEnemies.Remove(enemy);

        spawnCooldownCount = spawnCooldown;

    }

    public void GetNewDoorSpawner(SC_Door newDoor)
    {
        countingPooledEnemySpawned = maxEnemies;

        spawnCooldownCount = 1;

        int initialSpanwersCount = spawners.Count;
        int spwanersToCreate = newDoor.enemyEntryList.Count - initialSpanwersCount;

        if (spwanersToCreate > 0) //create new entry if count is lower
        {
            for (int i = 0; i < spwanersToCreate; i++)
            {
                GameObject newEntry = Instantiate(spawnerObj);
                spawners.Add(newEntry);
            }
        }
        else
        {
            spwanersToCreate = 0;
        }



        for (int i = 0; i < initialSpanwersCount + spwanersToCreate; i++) //set active or inaactive
        {
            if (i < newDoor.enemyEntryList.Count)
            {
                spawners[i].gameObject.transform.position = newDoor.enemyEntryList[i].transform.position;
            }
            else
            {
                Destroy(spawners[i]);
                //spawners.RemoveAt(i);
            }
            


        }




    }

    IEnumerator SpawnSquadEnemy(int spawnerPoint)
    {
        if (!spawningPooledEnemy)
        {
            foreach (GameObject enemy in squadType[squadToSpawn].enemyToSpawns)
            {
                Instantiate(enemy);
                enemy.transform.parent = null;
                enemy.transform.position = spawners[spawnerPoint].GetComponentInChildren<Transform>().position;
                yield return new WaitForSeconds(spawnCooldown);

            }
        }


    }

    IEnumerator SpawnRandomSquad(int squadTypeToSpawn)
    {

        if (!spawningPooledEnemy && activeEnemyCount < maxEnemies - squadType[squadTypeToSpawn].enemyToSpawns.Count)
        {
            foreach (GameObject enemy in squadType[squadTypeToSpawn].enemyToSpawns)
            {

                if (activeEnemyCount < maxEnemies - squadType[squadTypeToSpawn].enemyToSpawns.Count)
                {
                    int spawnerPoint = Random.Range(0, spawners.Count);

                    Debug.Log("Spawn " + enemy.name + " at " + spawners[spawnerPoint].name);
                    Instantiate(enemy, spawners[spawnerPoint].GetComponentInChildren<Transform>().position,Quaternion.identity);
                    enemy.transform.parent = null;
                    //enemy.transform.position = spawners[spawnerPoint].GetComponentInChildren<Transform>().position;

                }

                yield return new WaitForSeconds(spawnCooldown);
 

            }
        }
    }

}


