using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
	public float fallMultiplier;
	public float lowMultiplier;

	public PlayerController player;

	PlayerInputActions inputActions;

	private Rigidbody2D rb;

	private void Awake()
	{
		inputActions = new PlayerInputActions();
	}

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		if (!player.isWallSliding && !player.isWallJumping && !player.isDashing)
		{
			if (rb.velocity.y < 0)
			{
				rb.velocity += Vector2.up * Physics2D.gravity.y * (lowMultiplier - 1) * Time.deltaTime;
			}

			else if (rb.velocity.y > 0 && !inputActions.Player.Jump.triggered)
			{
				rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
			}
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
