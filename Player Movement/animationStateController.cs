/*using UnityEngine;
using UnityEngine.InputSystem;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2.0f;
    public float decceleration = 2.0f;
    public float maxWalkVel = 0.5f;
    public float maxRunVal = 2.0f;

    int VelocityZHash;
    int VelocityXHash;

    PlayerInput input;
    Vector2 currentMovement;
    bool movementPressed;
    bool runPress;
    int isWalkingHash;
    int isRunningHash;

    public CharacterController controller;


    void Awake()
    {
        input = new PlayerInput();
        input.CharacterControls.Movement.performed += ctx =>
        {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        };
        input.CharacterControls.Run.performed += ctx => runPress = ctx.ReadValueAsButton();
    }


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        VelocityZHash = Animator.StringToHash("Velocity Z");
        VelocityXHash = Animator.StringToHash("Velocity X");
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // handles acceleration and deceleration
    void changeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVel)
    {
        if (forwardPressed && velocityZ < currentMaxVel)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        if (leftPressed && velocityX > -currentMaxVel)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        if (rightPressed && velocityX < currentMaxVel)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * decceleration;
        }

        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * decceleration;
        }

        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * decceleration;
        }
    }

    // handles reset and velocity
    void lockOrRestVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVel)
    {
        if (!forwardPressed && velocityZ < 0.0f)
        {
            velocityZ = 0.0f;
        }


        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0.0f;
        }

        if (forwardPressed && runPressed && velocityZ > currentMaxVel)
        {
            velocityZ = currentMaxVel;
        }

        else if (forwardPressed && velocityZ > currentMaxVel)
        {
            velocityZ -= Time.deltaTime * decceleration;
            if (velocityZ > currentMaxVel && velocityZ < (currentMaxVel + 0.05f))
            {
                velocityZ = currentMaxVel;
            }
        }

        else if (forwardPressed && velocityZ < currentMaxVel && velocityZ > (currentMaxVel - 0.05f))
        {
            velocityZ = currentMaxVel;
        }

        if (leftPressed && runPressed && velocityX < -currentMaxVel)
        {
            velocityX = -currentMaxVel;
        }

        else if (leftPressed && velocityX < -currentMaxVel)
        {
            velocityX += Time.deltaTime * decceleration;
            if (velocityX < -currentMaxVel && velocityX > (-currentMaxVel - 0.05))
            {
                velocityX = -currentMaxVel;
            }
        }

        else if (leftPressed && velocityX > -currentMaxVel && velocityX < (-currentMaxVel + 0.05f))
        {
            velocityX = -currentMaxVel;
        }

        if (rightPressed && runPressed && velocityX > currentMaxVel)
        {
            velocityX = currentMaxVel;
        }

        else if (rightPressed && velocityX > currentMaxVel)
        {
            velocityX -= Time.deltaTime * decceleration;
            if (velocityX > currentMaxVel && velocityX < (currentMaxVel + 0.05))
            {
                velocityX = currentMaxVel;
            }
        }

        else if (rightPressed && velocityX < currentMaxVel && velocityX > (currentMaxVel - 0.05f))
        {
            velocityX = currentMaxVel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //bool forwardPressed = Input.GetKey(KeyCode.W);
        //bool leftPressed = Input.GetKey(KeyCode.A);
        //bool rightPressed = Input.GetKey(KeyCode.D);
        //bool runPressed = Input.GetKey(KeyCode.LeftShift);
        //float currentMaxVel = runPressed ? maxRunVal : maxWalkVel;

        bool forwardPressed = false;
        bool leftPressed = false;
        bool rightPressed = false;
        bool runPressed = false;
        float currentMaxVel = runPress ? maxRunVal : maxWalkVel;
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = new Vector3(currentMovement.x, 0, currentMovement.y);
        Vector3 positionToLookAt = currentPosition + newPosition;
        transform.LookAt(positionToLookAt);

        // controller
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        
        // start walking if movement pressed is true and not already walking
        if(movementPressed && !isWalking)
        {
            animator.SetBool(isRunningHash, true);
            forwardPressed = true;
            controller.Move(newPosition * maxWalkVel * Time.deltaTime);
        }

        // stop walking
        if (!movementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
            forwardPressed = false;
        }

        // start running
        if((!movementPressed && runPress) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
            runPress = true;
            runPressed = true;
            currentMaxVel = runPressed ? maxRunVal : maxWalkVel;
            controller.Move(newPosition * maxRunVal * Time.deltaTime);
        }

        // stop running
        if ((!movementPressed || !runPress) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
            runPress = false;
            runPressed = false;
            currentMaxVel = runPress ? maxRunVal : maxWalkVel;
        }

        changeVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVel);
        lockOrRestVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVel);

        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);
    }

    void OnEnable()
    {
        input.CharacterControls.Enable();    
    }

    private void OnDisable()
    {
        input.CharacterControls.Disable();
    }
}
*/