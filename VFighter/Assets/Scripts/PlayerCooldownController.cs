﻿using System.Collections;
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
    ShotgunKnockback
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
    private CharacterSelectController characterSelect;

    private void Start()
    {
        Enum.GetValues(typeof(CooldownType)).Cast<CooldownType>().ToList().ForEach(x => _coolDownTimers.Add(x, null));
        _flashCoroutine = null;
        characterSelect = GetComponent<CharacterSelectController>();
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
        _flashCoroutine = StartCoroutine(FlashRenderer(cooldownFlashInterval, temp.CooldownTime));
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

    public IEnumerator FlashRenderer(float interval, float duration)
    {
        characterSelect.SetCurrentMaterialLossy(FlashingMaterial);
        Color minColor = Color.black;
        Color maxColor = new Color(1f,1f,1f,1f);

        float currentInterval = 0;
        while (duration > 0)
        {
            float tColor = currentInterval / interval;
            FlashingMaterial.color = Color.Lerp(minColor, maxColor, tColor);
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
        
        characterSelect.ResetToCurrentMaterial();
    }
}
