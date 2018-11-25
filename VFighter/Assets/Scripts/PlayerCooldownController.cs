using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum CooldownType
{
    NormalShot,
    ShotGunShot,
    Dash,
    DashRecharge,
    Rocket,
    ChangeGravity,
    Invincibility,
    ShotgunKnockback,
    Death
}

public class PlayerCooldownController : MonoBehaviour
{
    [System.Serializable]
    public struct CooldownTimePair
    {
        [SerializeField]
        public CooldownType Type;
        [SerializeField]
        public float CooldownTime;
        [SerializeField]
        public bool IsCoolingDown;
        [SerializeField]
        public Material FlashingMaterial;
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
    
    private Coroutine _flashCoroutine;
    
    [SerializeField]
    private Material FlashingMaterial;

    private Material _flashingMaterialCopy;

    private CharacterSelectController characterSelect;

    private void Start()
    {
        Enum.GetValues(typeof(CooldownType)).Cast<CooldownType>().ToList().ForEach(x => _coolDownTimers.Add(x, null));
        _flashCoroutine = null;
        characterSelect = GetComponent<CharacterSelectController>();
        _flashingMaterialCopy = new Material(FlashingMaterial);
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
        var temp = _coolDowns.Find(x => x.Type == type);
        int index = _coolDowns.IndexOf(temp);

        temp.IsCoolingDown = true;
        _coolDowns[index] = temp;
        ChangeCooldownState(temp);
        if(_flashCoroutine != null)
        {
            characterSelect.ResetToCurrentMaterial();
            StopCoroutine(_flashCoroutine);
        }

        var coolDown = new CooldownCoroutineCallbackPair();

        coolDown.CooldownTimer = StartCoroutine(CooldownInternal(type, temp.CooldownTime, cb));
        _flashCoroutine = StartCoroutine(FlashRenderer(cooldownFlashInterval, temp.CooldownTime, _coolDowns[index].FlashingMaterial));
        coolDown.Callback = cb;
        _coolDownTimers[type] = coolDown;
    }

    public bool IsCoolingDown(CooldownType type)
    {
        var temp = _coolDowns.Find(x => x.Type == type);
        return temp.IsCoolingDown;
    }

    public float GetCooldownTime(CooldownType type)
    {
        return _coolDowns.Find(x => x.Type == type).CooldownTime;
    }

    public void StopCooldown(CooldownType type)
    {
        var temp = _coolDowns.Find(x => x.Type == type);
        int index = _coolDowns.IndexOf(temp);
        if(_coolDownTimers[type] != null)
        {
            StopCoroutine(_coolDownTimers[type].CooldownTimer);
            _coolDownTimers[type].Callback();
            _coolDownTimers[type] = null;
        }

        temp.IsCoolingDown = false;
        _coolDowns[index] = temp;
        ChangeCooldownState(temp);
    }

    private IEnumerator CooldownInternal(CooldownType type, float time, Action cb)
    {
        var temp = _coolDowns.Find(x => x.Type == type);
        int index = _coolDowns.IndexOf(temp);
        yield return new WaitForSeconds(time);
        _coolDownTimers[type] = null;
        temp.IsCoolingDown = false;
        _coolDowns[index] = temp;
        ChangeCooldownState(temp);
        cb();
    }
    
    private void ChangeCooldownState(CooldownTimePair pair)
    {
        var temp = _coolDowns.Find(x => x.Type == pair.Type);
        int index = _coolDowns.IndexOf(temp);
        _coolDowns[index] = pair;
    }

    public IEnumerator FlashRenderer(float interval, float duration, Material OtherMaterialState)
    {
        Material playerMaterial = characterSelect.GetCurrentPlayerMaterial();
        characterSelect.SetCurrentMaterialLossy(_flashingMaterialCopy);
        Color minColor = Color.black;
        Color maxColor = OtherMaterialState.GetColor("_Color");

        Color minEmission = Color.black;
        Color maxEmission = OtherMaterialState.GetColor("_EmissionColor");

        while (duration > 0)
        {
            float lerp = Mathf.PingPong(Time.time, duration) / duration;
            _flashingMaterialCopy.SetColor("_Color", Color.Lerp(minColor, maxColor, lerp));
            _flashingMaterialCopy.SetColor("_EmissionColor", Color.Lerp(minEmission, maxEmission, lerp));

            duration -= Time.deltaTime;
            yield return null;
        }

        _flashingMaterialCopy.CopyPropertiesFromMaterial(FlashingMaterial);
        characterSelect.ResetToCurrentMaterial();
    }
}
