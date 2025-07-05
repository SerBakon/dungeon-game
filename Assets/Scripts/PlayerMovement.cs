using Unity.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    [SerializeField] private float speed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float playerHeight;
    [SerializeField] private float jumpHeight = 3f;


    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform feetPos;

    private float x;
    private float z;

    private Vector3 move;
    [SerializeField] private Vector3 velocity;

    private bool isGrounded;

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

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            speed *= 1.75f;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)) {
            speed /= 1.75f;
        }

        characterController.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded) {
            Debug.Log("Jumping");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
}
