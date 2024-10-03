using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    //public ;

    private void Start()
    {
        PlayerSpawner(PlayerPrefs.GetInt("Player1ID"), PlayerPrefs.GetInt("Player2ID"));
    }

    public void PlayerSpawner(int player1ID, int player2ID)
    {
        GameObject myPlayerPrefab = Resources.Load<GameObject>("Prefabs/DogPBR");
        GameObject enemyPlayerPrefab = Resources.Load<GameObject>("Prefabs/EnemyPlayer");

        Transform player1Spawn = GameObject.Find("PlayerSpawner").transform.Find("Player1Spawner");
        Transform player2Spawn = GameObject.Find("PlayerSpawner").transform.Find("Player2Spawner");

        if(player1ID == NetworkManager.instance.PlayerID)
        {
            GameObject myPlayer = Instantiate(myPlayerPrefab, player1Spawn.position, Quaternion.identity);
            myPlayer.transform.SetParent(player1Spawn);
            myPlayer.AddComponent<MyPlayer>();

            GameObject enemyPlayer = Instantiate(enemyPlayerPrefab, player2Spawn.position, Quaternion.identity);
            enemyPlayer.transform.SetParent(player2Spawn);
            enemyPlayer.AddComponent<EnemyPlayer>();
        }
        else if (player2ID == NetworkManager.instance.PlayerID)
        {
            GameObject myPlayer = Instantiate(myPlayerPrefab, player2Spawn.position, Quaternion.identity);
            myPlayer.transform.SetParent(player2Spawn);
            myPlayer.AddComponent<MyPlayer>();

            GameObject enemyPlayer = Instantiate(enemyPlayerPrefab, player1Spawn.position, Quaternion.identity);
            enemyPlayer.transform.SetParent(player1Spawn);
            enemyPlayer.AddComponent<EnemyPlayer>();
        }
    }
}
