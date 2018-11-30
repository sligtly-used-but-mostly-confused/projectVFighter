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
    public class CooldownTimePair
    {
        [SerializeField]
        public CooldownType Type;
        [SerializeField]
        public float CooldownTime;
        [SerializeField]
        public bool IsCoolingDown;
        [SerializeField]
        public Material FlashingMaterial;
        [SerializeField]
        public float TimeLeft;
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
    private CooldownTimePair _currentlyFlashingCooldown;


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

    public void OnDisable()
    {
    #if !UNITY_EDITOR
                CancelInvoke();
                StopAllCoroutines();
    #endif
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

        temp.IsCoolingDown = true;

        if(_flashCoroutine != null)
        {
            //theres already a flasher going so we need to check which one will stop first
            if(_currentlyFlashingCooldown.TimeLeft < temp.CooldownTime)
            {
                characterSelect.ResetToCurrentMaterial();
                StopCoroutine(_flashCoroutine);
                StartFlasher(temp);
            }
        }
        else
        {
            //no current flasher so we can just start
            StartFlasher(temp);
        }

        var coolDown = new CooldownCoroutineCallbackPair();
        coolDown.Callback = cb;
        _coolDownTimers[type] = coolDown;
        coolDown.CooldownTimer = StartCoroutine(CooldownInternal(type, temp.CooldownTime, cb));
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
        if(_coolDownTimers[type] != null)
        {
            StopCoroutine(_coolDownTimers[type].CooldownTimer);
            _coolDownTimers[type].Callback();
            _coolDownTimers[type] = null;
        }

        temp.IsCoolingDown = false;
    }

    private IEnumerator CooldownInternal(CooldownType type, float time, Action cb)
    {
        var temp = _coolDowns.Find(x => x.Type == type);

        var timeRemaining = time;

        while(timeRemaining > 0)
        {
            yield return new WaitForEndOfFrame();
            timeRemaining -= Time.deltaTime * GameManager.Instance.TimeScale;
            temp.TimeLeft = timeRemaining;
        }
        
        _coolDownTimers[type] = null;
        temp.IsCoolingDown = false;
        cb();
    }

    public void StartFlasher(CooldownTimePair cooldown)
    {
        _flashCoroutine = StartCoroutine(FlashRenderer(cooldown.CooldownTime, cooldown.FlashingMaterial));
        _currentlyFlashingCooldown = cooldown;
    }

    public IEnumerator FlashRenderer(float duration, Material OtherMaterialState)
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
