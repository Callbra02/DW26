using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Retrievable playercount
    public int playerCount;

    public float gameTimer { get; private set; }
    public float maxGameTime = 300.0f;

    public int artifactCount = 4;
    public GameObject artifactPrefab;
    public List<GameObject> artifacts = new List<GameObject>();
    public GameObject artifactSpawnPointHolder;
    private Transform[] artifactSpawnPoints;

    // Retrievable player controller list
    public List<bPlayerController> PlayerControllers;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        
        gameTimer = maxGameTime;

        // Create spawn point array
        artifactSpawnPoints = new Transform[artifactSpawnPointHolder.transform.childCount];
        
        // Populate artifact spawn points 
        for (int i = 0; i < artifactSpawnPointHolder.transform.childCount; i++)
        {
            artifactSpawnPoints[i] = artifactSpawnPointHolder.transform.GetChild(i);
        }
        
        SpawnArtifacts();
    }

    void Update()
    {
        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0)
        {
            Debug.Log("END GAME");
        }
    }

    void SpawnArtifacts()
    {
        // 
        List<Transform> availableSpawnPoints = new List<Transform>(artifactSpawnPoints);

        for (int i = 0; i < artifactCount; i++)
        {
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform selectedPoint = availableSpawnPoints[randomIndex];

            Instantiate(artifactPrefab, selectedPoint.position, selectedPoint.rotation);

            availableSpawnPoints.RemoveAt(randomIndex);
        }
    }
}
