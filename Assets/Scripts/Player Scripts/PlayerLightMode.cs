using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightMode : MonoBehaviour
{
	Animator anim;

	PlayerInputActions inputActions;

	public PlayerController player;

	private void Awake()
	{
		inputActions = new PlayerInputActions();
	}

	void Start()
    {
		anim = GetComponent<Animator>();
    }

    
    void Update()
    {
		if (!player.isDashing)
		{
			anim.SetFloat("velocityY", player.GetComponent<Rigidbody2D>().velocity.y);

			anim.SetBool("Dash", false);

			if (player.isGrounded && !player.isOnWall)
			{
				if (player.move.x != 0)
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

			if (player.isWallJumping || player.isJumping)
			{
				anim.SetTrigger("Jump");
			}
		}

		else
		{
			anim.SetBool("Dash", true);
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
