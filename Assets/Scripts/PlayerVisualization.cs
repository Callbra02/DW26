using UnityEngine;

public class PlayerVisualization : MonoBehaviour
{
    private bPlayerController playerController;
    //debug

    public GameObject playerVisPrefab;
    private GameObject playerVisualizer;
    public Vector3 spawnOffset = new Vector3(57.6f, 0, 0);
    
    void Start()
    {
        // Get player controller
        playerController = this.GetComponent<bPlayerController>();
        
        // Instantiate player visualizer prefab
        playerVisualizer = Instantiate(playerVisPrefab);
        
        // Set color (eventually swap to sprite)
        playerVisualizer.GetComponent<SpriteRenderer>().sprite = playerController.GetComponent<SpriteRenderer>().sprite;
        //playerVisualizer.GetComponent<SpriteRenderer>().color = playerController.PlayerColor;
    }

    void Update()
    {
        // Set player visualizer to offset position to reflect on display 2
        playerVisualizer.transform.position = playerController.transform.position + spawnOffset;
        playerVisualizer.transform.rotation = playerController.transform.rotation;
    }

}
