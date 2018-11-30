using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatisticsUIController : MonoBehaviour
{
    public GameObject MainStatsPanel;
    public GameObject OtherStatsPanel;
    public GameObject PlayerStatFieldPrefab;

    public List<GameObject> _currentFields;

    public void Init(Player player)
    {
        _currentFields.ForEach(x => DestroyImmediate(x));
        _currentFields.Clear();


        int numWins = player.NumStageWins;
        int numKills = player.NumKills;
        int numDeaths = player.NumDeaths;
        List<Tuple<string, string>> mainStatVals = new List<Tuple<string, string>>();
        mainStatVals.Add(new Tuple<string, string>("Wins", numWins.ToString()));
        mainStatVals.Add(new Tuple<string, string>("Kills", numKills.ToString()));
        mainStatVals.Add(new Tuple<string, string>("Deaths", numDeaths.ToString()));

        _currentFields = new List<GameObject>();

        foreach (var stat in mainStatVals)
        {
            var field = Instantiate(PlayerStatFieldPrefab);
            field.transform.SetParent(MainStatsPanel.transform, false);
            field.GetComponent<PlayerStatisticsFieldUIController>().DisplayField(stat.Item1, stat.Item2);
            _currentFields.Add(field);
        }

        List<Tuple<string, string>> otherStatsVals = new List<Tuple<string, string>>();
        otherStatsVals.Add(new Tuple<string, string>("Shots Fired ", player.ShotsFired.ToString()));
        otherStatsVals.Add(new Tuple<string, string>("Specials Fired", player.SpecialsFired.ToString()));
        otherStatsVals.Add(new Tuple<string, string>("Gravity Changes", player.GravityChanges.ToString()));

        foreach (var stat in otherStatsVals)
        {
            var field = Instantiate(PlayerStatFieldPrefab);
            field.transform.SetParent(OtherStatsPanel.transform, false);
            field.GetComponent<PlayerStatisticsFieldUIController>().DisplayField(stat.Item1, stat.Item2);
            _currentFields.Add(field);
        }
    }
}
