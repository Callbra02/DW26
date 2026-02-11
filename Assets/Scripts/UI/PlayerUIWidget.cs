using UnityEngine;
using UnityEngine.UI;

public class PlayerUIWidget : MonoBehaviour
{
    public Slider healthSlider;
    public Slider staminaSlider;

    public int playerNumber;
    
    void Start()
    {
        // Set slider values to max
        healthSlider.value = healthSlider.maxValue;
        staminaSlider.value = staminaSlider.maxValue;
    }

    public void SetColor(Color color)
    {
        // This is nasty lmao (edited to be slighter better, still nasty)
        // Get slider from gameobject children, set color of Image on health fill object
        healthSlider.gameObject.transform.GetChild(1).GetComponentInChildren<Image>().color = color;
    }
    
    // Set health to given value
    public void SetHealth(float value)
    {
        healthSlider.value = value;
    }

    // Set stamina to given value
    public void SetStamina(float value)
    {
        staminaSlider.value = value;
    }
}
