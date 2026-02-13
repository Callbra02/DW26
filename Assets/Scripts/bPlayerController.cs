using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class bPlayerController : MonoBehaviour
{
    [field: SerializeField] public int PlayerNumber { get; private set; }
    [field: SerializeField] public Color PlayerColor { get; private set; }
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    [field: SerializeField] public Rigidbody2D Rigidbody2D { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; } = 10f;
    [field: SerializeField] public float DashSpeed { get; private set; } = 15f;
    [field: SerializeField] public float deccelSpeed { get; private set; } = 2.5f;

    private GhostCollider ghostCollider;
    private Coroutine runningCoroutine;
    
    public bool isGhost = false;
    private bool isPlayerInRange = false;
    public bool canScare = true;
    public CircleCollider2D ghostScareCollider;

    private bool canDash = true;
    private bool isDashing = false;
    public float dashStaminaTax = 2.5f;
    public float dashStaminaRegenRate = 1.0f;
    public float maxStamina { get; private set; } = 10.0f;
    public float currentStamina { get; private set; } = 10.0f;

    public bool isSlowed = false;
    private float slowTimer = 0.0f;

    private float CurrentSpeed;
    public bool DoDash { get; private set; }

    public float maxHealth = 100.0f;
    public float currentHealth;

    public bool isScared = false;
    private bool CR_DoScare = false;
    public float scareTime = 2.5f;
    private Vector2 scareDir;
    
    private bool isInteracting = false;
    private GameObject currentArtifact = null;
    
    // Player input information
    private PlayerInput PlayerInput;
    private InputAction InputActionMove;
    private InputAction InputActionInteract;

    private bool isColliding = false;

    private void Start()
    {
        currentHealth = maxHealth;
        ghostCollider = GetComponentInChildren<GhostCollider>();
        if (isGhost)
        {
            this.GetComponent<PlayerVisualization>().spawnOffset.x =
                -this.GetComponent<PlayerVisualization>().spawnOffset.x;
        }
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
        //else
            //SpriteRenderer.color = PlayerColor;
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

        HandleSlow();
        
        // Break from logic if player is scared
        if (isScared)
            return;
        
        HandleDash();

        if (isGhost)
        {
            this.isPlayerInRange = ghostCollider.isPlayerInRange;
            HandleGhostInteract();
        }
        else
        {
            HandlePlayerInteract();
        }
    }
    
    void HandlePlayerInteract()
    {
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
        if (InputActionInteract.WasPressedThisFrame())
        {
            Debug.Log("Ghost Interact");
            if (isPlayerInRange && canScare)
            {
                ghostCollider.playerInRange.isScared = true;
            }
        }
    }

    void HandleSlow()
    {
        if (!isSlowed)
        {
            slowTimer = 0.0f;
            return;
        }

        slowTimer += Time.deltaTime;

        if (slowTimer > 2.0f)
        {
            isSlowed = false;
        }
    }
    
    void HandleGhostScareTrap()
    {
        
    }
    
    // Updates player speed based on input and stamina
    void HandleDash()
    {
        // Prevent dashing for ghosts
        if (isGhost)
        {
            CurrentSpeed = MoveSpeed;
            return;
        }

        HandleStaminaRegeneration();

        // If player is scared, dash and do not tax player
        if (isScared)
        {
            CurrentSpeed = DashSpeed;
            return;
        }
        
        // If player can dash, and is dashing, deplete stamina and raise movement speed
        if (isDashing && canDash)
        {
            CurrentSpeed = DashSpeed;
            currentStamina -= dashStaminaTax * Time.deltaTime;
        }
        // Else regen stamina
        else
        {
            if (isSlowed)
            {
                CurrentSpeed = MoveSpeed * 0.5f;
            }
            else
            {
                CurrentSpeed = MoveSpeed;
            }
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

    // Return new random scare direction
    Vector2 GetScareDirection()
    {
        return new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    }
    
    // Runs each phsyics update
    void FixedUpdate()
    {
        // Log if player does not have a rigidbody
        if (Rigidbody2D == null)
        {
            Debug.Log($"{name}'s {nameof(PlayerController)}.{nameof(Rigidbody2D)} is null.");
            return;
        }

        // Declare and initialize moveValue to a default
        Vector2 moveValue = new Vector2();
        
        if (isScared)
        {
            // If scare coroutine is not running, set running coroutine to DoScare
            // Stop coroutine after given scare time
            // Only do this once per isScared cycle
            if (!CR_DoScare)
            {
                if (currentArtifact != null)
                    DropArtifact();
                
                runningCoroutine = StartCoroutine(DoScare());
                StartCoroutine(StopCrAfter(scareTime));
                CR_DoScare = true;
            }

            // Set movedir to scaredir
            moveValue = scareDir;
        }
        else
        {
            // Read player input when appropriate
            moveValue = InputActionMove.ReadValue<Vector2>();
        }

        // MOVE
        Vector2 moveForce = new Vector2(moveValue.x * CurrentSpeed, moveValue.y * CurrentSpeed);
        
        bUtils.RotateToDirection(new Vector2(moveForce.y, -moveForce.x), this.transform);
        Rigidbody2D.AddForceX(moveForce.x, ForceMode2D.Force);
        Rigidbody2D.AddForceY(moveForce.y, ForceMode2D.Force);

        // If player is moving, apply decel vel
        if (Rigidbody2D.linearVelocity != Vector2.zero)
        {
            // Decelerate the player
            Vector2 deccelVel = Vector2.Lerp(Rigidbody2D.linearVelocity, Vector2.zero, Time.deltaTime * deccelSpeed);
            Rigidbody2D.linearVelocity = deccelVel;
        }
    }

    // Pickup artifact if player interacts with one
    private void PickupArtifact(GameObject artifactObject)
    {
        currentArtifact = artifactObject;
        currentArtifact.GetComponent<Artifact>().isHeld = true;
        currentArtifact.transform.position = this.transform.position;
        currentArtifact.transform.SetParent(this.transform);
    }

    // Drops artifact if player has one currently
    public void DropArtifact()
    {
        if (currentArtifact == null)
            return;
        
        currentArtifact.GetComponent<Artifact>().isHeld = false;
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

    // End all running coroutines on end
    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        isColliding = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        isColliding = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("GhostTrap"))
        {
            isSlowed = true;
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Artifact"))
            return;
        
        if (isInteracting && !isGhost && !other.GetComponent<Artifact>().isHeld)
        {
            PickupArtifact(other.gameObject);
        }
    }

    // Helper function that stops a coroutine after a delay
    IEnumerator StopCrAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            
            // Scare is the only coroutine in the script so call these when cr should be stopped
            CR_DoScare = false;
            isScared = false;
        }
    }
    
    IEnumerator DoScare()
    {
        CR_DoScare = true;
        
        while (true)
        {
            // Set scare direction to random direction
            scareDir = GetScareDirection();

            if (isColliding)
                scareDir = GetScareDirection();
            
            // Wait to swap direction after a tenth of the scaretime
            yield return new WaitForSeconds(scareTime * 0.1f);
        }
    }
}
