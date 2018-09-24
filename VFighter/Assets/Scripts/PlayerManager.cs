using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    public List<Player> Players = new List<Player>();

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
