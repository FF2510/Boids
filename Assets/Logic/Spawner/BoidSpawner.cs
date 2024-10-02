using System.Collections;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public GameObject Boid;
    public int Count = 100;
    public float RandomDelay = 0.5f;


    void Start()
    {
        for (int i = 0; i < Count; i++)
        {
            StartCoroutine(Delay());
        }
    }

    private IEnumerator Delay()
    {
        float time = Random.Range(0, RandomDelay);
        
        yield return new WaitForSeconds(time);

        Instantiate(Boid);
    }
}
