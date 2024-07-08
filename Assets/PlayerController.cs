using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

using FishNet.Object.Synchronizing;
using UnityEngine.PlayerLoop;

//This is made by Bobsi Unity - Youtube
public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 5f;
    public float runningSpeed = 10f;
    public float jumpSpeed = 7.0f;
    public float lookSpeed = 2.0f;
    public float gravity = 15.0f;
    public float lookXLimit = 45.0f;
    private float walkFOV = 70f;
    private float runFOV = 90f;
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool confusion = false;
    public bool canMove = true;

    private int windUpdate = 0;
    private Vector3 windDirection = Vector3.zero;

    [SerializeField]
    private float cameraYOffset = 1f;
    private Camera playerCamera;
    [HideInInspector] public bool isWalking = false, isMoonwalking = false, isWalkingLeft = false, isWalkingRight = false, isJumping = false, isRunning = false;
    [HideInInspector] public bool frost = false, Agility = false, Heft = false;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z + .35f);
            playerCamera.transform.SetParent(transform);
            playerCamera.nearClipPlane = .2f;
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private IEnumerator MoveOverTime(Vector3 direction, float seconds)
    {
        canMove = false;
        float duration = seconds + Time.time;
        //while (Time.time < duration)
        //{
        //    characterController.Move(direction * Time.deltaTime);
        //    yield return new WaitForSeconds(.05f);
        //}
        characterController.Move(direction);//TOO SHARP
        //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, direction, duration); ;
        canMove = true;
        yield return null;
    }
    public void AddForce(Vector3 direction)
    {//USED BY WINDPOWER IMPACT
        const int frameDuration = 30;
        windUpdate = frameDuration;//add direction for n fixedUpdates
        windDirection = direction * 2 * windUpdate / frameDuration;
        //StartCoroutine(MoveOverTime(direction, .5f));
    }
    private void FixedUpdate()
    {
        if (windUpdate > 0)
        {
            windUpdate--;

        }
    }
    void Update()
    {
        #region Cursor
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            Cursor.lockState = CursorLockMode.None;
        if (Input.GetKeyUp(KeyCode.LeftAlt))
            Cursor.lockState = CursorLockMode.Locked;
        #endregion
        isRunning = false;

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);
        #region Animation
        if (isRunning)
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, runFOV, .5f);
        else
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, walkFOV, .5f);

        isWalking = Input.GetKey(KeyCode.W);
        isMoonwalking = Input.GetKey(KeyCode.S);
        isWalkingLeft = Input.GetKey(KeyCode.A);
        isWalkingRight = Input.GetKey(KeyCode.D);
        #endregion

        float updWalkingSpeed = walkingSpeed;
        float updRunningSpeed = runningSpeed;
        float updLookSpeed = lookSpeed;
        float updJumpSpeed = jumpSpeed;
        float updGravity = gravity;
        #region PassiveEffects
        if (frost)
        {
            updWalkingSpeed = updWalkingSpeed * .1f;
            updRunningSpeed = updRunningSpeed * .1f;
            updLookSpeed = updLookSpeed * .3f;
        }
        if (Agility)
        {
            updWalkingSpeed = updWalkingSpeed * 1.5f;
            updRunningSpeed = updRunningSpeed * 1.5f;
            updJumpSpeed *= 1.2f;
        }
        if(Heft)
        {
            updWalkingSpeed = updWalkingSpeed * .5f;
            updRunningSpeed = updRunningSpeed * .5f;
            updGravity *= 5;
        }
        #endregion
        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = canMove ? (isRunning ? updRunningSpeed : updWalkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? updRunningSpeed : updWalkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        if (confusion)
        {
            moveDirection = (-forward * curSpeedX) + (-right * curSpeedY);
        }
        else
        {
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        }

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = updJumpSpeed;
            isJumping = true;
        }
        else
        {
            isJumping = false;
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= updGravity * Time.deltaTime;
        }
        if (windUpdate != 0)
            moveDirection += windDirection;
        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove && playerCamera != null)
        {
            if (confusion)
            {
                rotationX += Input.GetAxis("Mouse Y") * updLookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, -Input.GetAxis("Mouse X") * updLookSpeed, 0);
            }
            else
            {
                rotationX += -Input.GetAxis("Mouse Y") * updLookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * updLookSpeed, 0);
            }
        }

    }

    [ObserversRpc]
    public void TransportPlayerToPosition(Vector3 new_position)
    {
        if (!base.IsOwner) return;
        Debug.Log("Changing player position");
        this.characterController.enabled = false;
        this.transform.position = new_position;
        this.characterController.enabled = true;
        return;
    }

    [TargetRpc]
    public void SetCanMove(NetworkConnection net_connection, bool setting)
    {
        this.canMove = setting;
    }
}

