using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject swarmerPrefab;
    [SerializeField]
    private GameObject swarmer2Prefab;

    [SerializeField]
    private float swarmerMinInterval = 3;
    [SerializeField]
    private float swarmerMaxInterval = 3;




    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy(swarmerMinInterval, swarmerMaxInterval, swarmerPrefab));
        StartCoroutine(SpawnEnemy(swarmerMinInterval, swarmerMaxInterval, swarmer2Prefab));
    }

    // Update is called once per frame
    private IEnumerator SpawnEnemy(float minInterval, float maxInterval, GameObject enemy)
    {

        yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        GameObject newEnemy = Instantiate(enemy, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        StartCoroutine(SpawnEnemy(minInterval, maxInterval, enemy));
    }

    private IEnumerator SpawnEnemy2(float minInterval, float maxInterval, GameObject enemy)
    {

        yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        GameObject newEnemy = Instantiate(enemy, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        StartCoroutine(SpawnEnemy2(minInterval, maxInterval, enemy));
    }
}
