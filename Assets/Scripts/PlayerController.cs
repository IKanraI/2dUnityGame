using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour {

    [SerializeField] private float speed, jumpSpeed;
    [SerializeField] private LayerMask ground;
    private PlayerActionControls playerControls;
    private Rigidbody2D body;
    private Collider2D col;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Animation animation;

    private void Awake() {
        playerControls = new PlayerActionControls();
        body = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animation = GetComponent<Animation>();
    }

    private void OnEnable() {
        playerControls.Enable();
    }
    
    private void OnDisable() {
        playerControls.Disable();
    }
    
    void Start() {
        playerControls.Land.Jump.performed += _ => Jump();
    }

    private bool IsGrounded() {
        Vector2 topLeftPoint = transform.position;
        topLeftPoint.x -= col.bounds.extents.x;
        topLeftPoint.y += col.bounds.extents.y;

        Vector2 bottomRightPoint = transform.position;
        bottomRightPoint.x += col.bounds.extents.x;
        bottomRightPoint.y -= col.bounds.extents.y;
        return Physics2D.OverlapArea(topLeftPoint, bottomRightPoint, ground);
    }

    // Update is called once per frame
    void Update() {
        Move();
    }

    private void Crouch(float input) {
        if (input != 0) {
            animator.SetBool("isCrouching", true);
            animator.SetBool("isIdle", false);
        }
        else 
            animator.SetBool("isCrouching", false);
            animator.SetBool("isCrouchWalking", false);
            animator.SetBool("isIdle", true);
    }

    private void Move() {
        //read movement value
        float input = playerControls.Land.Move.ReadValue<float>();
        float inputCrouch = playerControls.Land.Crouch.ReadValue<float>();

        //move the player
        Vector3 currPos = transform.position;
        currPos.x += input * speed * Time.deltaTime;
        transform.position = currPos;

        
        //animation
        Crouch(inputCrouch);

        if (input != 0 && inputCrouch == 0) {
            animator.SetBool("isRunning", true);
            animator.SetBool("isIdle", false);
        }
        else if (input != 0 && inputCrouch != 0) {
            animator.SetBool("isCrouchWalking", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
        }
        else {
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouchWalking", false);
        }

        if (input == 0 && inputCrouch == 0)
            animator.SetBool("isIdle", true);

        //sprite flip
        if (input == -1) {
            animator.SetTrigger("isTurning");
            spriteRenderer.flipX = true;
        }
        else if (input == 1) {
            //animator.SetTrigger("isTurning");
            spriteRenderer.flipX = false;
        }
    }

    private void Jump() {
        if (IsGrounded()) {
            body.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);

            animator.SetTrigger("isJumping");
        
        }
    }
   private void OnCollisionEnter2D(Collision2D other) {
       if (other.gameObject.tag == "Enemy") {
           animator.SetTrigger("isDead");
           
          // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
       }
   }

}
