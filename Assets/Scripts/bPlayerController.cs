using UnityEngine;
using UnityEngine.InputSystem;

public class bPlayerController : MonoBehaviour
{
    [field: SerializeField] public int PlayerNumber { get; private set; }
    [field: SerializeField] public Color PlayerColor { get; private set; }
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    [field: SerializeField] public Rigidbody2D Rigidbody2D { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; } = 10f;
    [field: SerializeField] public float DashSpeed { get; private set; } = 15f;
    [field: SerializeField] public float deccelSpeed { get; private set; } = 2.5f;
    
    public bool isGhost = false;
    private bool canDash = true;
    private bool isDashing = false;

    private float CurrentSpeed;
    public bool DoDash { get; private set; }

    // Player input information
    private PlayerInput PlayerInput;
    private InputAction InputActionMove;
    private InputAction InputActionInteract;

    // Assign color value on spawn from main spawner
    public void AssignColor(Color color)
    {
        // record color / Set ghost to white
        if (isGhost)
        {
            PlayerColor = Color.white;
        }
        else
        {
            PlayerColor = color;
        }
        
        // Assign to sprite renderer
        if (SpriteRenderer == null)
            Debug.Log($"Failed to set color to {name} {nameof(PlayerController)}.");
        else
            SpriteRenderer.color = PlayerColor;
    }

    // Set up player input
    public void AssignPlayerInputDevice(PlayerInput playerInput)
    {
        // Record our player input (ie controller).
        PlayerInput = playerInput;
        // Find the references to the "Move" and "Jump" actions inside the player input's action map
        // Here I specify "Player/" but it in not required if assigning the action map in PlayerInput inspector.
        InputActionMove = playerInput.actions.FindAction($"Player/Move");
        
        // Setup context bool for interact button
        InputActionInteract = playerInput.actions.FindAction($"Player/Interact");
        playerInput.actions.FindAction("Player/Dash").started += ctx => isDashing = true;
        playerInput.actions.FindAction("Player/Dash").canceled += ctx => isDashing = false;
    }

    // Assign player number on spawn
    public void AssignPlayerNumber(int playerNumber)
    {
        this.PlayerNumber = playerNumber;
    }

    // Runs each frame
    public void Update()
    {
        HandleDash();
        
        HandlePlayerInteract();
        HandleGhostInteract();
    }

    void HandlePlayerInteract()
    {
        // Break if ghost
        if (isGhost)
            return;

        if (InputActionInteract.WasPressedThisFrame())
        {
            Debug.Log("Player Interact");
        }
    }

    void HandleGhostInteract()
    {
        // Break if not ghost
        if (!isGhost)
            return;
        
        if (InputActionInteract.WasPressedThisFrame())
        {
            Debug.Log("Ghost Interact");
        }
    }
    
    // Updates player speed based on input and stamina
    void HandleDash()
    {
        // Prevent dashing for ghosts
        if (isGhost)
            return;
        
        if (isDashing)
        {
            CurrentSpeed = DashSpeed;
        }
        else
        {
            CurrentSpeed = MoveSpeed;
        }
    }

    // Runs each phsyics update
    void FixedUpdate()
    {
        if (Rigidbody2D == null)
        {
            Debug.Log($"{name}'s {nameof(PlayerController)}.{nameof(Rigidbody2D)} is null.");
            return;
        }

        // MOVE
        // Read the "Move" action value, which is a 2D vector
        Vector2 moveValue = InputActionMove.ReadValue<Vector2>();
        // Here we're only using the X axis to move.
        float moveForce = moveValue.x * CurrentSpeed;
        float moveForceY = moveValue.y * CurrentSpeed;
        // Apply fraction of force each frame
        Rigidbody2D.AddForceX(moveForce, ForceMode2D.Force);
        Rigidbody2D.AddForceY(moveForceY, ForceMode2D.Force);

        if (Rigidbody2D.linearVelocity != Vector2.zero)
        {
            Vector2 deccelVel = Vector2.Lerp(Rigidbody2D.linearVelocity, Vector2.zero, Time.deltaTime * deccelSpeed);
            Rigidbody2D.linearVelocity = deccelVel;
        }
    }

    // OnValidate runs after any change in the inspector for this script.
    private void OnValidate()
    {
        Reset();
    }

    // Reset runs when a script is created and when a script is reset from the inspector.
    private void Reset()
    {
        // Get if null
        if (Rigidbody2D == null)
            Rigidbody2D = GetComponent<Rigidbody2D>();
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}
