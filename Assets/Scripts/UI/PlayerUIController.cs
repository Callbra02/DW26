using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [field: SerializeField] public GameObject playerUIWidgetPrefab;
    public GameObject PlayerWidgetHolder;
    public List<GameObject> playerUIWidgets {get; private set;} = new List<GameObject>();
    
    void Update()
    {   // If new player joins, create a widget for health and stamina
        if (GameManager.Instance.playerCount > playerUIWidgets.Count)
        {
            CreateWidget();
        }
        
        // Update widget values | e.g. slider values
        UpdateWidgets();
    }

    public void CreateWidget()
    {
        // Instantiate new widget and set parent to widget holder under Display 1 canvas
        GameObject newWidget = Instantiate(playerUIWidgetPrefab);
        newWidget.transform.SetParent(PlayerWidgetHolder.transform);

        // Set position to equidistant values from already existing widgets
        newWidget.transform.position += new Vector3(PlayerWidgetHolder.transform.position.x, 
            PlayerWidgetHolder.transform.position.y + (GameManager.Instance.playerCount - 1) * -170);
        
        // Set widget color to player color
        newWidget.GetComponent<PlayerUIWidget>().SetColor(GameManager.Instance.PlayerControllers[GameManager.Instance.playerCount - 1].PlayerColor);
        
        // Set player number to widget
        newWidget.GetComponent<PlayerUIWidget>().playerNumber = GameManager.Instance.playerCount;
        
        // Pass widget to widgets list
        playerUIWidgets.Add(newWidget);
    }

    void UpdateWidgets()
    {
        // Do not update widgets if none exist
        if (playerUIWidgets.Count == 0)
            return;
        
        // Loop through player widgets
        for (int i = 0; i < playerUIWidgets.Count; i++)
        {
            // Get widget
            PlayerUIWidget playerUIWidget = playerUIWidgets[i].GetComponent<PlayerUIWidget>();
            
            // Get corresponding player controller
            bPlayerController playerController = GameManager.Instance.PlayerControllers[i].GetComponent<bPlayerController>();
            
            // Set widget stamina slider value to normalized value of the players current stamina (in relation to player max stamina)
            playerUIWidget.SetStamina(bUtils.NormalizeSliderValue(playerController.currentStamina, 0, playerController.maxStamina));
            playerUIWidget.SetHealth(bUtils.NormalizeSliderValue(playerController.currentHealth, 0, playerController.maxHealth));
        }
    }
}
