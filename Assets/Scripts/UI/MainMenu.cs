using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
	public GameObject options;
	public GameObject play;

	public void FirstOptions()
	{
		EventSystem.current.SetSelectedGameObject(options);
	}

	public void FirstMain()
	{
		EventSystem.current.SetSelectedGameObject(play);
	}

	public void Play()
	{
		SceneManager.LoadScene("LevelTest");
	}

	public void QuitGame()
	{
		#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying == true)
		{
			UnityEditor.EditorApplication.isPlaying = false;
		}
		else
		{
			Application.Quit();
		}
		#endif

		Application.Quit();
	}
}
