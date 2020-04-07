using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightMode : MonoBehaviour
{
	Animator anim;

	public PlayerController player;
    
    void Start()
    {
		anim = GetComponent<Animator>();
    }

    
    void Update()
    {
		if (player.isGrounded)
		{
			anim.SetBool("isJumping", false);

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
			anim.SetBool("isJumping", true);
		}

		if (Input.GetButtonDown("Jump"))
		{
			anim.SetTrigger("Jump");
		}
	}
}
