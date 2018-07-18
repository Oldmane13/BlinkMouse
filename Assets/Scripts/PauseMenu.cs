using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public string quit;	
	public bool isPaused;
	public GameObject pauseMenuCanvas;

	Teleporter teleport;
	Controller2D soric;
	PlayerPhysics soric1;

	public GameObject mainPauseHolder;
	public GameObject optionsHolder;

    Scene m_Scene;

    void Start(){

		teleport = FindObjectOfType<Teleporter> ();
		soric = FindObjectOfType<Controller2D> ();
		soric1 = FindObjectOfType<PlayerPhysics> ();
		}

	
	void Update () {
        m_Scene = SceneManager.GetActiveScene();
        if (isPaused){	
			Cursor.visible = true;
			pauseMenuCanvas.SetActive(true);
			Time.timeScale = 0f;
			teleport.enabled = false;
			soric.enabled = false;
			soric1.enabled = false;


		
		}else{			
			pauseMenuCanvas.SetActive(false);
			Time.timeScale = 1f;
			teleport.enabled = true;
			soric.enabled = true;
			soric1.enabled = true;
			Cursor.visible = false;
		}
	
		if (Input.GetKeyDown(KeyCode.Escape)){
			
			mainPauseHolder.SetActive (true);
			optionsHolder.SetActive (false);
			isPaused = !isPaused;
		}
	}
	
	public void Resume(){
        SceneManager.LoadScene("MainMenu");
	}

	public void OptionsMenu() {
		mainPauseHolder.SetActive (false);
		optionsHolder.SetActive (true);
	}

	public void MainPauseMenu() {
		mainPauseHolder.SetActive (true);
		optionsHolder.SetActive (false);
	}
	
	public void Reload(){
        SceneManager.LoadScene(m_Scene.name);

    }
	
	public void Quit(){
		Application.Quit();	
	}


}
