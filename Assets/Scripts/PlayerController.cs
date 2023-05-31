using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float sensX = 50.0f;
    private float sensY = 50.0f;
    private float walkSpeed = 5f;
    private float runSpeed = 10f;
    private float moveSpeed = 5f;
    private float maxAccel = 20f;
    private float jumpForce = 5f;

    public Transform orientation; // Camera object should be inside of this
    public Animator anim;

    public bool isSeeking = false;

    private float xRot;
    private float yRot;

    private Vector2 move;
    private Vector2 mouseMove;
    private bool is_sprinting;
    private bool is_grounded;
    private Rigidbody rb;
    public GameObject positions;

    [SerializeField]
    private LayerMask groundMask;



    public void OnMovement(InputAction.CallbackContext value)
    {
        move = value.ReadValue<Vector2>();
    }

    public void OnCamMovement(InputAction.CallbackContext value)
    {
        mouseMove = value.ReadValue<Vector2>();
    }

    public void onSprint(InputAction.CallbackContext value)
    {
        float sprint_val = value.ReadValue<float>();
        is_sprinting = sprint_val == 1;
        if (is_sprinting)
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }
    public void onJump(InputAction.CallbackContext context)
    {
        if (is_grounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (positions != null)
        {
            int count = positions.transform.childCount;
            Transform pos = positions.transform.GetChild(Random.Range(0, count));
            this.transform.position = pos.position;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        moveSpeed = walkSpeed;
    }

    void Update()
    {
        // # Mouse Movement
        float mouseX = mouseMove.x * Time.deltaTime * sensX;
        float mouseY = mouseMove.y * Time.deltaTime * sensY;

        yRot += mouseX;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        orientation.rotation = Quaternion.Euler(xRot, yRot, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);

        // # Jumping
        Vector3 offset = new Vector3(0, 0.1f, 0);
        is_grounded = Physics.Raycast(transform.position + offset, Vector3.down, .6f, groundMask);

        // # WASD
        Vector3 newPos = Vector3.zero;
        newPos += transform.forward * move.y;
        newPos += transform.right * move.x;
        newPos.Normalize();
        Vector3 targetSpeed = newPos;
        AccelerateTo(targetSpeed);

        UpdateAnimation();
        updateMultiplayerVariables();
    }

    void UpdateAnimation()
    {
        if (move != Vector2.zero)
        {
            anim.SetFloat("left-right", move.x);
            MultiplayerSingleton.Instance.anim_left_to_right = move.x;
            if (!is_sprinting)
            {
                anim.SetBool("walking", true);
            MultiplayerSingleton.Instance.anim_walking = true;
            }
            else
            {
                anim.SetBool("running", true);
                MultiplayerSingleton.Instance.anim_running = true;
            }
        }
        else
        {
            MultiplayerSingleton.Instance.anim_walking = false;
            MultiplayerSingleton.Instance.anim_running = false;

            anim.SetBool("walking", false);
            anim.SetBool("running", false);
        }
    }

    void updateMultiplayerVariables() {
        MultiplayerSingleton.Instance.pos = transform.position;
        MultiplayerSingleton.Instance.rot = transform.rotation.eulerAngles;
        MultiplayerSingleton.Instance.vel = rb.velocity;
    }
    public void AccelerateTo(Vector3 targetSpeed)
    {
        Vector3 targetVel = Vector3.ClampMagnitude(targetSpeed.normalized * moveSpeed, moveSpeed);
        Vector3 bodyVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 vel = Vector3.MoveTowards(bodyVel, targetVel, Time.deltaTime * maxAccel);
        rb.velocity = new Vector3(vel.x, rb.velocity.y, vel.z);
    }

    public void updateColorsAndSeeker(bool isSeeker, Color surfaceColor, Color seekerJointColor, Color hiderJointColor) {
        PlayerColorManager colorManager = gameObject.GetComponent<PlayerColorManager>();
        colorManager.updateColors(isSeeker ? seekerJointColor : hiderJointColor, surfaceColor);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isSeeking) return;
        MultiplayerPlayerController mpc = collision.collider.GetComponent<MultiplayerPlayerController>();
        if (mpc == null)
        {
            return;
        }
        string player_id = mpc.getId();
        MultiplayerSingleton.Instance.SendPlayerFound(player_id);
    }
}
