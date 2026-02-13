using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwap : MonoBehaviour
{
    public int nextScene;
    
    public PlayerInput playerInput;
    private InputAction anyAction;

    private float waitTimer = 0;
    
    void Start()
    {
        anyAction = playerInput.actions.FindAction($"Player/AnyButton");
    }

    void Update()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer < 1f)
            return;
        
        if (anyAction.WasPressedThisFrame())
            SceneManager.LoadScene(nextScene);
    }
}
