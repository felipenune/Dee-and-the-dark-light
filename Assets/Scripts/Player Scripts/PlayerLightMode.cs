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
		anim.SetFloat("velocityY", player.GetComponent<Rigidbody2D>().velocity.y);

		if (player.isGrounded)
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

		if (inputActions.Player.Jump.triggered)
		{
			anim.SetTrigger("Jump");
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
