using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player.SetActive(false);
        StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitUntil(() => MapGeneration.generated);
        player.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
