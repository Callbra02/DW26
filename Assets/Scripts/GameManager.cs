  using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Retrievable playercount
    public int playerCount;
    public bPlayerController ghostController;

    public float gameTimer { get; private set; }
    public float maxGameTime = 300.0f;

    public int artifactCount = 4;
    public int objectivesCompleted = 0;
    public GameObject artifactPrefab;
    public Sprite[] artifactSprites;
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

        
        if (objectivesCompleted == artifactCount)
        {
            HumanWin();
        }
        
        if (gameTimer <= 0)
        {
            GhostWin();
        }
    }

    void SpawnArtifacts()
    {
        List<Transform> availableSpawnPoints = new List<Transform>(artifactSpawnPoints);

        for (int i = 0; i < artifactCount; i++)
        {
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform selectedPoint = availableSpawnPoints[randomIndex];

            GameObject newArtifact = Instantiate(artifactPrefab, selectedPoint.position, selectedPoint.rotation);
            GameObject artifactVis = Instantiate(artifactPrefab, selectedPoint.position, selectedPoint.rotation);
            

            newArtifact.GetComponent<SpriteRenderer>().sprite = artifactSprites[Random.Range(0, artifactSprites.Length)];
            artifactVis.GetComponent<SpriteRenderer>().sprite = newArtifact.GetComponent<SpriteRenderer>().sprite;
            availableSpawnPoints.RemoveAt(randomIndex);
            newArtifact.GetComponent<Artifact>().visualizer = artifactVis;
        }
    }

    
    void GhostWin()
    {
        SceneManager.LoadScene(3);
    }

    void HumanWin()
    {
        SceneManager.LoadScene(4);
    }
}
