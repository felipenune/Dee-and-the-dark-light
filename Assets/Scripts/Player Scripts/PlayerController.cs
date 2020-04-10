using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Standart")]
	public float gravity;

	[Header("Movement")]
	public float speed;
	private bool canMove = true;

	[Header("Jump")]
	public float jumpForce;
	public float jumpTime;
	private float jumpTimerCounter;
	private bool isJumping = false;

	[Header("Wall Jump")]
	public float wallJumpForce;
	public Vector2 wallJumpDirection;
	public float waitTime;
	public bool isWallJumping;

	[Header("Wall Slide")]
	public float wallSlideSpeed;
	public bool isWallSliding;

	[Header("Dash")]
	public float dashSpeed;
	public float dashTime;
	public bool isDashing;
	private bool canDash;
	private float dashTimer;

	[Header("State")]
	public string state = "dark";

	[Header("Ground Check")]
	public Transform groundCheck;
	public LayerMask whatIsGround;
	public float checkRadius;
	public bool isGrounded;

	[Header("Wall Check")]
	public Transform wallCheck;
	public LayerMask whatIsWall;
	public float checkDistance;
	private bool isOnWall;

	[Header("Directions")]
	private int facingDir = 1;
	private Vector2 dashDir;

	[Header("Components")]
	private Rigidbody2D rb;
	private Animator anim;

	[Header("Inputs")]
	PlayerInputActions inputActions;
	private bool holdJump;

	[HideInInspector]
	public Vector2 move;
	private Vector2 moveInput;

	private void Awake()
	{
		inputActions = new PlayerInputActions();

		inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
		inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

		inputActions.Player.Jump.performed += ctx => holdJump = true;
		inputActions.Player.Jump.canceled += ctx => holdJump = false;
	}

	void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

    void Update()
    {
		/*move.x = Input.GetAxisRaw("Horizontal");
		move.y = Input.GetAxisRaw("Vertical");*/

		move.x = moveInput.x;
		move.y = moveInput.y;

		Animations();
		Checks();
		Jump();

		if (isGrounded)
		{
			canDash = true;
		}

		if (!isGrounded && isOnWall && move.x != 0 && rb.velocity.y < 0)
		{
			isWallSliding = true;
		}
		else
		{
			isWallSliding = false;
		}

		if (isWallSliding && inputActions.Player.Jump.triggered)
		{
			Vector2 force = new Vector2(
				wallJumpForce * wallJumpDirection.x * -facingDir,
				wallJumpForce * wallJumpDirection.y
			);

			rb.velocity = Vector2.zero;

			rb.AddForce(force, ForceMode2D.Impulse);

			isWallJumping = true;

			StartCoroutine("StopMoveWall");
		}

		if (canDash && inputActions.Player.Dash.triggered)
		{
			isDashing = true;
			canDash = false;
			
			if (move.x > -0.3f && move.x < 0.3f && move.y > -0.3f && move.y < 0.3f)
			{
				dashDir.x = facingDir;
			}
			else if (move.x > 0.3f || move.x < -0.3f)
			{
				dashDir.x = facingDir;
			}

			if (move.y > 0.3f)
			{
				dashDir.y = 1;
			}
			else if (move.y < -0.3f)
			{
				dashDir.y = -1;
			}

			rb.velocity = Vector2.zero;
			rb.AddForce(dashDir * dashSpeed, ForceMode2D.Impulse);

			StartCoroutine("StopMoveDash");
		}

		if (move.x * facingDir < 0 && !isWallJumping)
		{
			FLip();
		}

		
	}

	private void FixedUpdate()
	{
		if (canMove)
		{
			Move();
		}

		WallSlide();		
	}

	void Animations()
	{
		anim.SetFloat("velocityY", rb.velocity.y);

		if (isGrounded)
		{

			if (move.x != 0)
			{
				anim.SetBool("Walk", true);
			}
			else
			{
				anim.SetBool("Walk", false);
			}
		}
		else
		{
			anim.SetBool("Walk", false);
		}

		if (inputActions.Player.Jump.triggered)
		{
			anim.SetTrigger("Jump");
		}
	}

	void FLip()
	{
		facingDir *= -1;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void Move()
	{
		rb.velocity = new Vector2(move.x * speed, rb.velocity.y);
	}

	void Jump()
	{
		if (inputActions.Player.Jump.triggered && isGrounded && !isDashing)
		{
			isJumping = true;
			jumpTimerCounter = jumpTime;
			rb.velocity = new Vector2(rb.velocity.x, 0f);
			rb.velocity = Vector2.up * jumpForce;
		}

		if (holdJump && isJumping && !isDashing)
		{
			if (jumpTimerCounter > 0)
			{
				rb.velocity = Vector2.up * jumpForce;
				jumpTimerCounter -= Time.deltaTime;
			}
			else
			{
				isJumping = false;
			}
		}

		if (!holdJump)
		{
			isJumping = false;
		}
	}

	void WallSlide()
	{
		if (isWallSliding)
		{
			if (rb.velocity.y < wallSlideSpeed)
			{
				rb.velocity = new Vector2(rb.velocity.x, wallSlideSpeed);
			}
		}
	}

	void Checks()
	{
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

		if (facingDir == 1)
		{
			isOnWall = Physics2D.Raycast(wallCheck.position, transform.right, checkDistance, whatIsWall);
		}
		else
		{
			isOnWall = Physics2D.Raycast(wallCheck.position, -transform.right, checkDistance, whatIsWall);
		}
	}

	IEnumerator StopMoveWall()
	{
		canMove = false;

		FLip();

		yield return new WaitForSeconds(waitTime);

		isWallJumping = false;
		canMove = true;
	}

	IEnumerator StopMoveDash()
	{
		canMove = false;
		rb.gravityScale = 0f;

		yield return new WaitForSeconds(dashTime);

		rb.gravityScale = gravity;

		rb.velocity = Vector2.zero;
		dashDir = Vector2.zero;

		isDashing = false;
		canMove = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
		
		if (facingDir == 1)
		{
			Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + checkDistance, wallCheck.position.y, wallCheck.position.z));
		}
		else
		{
			Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x - checkDistance, wallCheck.position.y, wallCheck.position.z));
		}
	}

	private void OnEnable()
	{
		inputActions.Enable();
	}
	private void OnDisable()
	{
		inputActions.Disable();
	}
}
