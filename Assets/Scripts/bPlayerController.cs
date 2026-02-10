using UnityEngine;
using UnityEngine.InputSystem;

public class bPlayerController : MonoBehaviour
{
    [field: SerializeField] public int PlayerNumber { get; private set; }
    [field: SerializeField] public Color PlayerColor { get; private set; }
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    [field: SerializeField] public Rigidbody2D Rigidbody2D { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; } = 10f;
    [field: SerializeField] public float DashForce { get; private set; } = 5f;
    [field: SerializeField] public float deccelSpeed { get; private set; } = 0.34f;
    
    public bool isGhost = false;
    private bool canDash = true;
    private bool isDashing = false;

    public bool DoDash { get; private set; }

    // Player input information
    private PlayerInput PlayerInput;
    private InputAction InputActionMove;
    private InputAction InputActionDash;

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
        InputActionDash = playerInput.actions.FindAction($"Player/Dash");
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
        // Read the "Jump" action state, which is a boolean value
        if (InputActionDash.WasPressedThisFrame() && canDash)
        {
            // Buffer input becuase I'm controlling the Rigidbody through FixedUpdate
            // and checking there we can miss inputs.
            DoDash = true;
            //DoJump = true;
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
        float moveForce = moveValue.x * MoveSpeed;
        float moveForceY = moveValue.y * MoveSpeed;
        // Apply fraction of force each frame
        Rigidbody2D.AddForceX(moveForce, ForceMode2D.Force);
        Rigidbody2D.AddForceY(moveForceY, ForceMode2D.Force);

        if (Rigidbody2D.linearVelocity != Vector2.zero)
        {
            
            Vector2 deccelVel = Vector2.Lerp(Rigidbody2D.linearVelocity, Vector2.zero, Time.deltaTime * deccelSpeed);
            
            string msg = $"LVP: {Rigidbody2D.linearVelocity}, LVA: {deccelVel}";
            Debug.Log(msg);
            Rigidbody2D.linearVelocity = deccelVel;
        }

        // Dash only if the player is not the ghost
        if (DoDash && !isGhost)
        {
            // Get current velocity
            Vector3 currentVelocity = Rigidbody2D.linearVelocity;
            currentVelocity *= DashForce;
            
            Rigidbody2D.AddForce(currentVelocity, ForceMode2D.Impulse);
            DoDash = false;
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
