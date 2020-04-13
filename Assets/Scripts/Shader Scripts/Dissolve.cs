using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
	[ColorUsageAttribute(true, true)]
	public Color[] colors;

	public PlayerController player;
	public GameObject lightComponent;

	public bool isDissolving = false;
	private float dissolveAmount = 1;

	private SpriteRenderer lightSprite;
	private SpriteRenderer darkSprite;

	private Material darkDissolveMaterial;
	private Material lightDissolveMaterial;

	PlayerInputActions inputActions;

	private void Awake()
	{
		inputActions = new PlayerInputActions();
	}

	void Start()
    {
		darkSprite = GetComponent<SpriteRenderer>();
		lightSprite = lightComponent.GetComponent<SpriteRenderer>();
		darkDissolveMaterial = darkSprite.material;
		lightDissolveMaterial = lightSprite.material;

	}

	void Update()
	{
		if (inputActions.Player.Dissolve.triggered)
		{
			isDissolving = true;
		}

		if (isDissolving)
		{
			if (player.state == "dark")
			{
				dissolveAmount -= Time.deltaTime * 2.5f;

				if (lightSprite.enabled == false)
				{
					lightSprite.enabled = true;
				}

				if (dissolveAmount <= 0)
				{
					dissolveAmount = 0;
					player.state = "light";
					darkSprite.sortingOrder = -1;
					lightSprite.sortingOrder = 1;
					darkSprite.enabled = false;
					dissolveAmount = 1;
					isDissolving = false;
				}

				darkDissolveMaterial.SetFloat("_dissolveAmount", dissolveAmount);
				darkDissolveMaterial.SetColor("_Color", colors[1]);
			}
			else
			{
				dissolveAmount -= Time.deltaTime * 2.5f;

				if (darkSprite.enabled == false)
				{
					darkSprite.enabled = true;
				}

				if (dissolveAmount <= 0)
				{
					dissolveAmount = 0;
					player.state = "dark";
					lightSprite.sortingOrder = -1;
					darkSprite.sortingOrder = 1;
					lightSprite.enabled = false;
					dissolveAmount = 1;
					isDissolving = false;
				}

				lightDissolveMaterial.SetFloat("_dissolveAmount", dissolveAmount);
				lightDissolveMaterial.SetColor("_Color", colors[0]);
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
