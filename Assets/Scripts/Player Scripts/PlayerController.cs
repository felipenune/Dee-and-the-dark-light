using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
	private bool isWallJumping;

	[Header("Wall Slide")]
	public float wallSlideSpeed;
	public bool isWallSliding;

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

	[Header("Components")]
	private Rigidbody2D rb;
	private Animator anim;

	[HideInInspector]
	public Vector2 move;

	void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

    void Update()
    {
		move.x = Input.GetAxisRaw("Horizontal");
		move.y = Input.GetAxisRaw("Vertical");

		Animations();
		Checks();
		Jump();

		if (!isGrounded && isOnWall && move.x != 0 && rb.velocity.y < 0)
		{
			isWallSliding = true;
		}
		else
		{
			isWallSliding = false;
		}

		if (isWallSliding && Input.GetButtonDown("Jump"))
		{
			Vector2 force = new Vector2(
				wallJumpForce * wallJumpDirection.x * -facingDir,
				wallJumpForce * wallJumpDirection.y
			);

			rb.velocity = Vector2.zero;

			rb.AddForce(force, ForceMode2D.Impulse);

			isWallJumping = true;

			StartCoroutine("StopMove");
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
		if (isGrounded)
		{
			anim.SetBool("isJumping", false);

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
			anim.SetBool("isJumping", true);
		}

		if (Input.GetButtonDown("Jump"))
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
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			isJumping = true;
			jumpTimerCounter = jumpTime;
			rb.velocity = Vector2.up * jumpForce;
		}

		if (Input.GetButton("Jump") && isJumping)
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

		if (Input.GetButtonUp("Jump"))
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

	IEnumerator StopMove()
	{
		canMove = false;

		FLip();

		yield return new WaitForSeconds(waitTime);

		isWallJumping = false;
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
}
