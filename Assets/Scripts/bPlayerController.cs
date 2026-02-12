using System;
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
    public float dashStaminaTax = 2.5f;
    public float dashStaminaRegenRate = 1.0f;
    public float maxStamina { get; private set; } = 10.0f;
    public float currentStamina { get; private set; } = 10.0f;

    private float CurrentSpeed;
    public bool DoDash { get; private set; }

    public float maxHealth = 100.0f;
    public float currentHealth;

    private bool isInteracting = false;
    private GameObject currentArtifact = null;
    
    // Player input information
    private PlayerInput PlayerInput;
    private InputAction InputActionMove;
    private InputAction InputActionInteract;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    
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
            isInteracting = true;
        }
        else
        {
            isInteracting = false;
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
        
        HandleStaminaRegeneration();
        
        // If player can dash, and is dashing, deplete stamina and raise movement speed
        if (isDashing && canDash)
        {
            CurrentSpeed = DashSpeed;
            currentStamina -= dashStaminaTax * Time.deltaTime;
        }
        // Else regen stamina
        else
        {
            CurrentSpeed = MoveSpeed;
        }
    }

    void HandleStaminaRegeneration()
    {
        // If current stamina reaches 0, disable dash
        if (currentStamina <= 0)
            canDash = false;
        
        // If stamina is not maxxed, regenerate stamina
        // Else set stamina to max and enable dashing
        if (currentStamina < maxStamina)
        {
            currentStamina += dashStaminaRegenRate * Time.deltaTime;
        }
        else
        {
            currentStamina = maxStamina;
            canDash = true;
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
        Vector2 moveForce = new Vector2(moveValue.x * CurrentSpeed, moveValue.y * CurrentSpeed);
        Rigidbody2D.AddForceX(moveForce.x, ForceMode2D.Force);
        Rigidbody2D.AddForceY(moveForce.y, ForceMode2D.Force);

        if (Rigidbody2D.linearVelocity != Vector2.zero)
        {
            // Decelerate the player
            Vector2 deccelVel = Vector2.Lerp(Rigidbody2D.linearVelocity, Vector2.zero, Time.deltaTime * deccelSpeed);
            Rigidbody2D.linearVelocity = deccelVel;
        }
    }

    private void PickupArtifact(GameObject artifactObject)
    {
        currentArtifact = artifactObject;
        currentArtifact.GetComponent<CircleCollider2D>().enabled = false;
        currentArtifact.transform.position = this.transform.position;
        currentArtifact.transform.SetParent(this.transform);
    }

    private void DropArtifact()
    {
        if (currentArtifact == null)
            return;
        
        currentArtifact.GetComponent<CircleCollider2D>().enabled = true;
        currentArtifact.transform.SetParent(null);
        currentArtifact = null;
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isInteracting && !isGhost)
        {
            PickupArtifact(other.gameObject);
        }
    }
}
