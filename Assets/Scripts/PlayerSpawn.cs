using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    [field: SerializeField] public Transform[] SpawnPoints { get; private set; }
    [field: SerializeField] public Color[] PlayerColors { get; private set; }
    public int PlayerCount { get; private set; }

    public int ghostNumber { get; private set; } = 3;

    public Sprite[] playerSprites;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int maxPlayerCount = Mathf.Min(SpawnPoints.Length, PlayerColors.Length);
        if (maxPlayerCount < 1)
        {
            string msg =
                $"You forgot to assign {name}'s {nameof(PlayerSpawn)}.{nameof(SpawnPoints)}" +
                $"and {nameof(PlayerSpawn)}.{nameof(PlayerColors)}!";
            Debug.Log(msg);
        }

        // Prevent adding in more than max number of players
        if (PlayerCount >= maxPlayerCount)
        {
            // Delete new object
            string msg =
                $"Max player count {maxPlayerCount} reached. " +
                $"Destroying newly spawned object {playerInput.gameObject.name}.";
            Debug.Log(msg);
            Destroy(playerInput.gameObject);
            return;
        }

        // Assign spawn transform values
        if (PlayerCount + 1 == ghostNumber)
        {
            playerInput.transform.position = SpawnPoints[5].position;
            playerInput.transform.rotation = SpawnPoints[5].rotation;
        }
        else
        {
            playerInput.transform.position = SpawnPoints[PlayerCount].position;
            playerInput.transform.rotation = SpawnPoints[PlayerCount].rotation;
        }
        
        Color color = PlayerColors[PlayerCount];

        // Increment player count
        PlayerCount++;
        
        // Set gamemanager playercount
        GameManager.Instance.playerCount = PlayerCount;

        // Set up player controller
        bPlayerController playerController = playerInput.gameObject.GetComponent<bPlayerController>();
        playerController.AssignPlayerInputDevice(playerInput);
        playerController.AssignPlayerNumber(PlayerCount);
        playerController.AssignColor(color);
        playerController.GetComponent<SpriteRenderer>().sprite = playerSprites[PlayerCount - 1];
        
        
        // Playercount check\
        if (playerController.PlayerNumber == ghostNumber)
        {
            playerController.isGhost = true;
            GameManager.Instance.ghostController = playerController;
        }
        
        // Add playercontroller to gamemanager list
        GameManager.Instance.PlayerControllers.Add(playerController);
        
        // Create UI widget for players
        if (!playerController.isGhost) 
            PlayerUIController.Instance.CreateWidget();
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        // Not handling anything right now.
        Debug.Log("Player left...");
    }
}
