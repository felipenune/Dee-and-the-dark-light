using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
	public float fallMultiplier;
	public float lowMultiplier;

	public PlayerController player;

	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		if (!player.isWallSliding)
		{
			if (rb.velocity.y < 0)
			{
				rb.velocity += Vector2.up * Physics2D.gravity.y * (lowMultiplier - 1) * Time.deltaTime;
			}

			else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
			{
				rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
			}
		}
	}
}
