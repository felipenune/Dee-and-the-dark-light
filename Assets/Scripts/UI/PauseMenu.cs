using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	PlayerInputActions inputActions;

	public PlayerController player;

	public GameObject pauseMenu;
	public GameObject resumeButton;
	public GameObject musicSlider;

	public TMP_Dropdown resolutionDrop;

	private Resolution[] resolutions;

	public static bool gameIsPaused = false;

	void Awake()
	{
		inputActions = new PlayerInputActions();
	}

	void Start()
	{
		resolutions = Screen.resolutions;
		resolutionDrop.ClearOptions();

		List<string> options = new List<string>();

		int currentResolutionIndex = 0;
		for (int i = 0; i < resolutions.Length; i++)
		{
			string option = resolutions[i].width + "x" + resolutions[i].height;
			options.Add(option);

			if (resolutions[i].width == Screen.currentResolution.width &&
				resolutions[i].height == Screen.currentResolution.height)
			{
				currentResolutionIndex = i;
			}
		}

		resolutionDrop.AddOptions(options);
		resolutionDrop.value = currentResolutionIndex;
		resolutionDrop.RefreshShownValue();
	}

	void Update()
    {
		if (inputActions.Player.Pause.triggered)
		{
			if (gameIsPaused)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
	}

	void Pause()
	{
		pauseMenu.SetActive(true);
		StartCoroutine("Highlithg");
		player.moveInput = Vector2.zero;
		player.move.x = 0f;
		Time.timeScale = 0f;
		gameIsPaused = true;
	}

	public void Resume()
	{
		pauseMenu.SetActive(false);
		Time.timeScale = 1f;
		StartCoroutine("ResumeGame");
	}

	public void LoadMenu()
	{
		SceneManager.LoadScene("Menu");
		Time.timeScale = 1f;
		gameIsPaused = false;
	}

	public void FirstOptions()
	{
		EventSystem.current.SetSelectedGameObject(musicSlider);
	}

	public void FirstMain()
	{
		EventSystem.current.SetSelectedGameObject(resumeButton);
	}

	public void SetResolution(int resolutionIndex)
	{
		Resolution resolution = resolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	public void SetFullscreen(bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
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

	IEnumerator Highlithg()
	{
		EventSystem.current.SetSelectedGameObject(null);
		yield return null;
		EventSystem.current.SetSelectedGameObject(resumeButton);
	}

	IEnumerator ResumeGame()
	{
		yield return new WaitForSeconds(0.1f);
		gameIsPaused = false;
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
