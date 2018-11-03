using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EndScoreScreenLevelManager : LevelManager {

    [SerializeField]
    private GameObject _playerReadyIndicatorPrefab;

    public override void Update()
    {
        if (isServer && !GameManager.Instance.CurrentlyChangingScenes)
        {
            var allPlayersReady = Players.All(x => x.IsReady);
            if(allPlayersReady)
            {
                GameManager.Instance.EndGame();
            }
        }
    }

    protected override void SpawnPlayers()
    {
        Debug.Log("here1");
        var players = FindObjectsOfType<PlayerController>().ToList();

        players.Sort
        ( 
            (x, y) => x.ControlledPlayer.NumStageWins.CompareTo(y.ControlledPlayer.NumStageWins) 
        );
        List<SpawnPosition> spawnPositionsCopy = new List<SpawnPosition>(_spawnPositions);
        players.ForEach(x => SpawnPlayer(x, spawnPositionsCopy));
    }

    public override void SpawnPlayer(PlayerController player, List<SpawnPosition> spawnPositions)
    {
        var indicator = Instantiate(_playerReadyIndicatorPrefab);
        indicator.GetComponent<PlayerReadyIndicatorController>().AttachedPlayer = player;
        int index = 0;
        SpawnPosition position = spawnPositions[index];
        Debug.Log(position);
        spawnPositions.RemoveAt(index);
        player.InitializeForStartLevel(position.gameObject.transform.position, false);
        player.IsReady = false;
        player.GetComponent<GravityObjectRigidBody>().CanMove = false;
    }
}
