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
        float highX = 0;
        float lowX = 0;
        float highY = 0;
        float lowY = 0;
        
        for (int i = 0; i < players.Count; i++)
        {
            if (i == 0)
            {
                highX = players[i].transform.position.x;
                lowX = players[i].transform.position.x;
                highY = players[i].transform.position.y;
                lowY = players[i].transform.position.y;
            }

            if (players[i].transform.position.x > highX)
            {
                highX = players[i].transform.position.x;
            }
            else if (players[i].transform.position.x < lowX)
            {
                lowX = players[i].transform.position.x;
            }

            if (players[i].transform.position.y > highY)
            {
                highY = players[i].transform.position.y;
            }
            else if (players[i].transform.position.y < lowY)
            {
                lowY = players[i].transform.position.y;
            }
        }

        Vector3 m = new Vector3((highX + lowX) / 2, (highY + lowY) / 2, -10);
        
        leftCam.transform.position = m;
    }
}
