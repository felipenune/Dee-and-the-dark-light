using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnims : MonoBehaviour
{
	public PlayerController player;
	private Animator anim;
	
    void Start()
    {
		anim = GetComponent<Animator>();
    }

    void Update()
    {
		
    }
}
