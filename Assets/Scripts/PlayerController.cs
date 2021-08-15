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
    private bool playerDirectionForward;

    private void Awake() {
        playerControls = new PlayerActionControls();
        body = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Start() {
        playerControls.Land.Jump.started += _ => Jump();
        playerDirectionForward = true;
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
        if (animator.GetBool("isJumping") == true)
            StartCoroutine(WaitForJumpEnd());
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

        //sprite flip //idk do some shit with keeping the direction in a variable and fucking flip it
        if (input == -1 && playerDirectionForward) {
            StartCoroutine(WaitForTurnAround());
            playerDirectionForward = false;
        } else if (input == 1 && !playerDirectionForward) {
            StartCoroutine(WaitForTurnAround());
            playerDirectionForward = true; 
        }

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
    }

    private void Jump() {
        if (IsGrounded()) {
            body.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //Collision with an enemy tag that causes a death
       if (other.gameObject.tag == "Enemy") {
           animator.SetTrigger("isDead");
           StartCoroutine(WaitAnimation());
        }
    }

   // Custom waiting animations on things that rely on time

    private IEnumerator WaitAnimation() {
        OnDisable();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator WaitForJumpEnd() {
        yield return new WaitForSeconds(0.05f);
         if (IsGrounded())
            animator.SetBool("isJumping", false);
    }

    private IEnumerator WaitForTurnAround() {
        animator.SetTrigger("isTurning");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        spriteRenderer.flipX = !playerDirectionForward;      
    }

    //Needed for sprite animation to work

    private void OnEnable() {
        playerControls.Enable();
    }
    
    private void OnDisable() {
        playerControls.Disable();
    }
}
