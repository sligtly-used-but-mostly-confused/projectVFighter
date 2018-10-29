using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EndScoreScreenLevelManager : LevelManager {

    protected override void SpawnPlayers()
    {
        var players = FindObjectsOfType<PlayerController>().ToList();


        players.Sort
        ( 
            (x, y) => x.ControlledPlayer.NumRoundWins.CompareTo(y.ControlledPlayer.NumRoundWins) 
        );

        players.ForEach(x => SpawnPlayer(x));
    }

    public override void SpawnPlayer(PlayerController player)
    {
        player.GetComponent<GravityObjectRigidBody>().CanMove = false;
        int index = 0;
        SpawnPosition position = _spawnPositions[index];
        _spawnPositions.RemoveAt(index);
        player.InitializeForStartLevel(position.gameObject.transform.position, false);
    }
}
