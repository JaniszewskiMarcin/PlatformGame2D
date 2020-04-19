using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    private static int _remainingLives = 3;
    public static int RemainingLives => _remainingLives;

    void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
    }

    public Transform enemyPrefab;
    public Transform playerPrefab;
    public Transform spawnPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 2;
    public string respawnCountdown = "RespawnCountdown";
    public string spawnSoundName = "Spawn";
    public string gameOverSound = "GameOver";

    public CameraShake cameraShake;

    [SerializeField]
    private GameObject gameOverUI;

    private AudioManager audioManager;

    private void Start()
    {
        if(cameraShake == null)
        {
            Debug.LogError("No camera shake !");
        }

        audioManager = AudioManager.instance;
    }

    public void EndGame()
    {
        audioManager.PlaySound(gameOverSound);
        Debug.Log("GAME OVER!");
        gameOverUI.SetActive(true);
    }

    public IEnumerator _RespawnPlayer()
        {
            audioManager.PlaySound(respawnCountdown);
            yield return new WaitForSeconds(spawnDelay);

            audioManager.PlaySound(spawnSoundName);
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
            Destroy(clone.gameObject, 3.0f);
        }


    public static void KillPlayer(Player player)
    {

        Destroy(player.gameObject);
        _remainingLives--;

        if(_remainingLives <= 0)
        {
            gm.EndGame();
            _remainingLives = 3; 
        }

        else
        {
            gm.StartCoroutine(gm._RespawnPlayer());
        }

    }

    public static void KillEnemy(Enemy enemy)
    {
        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        audioManager.PlaySound(_enemy.deathSoundName);

        GameObject _clone = Instantiate(_enemy.deathParticles.gameObject, _enemy.transform.position, Quaternion.identity) as GameObject;
        Destroy(_clone, 5f);

        cameraShake.Shake(_enemy.shakeAmount, _enemy.shakeLength);
        Destroy(_enemy.gameObject);
    }
}
