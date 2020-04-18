using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
	public GameObject player;

	private PlayerController playerController;
	private Transform playerTransform;
	private Rigidbody2D playerRb;

	public float transitionTime;
	public bool isMoving;

	private Vector2 pos;

	private void Start()
	{
		playerTransform = player.GetComponent<Transform>();
	}

	void Update()
    {
		if (playerTransform.position.x > transform.position.x + 20)
		{
			transform.position = new Vector3(transform.position.x + 40, transform.position.y, transform.position.z);
		}

		if (playerTransform.position.x < transform.position.x - 20)
		{
			transform.position = new Vector3(transform.position.x - 40, transform.position.y, transform.position.z);
		}

		if (playerTransform.position.y > transform.position.y + 11.25f)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + 22.5f, transform.position.z);
		}

		if (playerTransform.position.y < transform.position.y - 13f)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - 22.5f, transform.position.z);
		}
	}
}
