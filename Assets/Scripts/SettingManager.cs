using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingManager : MonoBehaviour {

	public Toggle fullscreenToggle;
	public Dropdown resolutionDropdown;
	public Dropdown vSyncDropdown;
	//public Slider volumeSlider;

	//public AudioSource musicSource;
	public Resolution[] resolutions;
	GameSettings gameSettings;
	public Button applyButton;
	public Button backButton;
	public GameObject mainPauseHolder;
	public GameObject optionsHolder;

	void OnEnable(){

		gameSettings = new GameSettings ();

		fullscreenToggle.onValueChanged.AddListener (delegate {
			OnFullscreenToggle ();
		});

		resolutionDropdown.onValueChanged.AddListener (delegate {
			OnResolutionChange ();
		});

		vSyncDropdown.onValueChanged.AddListener (delegate {
			OnVSyncChange ();
		});

		/*volumeSlider.onValueChanged.AddListener (delegate {       //Scoti asta din comment daca ai muzica
			OnVolumeChange();
		});*/ 
		applyButton.onClick.AddListener (delegate {
			OnApplyButtonClick ();
		});
		backButton.onClick.AddListener (delegate {
			OnBackButtonClick ();
		});

		resolutions = Screen.resolutions;
		foreach (Resolution resolution in resolutions) {

			resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
		}

		if (File.Exists(Application.persistentDataPath + "/gamesettings.json") == true)
		{
			LoadSettings();
		}
	}

	public void OnFullscreenToggle(){

		gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
	}

	public void OnResolutionChange(){

		Screen.SetResolution (resolutions [resolutionDropdown.value].width, resolutions [resolutionDropdown.value].height, Screen.fullScreen);
		gameSettings.resolutionIndex = resolutionDropdown.value;
	}

	public void OnVSyncChange(){
		QualitySettings.vSyncCount = gameSettings.vSync = vSyncDropdown.value;
	}

	/*public void OnVolumeChange(){

		musicSource.volume = gameSettings.musicVolume = volumeSlider.value;                //Scoti asta din comment in caz ca ai muzica
	
	}*/

	public void OnApplyButtonClick(){
		SaveSettings ();
	}

	public void OnBackButtonClick(){
		mainPauseHolder.SetActive (true);
		optionsHolder.SetActive (false);
	}


	public void SaveSettings(){

		string jsonData = JsonUtility.ToJson (gameSettings, true);
		File.WriteAllText (Application.persistentDataPath + "/gamesettings.json", jsonData);

	}

	public void LoadSettings(){

		gameSettings = JsonUtility.FromJson<GameSettings> (File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

		//volumeSlider.value = gameSettings.musicVolume;
		vSyncDropdown.value = gameSettings.vSync;
		resolutionDropdown.value = gameSettings.resolutionIndex;
		fullscreenToggle.isOn = gameSettings.fullscreen;
		Screen.fullScreen = gameSettings.fullscreen;

		resolutionDropdown.RefreshShownValue ();
		
	}


}
