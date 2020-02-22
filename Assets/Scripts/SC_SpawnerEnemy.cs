using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SpawnerEnemy : MonoBehaviour
{
    public List<GameObject> spawners;
    public float spawnCooldown;
    public float spawnCooldownCount;
    public int enemyCount;
    public int maxEnemeies;
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        spawners.AddRange(GameObject.FindGameObjectsWithTag("EnemySpawnPoint"));
        spawnCooldownCount = spawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        enemy.transform.parent = null;


        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (spawnCooldownCount <= 0)
        {
            SpawnEnemy();
            Debug.Log("Spawned Enemy");
        }
        if (spawnCooldownCount > 0 && enemyCount < maxEnemeies)
        {
            spawnCooldownCount -= Time.deltaTime;
        }

    }

    public void SpawnEnemy()
    {
        Instantiate(enemy);
        enemy.transform.position = new Vector2(spawners[Random.Range(0, spawners.Count)].transform.position.x, 2);
        spawnCooldownCount = spawnCooldown;

    }
}
