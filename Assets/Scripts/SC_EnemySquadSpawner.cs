using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemySquadSpawner : MonoBehaviour
{
    public List<GameObject> spawners;
    public float spawnCooldown;
    public float spawnCooldownCount;

    public int enemyCount;
    public int enemyRangedCount;
    public int maxEnemeies;
    public int maxRangedEnemy;

    public int squadListCount;

    public List<int> squadList; //this contain what squad to spawn;

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
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        enemyRangedCount = GameObject.FindObjectsOfType<SC_EnemyRanged>().Length;

        if (squadListCount < squadList.Capacity)
        if (spawnCooldownCount <= 0)
        {
            SpawnSquad();
            Debug.Log("Spawned Squad Type " + squadToSpawn);
        }
        if (spawnCooldownCount > 0 && enemyCount < maxEnemeies)
        {
            spawnCooldownCount -= Time.deltaTime;
        }

    }

    public void SpawnSquad()
    {
        squadToSpawn = squadList[squadListCount];
        int spawnerPoint = Random.Range(0, spawners.Count);
        //spawn everything in EnemyToSpawn
            StartCoroutine(SpawnEnemy(spawnerPoint));
            //nme.transform.parent = null;
            //nme.transform.position = spawners[Random.Range(0, spawners.Count)].transform.position;

        

        spawnCooldownCount = spawnCooldown;
        squadListCount++;

    }

    IEnumerator SpawnEnemy(int spwnPoint)
    {
        foreach (GameObject enemy in squadType[squadToSpawn].enemyToSpawns)
        {
            Instantiate(enemy);
            enemy.transform.parent = null;
            enemy.transform.position = spawners[spwnPoint].transform.position;
            yield return new WaitForSeconds(3);

        }

    }
}


