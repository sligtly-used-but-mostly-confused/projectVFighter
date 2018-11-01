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

    public class CooldownCoroutineCallbackPair
    {
        public Coroutine CooldownTimer;
        public Action Callback;
    }

    [SerializeField]
    private List<CooldownTimePair> _coolDowns;
    private Dictionary<CooldownType, CooldownCoroutineCallbackPair> _coolDownTimers = new Dictionary<CooldownType, CooldownCoroutineCallbackPair>();

    [SerializeField]
    private float cooldownFlashInterval;

    [System.Serializable]
    public class SyncListCooldownPairs : SyncListStruct<CooldownTimePair>{}

    public SyncListCooldownPairs _pairs = new SyncListCooldownPairs();

    private Coroutine _flashCoroutine;
    private Color _defaultColor;
    private void Start()
    {
        Enum.GetValues(typeof(CooldownType)).Cast<CooldownType>().ToList().ForEach(x => _coolDownTimers.Add(x, null));
        _flashCoroutine = null;
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
        if(_flashCoroutine != null)
        {
            GetComponent<Renderer>().material.color = _defaultColor;
            StopCoroutine(_flashCoroutine);
        }

        var coolDown = new CooldownCoroutineCallbackPair();

        coolDown.CooldownTimer = StartCoroutine(CooldownInternal(type, temp.CooldownTime, cb));
        _flashCoroutine = StartCoroutine(FlashRenderer(cooldownFlashInterval, temp.CooldownTime));
        coolDown.Callback = cb;
        _coolDownTimers[type] = coolDown;
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
            StopCoroutine(_coolDownTimers[type].CooldownTimer);
            _coolDownTimers[type].Callback();
            _coolDownTimers[type] = null;
        }

        temp.IsCoolingDown = false;
        _pairs[index] = temp;
        CmdChangeCooldownState(temp);
    }

    private IEnumerator CooldownInternal(CooldownType type, float time, Action cb)
    {
        var temp = _pairs.ToList().Find(x => x.Type == type);
        int index = _pairs.ToList().IndexOf(temp);
        yield return new WaitForSeconds(time);
        _coolDownTimers[type] = null;
        temp.IsCoolingDown = false;
        _pairs[index] = temp;
        CmdChangeCooldownState(temp);
        cb();
    }

    [Command]
    private void CmdChangeCooldownState(CooldownTimePair pair)
    {
        var temp = _pairs.ToList().Find(x => x.Type == pair.Type);
        int index = _pairs.ToList().IndexOf(temp);
        _pairs[index] = pair;
    }

    public IEnumerator FlashRenderer(float interval, float duration)
    {
        var renderer = GetComponent<Renderer>();
        _defaultColor = renderer.material.color;
        Color minColor = _defaultColor;
        Color maxColor = new Color(1f,1f,1f,1f);

        float currentInterval = 0;
        while (duration > 0)
        {
            float tColor = currentInterval / interval;
            renderer.material.color = Color.Lerp(_defaultColor, maxColor, tColor);

            currentInterval += Time.deltaTime;
            if (currentInterval >= interval)
            {
                Color temp = minColor;
                minColor = maxColor;
                maxColor = temp;
                currentInterval = currentInterval - interval;
            }
            duration -= Time.deltaTime;
            yield return null;
        }

        renderer.material.color = _defaultColor;
    }

}
