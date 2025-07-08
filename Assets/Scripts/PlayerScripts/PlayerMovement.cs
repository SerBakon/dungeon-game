using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private StaminaSlider staminaSlider;

    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float playerHeight;
    [SerializeField] private float jumpHeight = 3f;


    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform feetPos;

    private float x;
    private float z;

    private float sprintSpeed;
    private float normalSpeed;

    private Vector3 move;
    [SerializeField] private Vector3 velocity;

    private bool isGrounded;
    private void Start() {
        normalSpeed = speed;
        sprintSpeed = speed * 1.75f;
    }

    // Update is called once per frame
    void Update()
    {
        //finish raycast
        isGrounded = Physics.CheckSphere(feetPos.position, .1f, ground);

        //Debug.Log(isGrounded);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -1f;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;

        checkSprint();

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            //Debug.Log("Sprinting");
            startSprint();
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)) {
            endSprint();
        }

        characterController.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded) {
            Debug.Log("Jumping");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void startSprint() {
        if (staminaSlider.stamina > 0)
            speed = sprintSpeed;
        else
            speed = normalSpeed;
            staminaSlider.sprinting = true;
        
    }

    private void endSprint() {
        speed = normalSpeed;
        staminaSlider.sprinting = false;
    }

    private bool checkSprint() {
        if (staminaSlider.stamina <= 0 && staminaSlider.sprinting) {
            Debug.Log("cannot sprint anymore");
            staminaSlider.stamina = -1;
            speed = normalSpeed;
            return false;
        }
        return true;
    }
}
