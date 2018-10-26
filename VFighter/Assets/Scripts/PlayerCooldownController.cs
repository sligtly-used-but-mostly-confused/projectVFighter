using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System;

public enum CooldownType
{
    NormalShot,
    ShotGunShot,
    Dash,
    Rocket,
    ChangeGravity
}

public class PlayerCooldownController : NetworkBehaviour {
    [System.Serializable]
    public struct CooldownTimePair
    {
        [SerializeField]
        public CooldownType Type;
        [SerializeField]
        public float CooldownTime;
        [SerializeField]
        public bool IsCoolingDown;
    }

    [SerializeField]
    private List<CooldownTimePair> _coolDowns;
    private Dictionary<CooldownType, Coroutine> _coolDownTimers = new Dictionary<CooldownType, Coroutine>();

    [System.Serializable]
    public class SyncListCooldownPairs : SyncListStruct<CooldownTimePair>{}

    public SyncListCooldownPairs _pairs = new SyncListCooldownPairs();

    private void Start()
    {
        Enum.GetValues(typeof(CooldownType)).Cast<CooldownType>().ToList().ForEach(x => _coolDownTimers.Add(x, null));
    }

    public override void OnStartServer()
    {
        _coolDowns.ForEach(x => _pairs.Add(x));
    }

    public bool TryStartCooldown(CooldownType type)
    {
        return TryStartCooldown(type, () => { });
    }

    public bool TryStartCooldown(CooldownType type, Action cb)
    {
        if(IsCoolingDown(type))
        {
            return false;
        }

        StartCooldown(type, cb);
        return true;
    }

    public void StartCooldown(CooldownType type, Action cb)
    {
        var temp = _pairs.ToList().Find(x => x.Type == type);
        int index = _pairs.ToList().IndexOf(temp);

        temp.IsCoolingDown = true;
        _pairs[index] = temp;
        CmdChangeCooldownState(temp);

        _coolDownTimers[type] = StartCoroutine(CooldownInternal(temp.CooldownTime, () => 
        {
            _coolDownTimers[type] = null;
            temp.IsCoolingDown = false;
            _pairs[index] = temp;
            CmdChangeCooldownState(temp);
            cb();
        }));
    }

    public bool IsCoolingDown(CooldownType type)
    {
        var temp = _pairs.ToList().Find(x => x.Type == type);
        return temp.IsCoolingDown;
    }

    public float GetCooldownTime(CooldownType type)
    {
        return _pairs.ToList().Find(x => x.Type == type).CooldownTime;
    }

    public void StopCooldown(CooldownType type)
    {
        var temp = _pairs.ToList().Find(x => x.Type == type);
        int index = _pairs.ToList().IndexOf(temp);
        if(_coolDownTimers[type] != null)
        {
            StopCoroutine(_coolDownTimers[type]);
            _coolDownTimers[type] = null;
        }

        temp.IsCoolingDown = false;
        _pairs[index] = temp;
        CmdChangeCooldownState(temp);
    }

    private IEnumerator CooldownInternal(float time, Action cb)
    {
        yield return new WaitForSeconds(time);
        cb();
    }

    [Command]
    private void CmdChangeCooldownState(CooldownTimePair pair)
    {
        var temp = _pairs.ToList().Find(x => x.Type == pair.Type);
        int index = _pairs.ToList().IndexOf(temp);
        _pairs[index] = pair;
    }
}
