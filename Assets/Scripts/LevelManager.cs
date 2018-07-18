using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public float respawnDelay;
    public PlayerPhysics gamePlayer;
    public GameObject range;

	void Start () {
        gamePlayer = FindObjectOfType<PlayerPhysics>();
	}
	
	public void Respawn()
    {
        StartCoroutine("RespawnCoroutine");
    }

    public IEnumerator RespawnCoroutine()
    {
        range.SetActive(false);
        gamePlayer.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        gamePlayer.transform.position = gamePlayer.respawnPoint;
        gamePlayer.gameObject.SetActive(true);
        range.SetActive(true);
    }

}
