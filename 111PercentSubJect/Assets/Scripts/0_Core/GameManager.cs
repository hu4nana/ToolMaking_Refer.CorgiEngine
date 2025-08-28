using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("SpawnPosition")]
    public Transform playerSpawnPosition;
    public Transform[] enemySpawnPosition;

    public GameObject Player;

    Health p_Health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Initialized()
    {
        p_Health = Player.GetComponent<Health>();
    }

    public void GameOver()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
