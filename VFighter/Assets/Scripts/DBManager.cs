using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBManager : MonoBehaviour {

    public bool IsLoggingToDB = true;
	void Update () {
        DataService.IsLogging = IsLoggingToDB;
	}
}
