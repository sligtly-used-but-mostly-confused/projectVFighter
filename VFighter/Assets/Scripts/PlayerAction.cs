using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;

public enum ActionType
{
    ChangeGrav,
    FireGravGun
}

public class PlayerAction {

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public ActionType Action { get; set; }
    public float DirX { get; set; }
    public float DirY { get; set; }

    public PlayerAction(ActionType type, Vector2 dir)
    {
        this.Action = type;
        this.DirX = dir.x;
        this.DirY = dir.y;
    }

    public override string ToString()
    {
        return string.Format("[Id = {0}, Action = {1}, DirX = {2}, DirY = {3}]", Id, Action, DirX, DirY);
    }
}
