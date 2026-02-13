using UnityEngine;

public class GhostTrap : MonoBehaviour
{
    public Sprite trapSprite;

    public float trapTimer = 0.0f;
    public float trapTimeMax = 15.0f;
    public bool isScareTrap = false;

    private bPlayerController ghostController;

    void Start()
    {
        ghostController = GameManager.Instance.ghostController;
    }
    
    // Update is called once per frame
    void Update()
    {
        trapTimer +=  Time.deltaTime;
        
        if (trapTimer >= trapTimeMax)
            DestroyThis();
    }

    public void DestroyThis()
    {
        ghostController.activeTrapCount--;
        Destroy(this.gameObject);
    }
}
