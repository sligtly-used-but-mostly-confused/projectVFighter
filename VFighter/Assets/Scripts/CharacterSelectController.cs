using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class CharacterSelectController : NetworkBehaviour {
    private readonly List<PlayerCharacterType> CharacterTypes = Enum.GetValues(typeof(PlayerCharacterType)).Cast<PlayerCharacterType>().ToList();

    [System.Serializable]
    public struct CaracterTypeMaterialMap
    {
        public Material Material;
        public PlayerCharacterType CharacterType;
    }
    
    public Dictionary<PlayerCharacterType, Material> CharacterTypeMaterialMappings = new Dictionary<PlayerCharacterType, Material>();

    [SerializeField]
    private List<CaracterTypeMaterialMap> CharacterTypeMaterialMappingsInternal = new List<CaracterTypeMaterialMap>();

    private bool _hasFoundReticle = false;

    // Show Character Select button mapping images
    public InputDevice Controller;
    public List<TutorialPrompt> Prompts;

    [System.Serializable]
    public struct TutorialPrompt
    {
        [SerializeField]
        public bool IsButton;
        [SerializeField]
        public MappedButton MappedButton;
        [SerializeField]
        public MappedAxis MappedAxis;
    }

    private void Awake()
    {
        Controller = MappedInput.InputDevices[3];
        //GetComponent<Image>().sprite = GetSpriteFromPrompt(Prompts[0]);

        CharacterTypeMaterialMappingsInternal.ForEach(x => CharacterTypeMaterialMappings.Add(x.CharacterType, x.Material));
    }

    void Update() {
        if (Prompts[0].MappedButton != MappedButton.None)
        {
            if (Controller.GetButton(Prompts[0].MappedButton))
            {
                GetComponent<Image>().sprite = GetSpriteFromPrompt(Prompts[1]);
                Prompts.RemoveAt(0);
            }
        }

        if (Prompts[0].MappedAxis != MappedAxis.None)
        {
            if (Controller.GetIsAxisTapped(Prompts[0].MappedAxis))
            {
                GetComponent<Image>().sprite = GetSpriteFromPrompt(Prompts[1]);
                Prompts.RemoveAt(0);
            }
        }

        if (GetComponent<PlayerController>().InputDevice == null)
        {
            return;
        }
        
        if(!_hasFoundReticle && GetComponent<PlayerController>().Reticle)
        {
            _hasFoundReticle = true;
            ChangeToNextCharacterType(1);
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

    private Sprite GetSpriteFromPrompt(TutorialPrompt prompt)
    {
        Sprite icon = null;

        if (prompt.MappedButton != MappedButton.None)
        {
            icon = Controller.GetButtonIcon(prompt.MappedButton);
        }

        if (prompt.MappedAxis != MappedAxis.None && icon == null)
        {
            icon = Controller.GetAxisIcon(prompt.MappedAxis);
        }

        return icon;
    }
}
