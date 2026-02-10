using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Camera leftCam;
    public Camera rightCam;

    public Transform leftCameraTarget;

    public List<GameObject> players;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > players.Count)
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
        }
        
        UpdateCameraPosition(players);
    }

    void UpdateCameraPosition(List<GameObject> players)
    {
        // Vars for high and low coords
        float highX = 0;
        float lowX = 0;
        float highY = 0;
        float lowY = 0;
        
        // Loop through players
        for (int currentPlayerIndex = 0; currentPlayerIndex < players.Count; currentPlayerIndex++)
        {
            var currentPlayer = players[currentPlayerIndex];
            
            // Assign first players values
            if (currentPlayerIndex == 0)
            {
                highX = currentPlayer.transform.position.x;
                lowX = currentPlayer.transform.position.x;
                highY = currentPlayer.transform.position.y;
                lowY = currentPlayer.transform.position.y;
            }
            
            if (currentPlayer.transform.position.x > highX)
            {
                highX = currentPlayer.transform.position.x;
            }
            else if (currentPlayer.transform.position.x < lowX)
            {
                lowX = currentPlayer.transform.position.x;
            }

            if (currentPlayer.transform.position.y > highY)
            {
                highY = currentPlayer.transform.position.y;
            }
            else if (currentPlayer.transform.position.y < lowY)
            {
                lowY = currentPlayer.transform.position.y;
            }
        }

        // Get midpoint between furthest players
        Vector3 m = new Vector3((highX + lowX) / 2, (highY + lowY) / 2, -10);
        
        // Update camera position
        leftCam.transform.position = m;
    }
}
