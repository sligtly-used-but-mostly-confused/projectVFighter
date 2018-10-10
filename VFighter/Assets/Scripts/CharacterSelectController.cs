using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerController))]
public class CharacterSelectController : NetworkBehaviour {
    private readonly List<PlayerCharacterType> CharacterTypes = Enum.GetValues(typeof(PlayerCharacterType)).Cast<PlayerCharacterType>().ToList();

    [System.Serializable]
    public struct CaracterTypeMaterialMap
    {
        public Material Material;
        public PlayerCharacterType CharacterType;
    }

    [SerializeField]
    private List<CaracterTypeMaterialMap> CharacterTypeMaterialMappingsInternal = new List<CaracterTypeMaterialMap>();

    public Dictionary<PlayerCharacterType, Material> CharacterTypeMaterialMappings = new Dictionary<PlayerCharacterType, Material>();

    private void Awake()
    {
        CharacterTypeMaterialMappingsInternal.ForEach(x => CharacterTypeMaterialMappings.Add(x.CharacterType, x.Material));
    }
    
    void Update() {
        if (GetComponent<PlayerController>().InputDevice == null)
        {
            return;
        }
        
        if(GetComponent<PlayerController>().InputDevice.GetButtonDown(MappedButton.SubmitCharacterChoice))
        {
            GetComponent<GravityObjectRigidBody>().CanMove = true;
            Destroy(this);
        }

        if(GetComponent<PlayerController>().InputDevice.GetIsAxisTapped(MappedAxis.ChangeCharacter))
        {
            float ChangeCharacterDir = GetComponent<PlayerController>().InputDevice.GetAxis(MappedAxis.ChangeCharacter);
            ChangeToNextCharacterType(ChangeCharacterDir > 0 ? 1 : -1);
        }
    }

    public void ChangeToNextCharacterType(int dir)
    {
        CmdChangeToNextCharacterType(dir);
    }

    [Command]
    public void CmdChangeToNextCharacterType(int dir)
    {
        ChangeToNextCharacterTypeInternal(dir);
    }

    private void ChangeToNextCharacterTypeInternal(int dir)
    {
        Debug.Log(dir);
        int index = CharacterTypes.IndexOf(GetComponent<PlayerController>().CharacterType);
        index += dir;
        index = (index + CharacterTypes.Count) % CharacterTypes.Count;
        GetComponent<PlayerController>().CharacterType = CharacterTypes[index];
        GetComponent<PlayerController>().ChangeMaterial(GetComponent<PlayerController>().CharacterType);
    }
}
