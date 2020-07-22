using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Standart")]
	public float gravity;
	public bool active = true;

	[Header("Movement")]
	public float speed;
	public bool canMove = true;

	[Header("Jump")]
	public float jumpForce;
	public bool isJumping = false;
	public float jumpTime;
	private float jumpTimerCounter;

	[Header("Coyot Jump")]
	public float coyotTime;
	private float coyotCounter;
	private bool jumpDelay;

	[Header("Wall Jump")]
	public float wallJumpForce;
	public Vector2 wallJumpDirection;
	public float waitTime;
	public bool isWallJumping;

	[Header("Wall Slide")]
	public float wallSlideSpeed;
	public bool isWallSliding;

	[Header("Dash")]
	public TrailRenderer dashTrail;
	public CinemachineVirtualCamera cam;
	public float dashSpeed;
	public float dashTime;
	public bool isDashing;
	private bool canDash;
	private float dashTimer;

	[Header("State")]
	public Dissolve dissolve;
	public string state = "dark";
	public float stateTime;
	public float stateCounter;

	[Header("Ground Check")]
	public Transform groundCheck;
	public LayerMask whatIsGround;
	public float checkRadius;
	public bool isGrounded;

	[Header("Wall Check")]
	public Transform wallCheck;
	public LayerMask whatIsWall;
	public float checkDistance;
	public bool isOnWall;

	[Header("Directions")]
	private int facingDir = 1;
	private Vector2 dashDir;

	[Header("Components")]
	private Rigidbody2D rb;
	private Animator anim;

	[Header("Inputs")]
	public float leftVib;
	public float rightVib;
	private bool holdJump;
	public Vector2 moveInput;
	PlayerInputActions inputActions;

	[HideInInspector]
	public Vector2 move;

	private void Awake()
	{
		inputActions = new PlayerInputActions();

		inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
		inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

		inputActions.Player.Jump.started += ctx => holdJump = true;
		inputActions.Player.Jump.canceled += ctx => holdJump = false;
	}

	void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		stateCounter = stateTime;
	}

    void Update()
    {
		if (active && !PauseMenu.gameIsPaused)
		{
			move.x = moveInput.x;
			move.y = moveInput.y;

			Animations();
			CoyoteJump();
			Jump();
			WallJump();
			Dash();
			StateControl();

			if (isGrounded && !isDashing)
			{
				canDash = true;
			}

			if (!isGrounded && isOnWall && move.x != 0 && rb.velocity.y < 0 && !isDashing)
			{
				isWallSliding = true;
			}
			else
			{
				isWallSliding = false;
			}

			if (move.x * facingDir < 0 && !isWallJumping && !isDashing)
			{
				FLip();
			}
		}

		if (PauseMenu.gameIsPaused)
		{
			if (Gamepad.current != null)
			{
				Gamepad.current.SetMotorSpeeds(0f, 0f);
			}

			moveInput = Vector2.zero;
		}
	}

	private void FixedUpdate()
	{
		if (active)
		{
			if (canMove)
			{
				Move();
			}

			Checks();
			WallSlide();
		}		
	}

	void Animations()
	{
		if (!isDashing)
		{
			anim.SetFloat("velocityY", rb.velocity.y);

			anim.SetBool("Dash", false);

			if (isGrounded && !isOnWall)
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

			if (isWallJumping || isJumping)
			{
				anim.SetTrigger("Jump");
			}
		}
		else
		{
			anim.SetBool("Dash", true);
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

	void CoyoteJump()
	{
		if (inputActions.Player.Jump.triggered)
		{
			jumpDelay = true;
		}

		if (jumpDelay)
		{
			coyotCounter += Time.deltaTime;
		}

		if (coyotCounter >= coyotTime)
		{
			jumpDelay = false;
			coyotCounter = 0f;
		}
	}

	void Jump()
	{
		if (coyotCounter > 0 && isGrounded && !isDashing)
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

	void WallJump()
	{
		if (isOnWall && inputActions.Player.Jump.triggered && !isGrounded)
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
	}

	void Dash()
	{
		if (canDash && inputActions.Player.Dash.triggered)
		{
			isDashing = true;
			canDash = false;
			isJumping = false;
			canMove = false;

			if (move.x > -0.3f && move.x < 0.3f && move.y > -0.3f && move.y < 0.3f)
			{
				dashDir.x = facingDir;
			}
			else if (move.x > 0.3f || move.x < -0.3f)
			{
				dashDir.x = facingDir;

				if (move.y > 0.3f || move.y < -0.3f)
				{
					dashDir.x = 0.7f * facingDir;
				}
			}

			if (move.y > 0.3f)
			{
				dashDir.y = 1;
			}
			else if (move.y < -0.3f)
			{
				dashDir.y = -1;
			}

			if (move.x == 0 && move.y == -1)
			{
				dashDir.y = -2;
			}

			rb.velocity = Vector2.zero;
			rb.AddForce(dashDir * dashSpeed, ForceMode2D.Impulse);

			dashTrail.emitting = true;

			if (Gamepad.current != null)
			{
				Gamepad.current.SetMotorSpeeds(leftVib, rightVib);
			}

			cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 1;

			StartCoroutine("StopMoveDash");
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

	void StateControl()
	{
		if (state == "light")
		{
			stateCounter -= Time.deltaTime;

			if (stateCounter <= 0)
			{
				dissolve.isDissolving = true;
			}
		}

		if (state == "dark" && stateCounter < stateTime)
		{
			stateCounter += Time.deltaTime * 2;
		}

		if (stateCounter >= stateTime)
		{
			stateCounter = stateTime;
		}
	}

	IEnumerator StopMoveWall()
	{
		canMove = false;

		if (!isDashing)
		{
			FLip();
		}

		yield return new WaitForSeconds(waitTime);

		isWallJumping = false;

		if (!isDashing)
		{
			canMove = true;
		}
	}

	IEnumerator StopMoveDash()
	{
		canMove = false;
		rb.gravityScale = 0f;

		yield return new WaitForSeconds(dashTime);

		if (Gamepad.current != null)
		{
			Gamepad.current.SetMotorSpeeds(0f, 0f);
		}

		cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;

		dashTrail.emitting = false;

		rb.gravityScale = gravity;

		rb.velocity = Vector2.zero;

		if (dashDir.y == -1f)
		{
			rb.velocity = new Vector2(0f, rb.velocity.y);
		}

		dashDir = Vector2.zero;

		isDashing = false;
		canMove = true;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.CompareTag("Ground"))
		{
			dashTrail.emitting = false;

			rb.gravityScale = gravity;

			rb.velocity = Vector2.zero;
			dashDir = Vector2.zero;

			isDashing = false;
			canMove = true;
		}
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
