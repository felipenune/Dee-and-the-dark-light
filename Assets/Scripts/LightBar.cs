using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LightBar : MonoBehaviour
{
	public PlayerController player;
	public Slider slider;

    void Start()
    {
        
    }

    void Update()
    {
		slider.maxValue = player.stateTime;
		slider.value = player.stateCounter;
    }
}
