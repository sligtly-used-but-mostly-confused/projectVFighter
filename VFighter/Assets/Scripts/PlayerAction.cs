using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ActionType
{
    ChangeGrav,
    FireGravGun
}

public class PlayerAction {
    public int Id { get; set; }
    public ActionType Action { get; set; }

    public string Dir { get; set; }

    public string Pos { get; set; }

    public string OtherPlayers { get; set; }

    public string GravityObjects { get; set; }

    public PlayerAction(ActionType type, Vector2 dir, Vector2 pos,
        List<PlayerController> otherPlayers, List<GravityObjectRigidBody> otherObjects)
    {
        Action = type;
        Dir = JsonUtility.ToJson(dir);
        Pos = JsonUtility.ToJson(pos);
        OtherPlayers = JsonUtility.ToJson(OtherPlayers);
    }

    public override string ToString()
    {
        return string.Format("[Id = {0}, Action = {1}, Dir = {2}, Pos = {3}]", Id, Action, Dir, Pos);
    }
}
