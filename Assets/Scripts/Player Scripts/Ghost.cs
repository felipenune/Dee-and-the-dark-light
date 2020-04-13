using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	public PlayerController player;
	public GameObject ghost;
	private GameObject currentGhost;
	

	public Color[] spriteColors;

	public float spawnTime;
	private float spawCounter;

	void Update()
    {
		if (player.isDashing)
		{
			spawCounter -= Time.deltaTime;

			if (spawCounter <= 0)
			{
				spawCounter = spawnTime;
				currentGhost = Instantiate(ghost, transform.position, Quaternion.identity);

				Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
				SpriteRenderer ghostSprite = currentGhost.GetComponent<SpriteRenderer>();

				ghostSprite.sprite = currentSprite;
				currentGhost.transform.localScale = GetComponent<Transform>().localScale;

				if (player.state == "dark")
				{
					ghostSprite.color = spriteColors[0];
				} else
				{
					ghostSprite.color = spriteColors[1];
				}

				Destroy(currentGhost, 1f);
			}
		}
		else if (!player.isDashing)
		{
			spawCounter = spawnTime;
		}
	}
}
