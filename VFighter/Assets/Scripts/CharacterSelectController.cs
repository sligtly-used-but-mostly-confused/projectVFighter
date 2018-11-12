﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]

public class CharacterSelectController : NetworkBehaviour {

    public GameObject previewPrefab;
    public GameObject currentIconGameObject;
    public GameObject descriptionPrefab;
    public float secondForCharacterTip;

    private readonly List<PlayerCharacterType> CharacterTypes = Enum.GetValues(typeof(PlayerCharacterType)).Cast<PlayerCharacterType>().ToList();

    private GameObject currentIcon;
    private GameObject descriptionCanvas;
    private PlayerCharacterType currentCharacterType;
    private PlayerCharacterType nextCharacterType;
    private PlayerCharacterType previousCharacterType;

    private PlayerController playerController;

    [System.Serializable]
    public struct CharacterData
    {
        public Material IconMaterial;
        public string description;
        public PlayerCharacterType CharacterType;
        public GameObject AnimatorGameObject;
        public List<Material> materials;
        public int currentMaterialIndex;
    }

    [SerializeField]
    private List<CharacterData> characterDataList = new List<CharacterData>();

    public Dictionary<PlayerCharacterType, List<Material>> CharacterTypeMaterialMappings = new Dictionary<PlayerCharacterType, List<Material>>();
    public Dictionary<PlayerCharacterType, Material> CharacterTypeIconMappings = new Dictionary<PlayerCharacterType, Material>();
    public Dictionary<PlayerCharacterType, string> CharacterTypeDescriptionMappings = new Dictionary<PlayerCharacterType, string>();
    public Dictionary<PlayerCharacterType, GameObject> characterTypeAnimatorGOMappings = new Dictionary<PlayerCharacterType, GameObject>();
    public Dictionary<PlayerCharacterType, int> characterTypeCurrentMaterialIndexMappings = new Dictionary<PlayerCharacterType, int>();

    private bool _hasFoundReticle = false;
    private float timeOnSelection;

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
        playerController = GetComponent<PlayerController>();

        characterDataList.ForEach(x => CharacterTypeMaterialMappings.Add(x.CharacterType, x.materials));
        characterDataList.ForEach(x => CharacterTypeIconMappings.Add(x.CharacterType, x.IconMaterial));
        characterDataList.ForEach(x => CharacterTypeDescriptionMappings.Add(x.CharacterType, x.description));
        characterDataList.ForEach(x => characterTypeAnimatorGOMappings.Add(x.CharacterType, x.AnimatorGameObject));
        characterDataList.ForEach(x => characterTypeCurrentMaterialIndexMappings.Add(x.CharacterType, x.currentMaterialIndex));

        descriptionCanvas = Instantiate(descriptionPrefab);
        descriptionCanvas.transform.SetParent(transform);
        descriptionCanvas.transform.position = Vector3.down * 2 + new Vector3(0, 0, -3);

        ChangeToNextCharacterTypeInternal(0);

        //initialize materials
        foreach(CharacterData cd in characterDataList){
            cd.AnimatorGameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = cd.materials[cd.currentMaterialIndex];
        }
        
        timeOnSelection = 0;
    }

    void Update() {
        
        if (!GameManager.Instance.CanChangeCharacters)
        {
            return;
        }

        timeOnSelection += Time.deltaTime;

        bool ready = playerController.IsReady;

        //set actives for different characters
        characterTypeAnimatorGOMappings[nextCharacterType].SetActive(!ready);
        characterTypeAnimatorGOMappings[previousCharacterType].SetActive(!ready);
        characterTypeAnimatorGOMappings[currentCharacterType].SetActive(true);

        descriptionCanvas.SetActive(!ready && timeOnSelection > secondForCharacterTip); 

        GravityObjectRigidBody rb = GetComponent<GravityObjectRigidBody>();
        rb.CanMove = ready;

        if(!ready){
            rb.ClearAllVelocities();
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

        if (GetComponent<PlayerController>().InputDevice == null)
        {
            return;
        }

        if (!_hasFoundReticle && GetComponent<PlayerController>().Reticle)
        {
            _hasFoundReticle = true;
            ChangeToNextCharacterType(1);
        }

        if(GetComponent<PlayerController>().InputDevice.GetButtonDown(MappedButton.SubmitCharacterChoice))
        {
            GetComponent<GravityObjectRigidBody>().CanMove = true;
        }

        if(GetComponent<PlayerController>().InputDevice.GetIsAxisTapped(MappedAxis.ChangeCharacter))
        {
            float ChangeCharacterDir = GetComponent<PlayerController>().InputDevice.GetAxis(MappedAxis.ChangeCharacter);
            ChangeToNextCharacterType(ChangeCharacterDir > 0 ? 1 : -1);
        }

        if (GetComponent<PlayerController>().InputDevice.GetIsAxisTapped(MappedAxis.ChangeMaterial))
        {
            float ChangeMaterialDir = GetComponent<PlayerController>().InputDevice.GetAxis(MappedAxis.ChangeMaterial);
            ChangeMaterialType(ChangeMaterialDir > 0 ? 1 : -1);
        }

        if (GetComponent<GravityObjectRigidBody>().GravityDirection.y < 0)
        {
            descriptionCanvas.transform.localPosition = Vector3.up * 4 + new Vector3(0, 0, -3);
        }
        else
        {
            descriptionCanvas.transform.localPosition = Vector3.down * 5 + new Vector3(0, 0, -3);
        }

    }

    public void ChangeToNextCharacterType(int dir)
    {
        if(!playerController.IsReady)
            CmdChangeToNextCharacterType(dir);
    }

    [Command]
    public void CmdChangeToNextCharacterType(int dir)
    {
        ChangeToNextCharacterTypeInternal(dir);
    }

    private void ChangeToNextCharacterTypeInternal(int dir)
    {
        //get the indexing right
        int index = CharacterTypes.IndexOf(GetComponent<PlayerController>().CharacterType);
        int indexRight, indexLeft;
        index += dir;
        index = (index + CharacterTypes.Count) % CharacterTypes.Count;
        indexRight = (index + 1 + CharacterTypes.Count) % CharacterTypes.Count;
        indexLeft  = (index - 1 + CharacterTypes.Count) % CharacterTypes.Count;

        //set the current character
        currentCharacterType = CharacterTypes[index];
        GetComponent<PlayerController>().CharacterType = currentCharacterType;
        GameObject currentGO = characterTypeAnimatorGOMappings[currentCharacterType];
        GetComponent<CharacterAnimScript>().currentAnimator = currentGO.GetComponent<Animator>();
        currentGO.transform.localPosition = new Vector3(0, -1.33f, 0);
        currentGO.transform.localScale = new Vector3(5, 5, 5);


        //set the right character preview
        nextCharacterType = CharacterTypes[indexRight];
        GameObject nextGO = characterTypeAnimatorGOMappings[nextCharacterType];
        nextGO.transform.localPosition = new Vector3(1.5f, -1.33f, 0);
        nextGO.transform.localScale = new Vector3(5f, 5f, 5f);

        //set the left character preview
        previousCharacterType = CharacterTypes[indexLeft];
        GameObject previousGO = characterTypeAnimatorGOMappings[previousCharacterType];
        previousGO.transform.localPosition = new Vector3(-1.5f, -1.33f, 0);
        previousGO.transform.localScale = new Vector3(5f, 5f, 5f);

        //reset selection time and update description
        descriptionCanvas.transform.GetChild(0).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = CharacterTypeDescriptionMappings[GetComponent<PlayerController>().CharacterType];
        timeOnSelection = 0;
        descriptionCanvas.SetActive(false);
    }

    private void ChangeMaterialType(int dir){
        List<Material> currentMaterialOptions = CharacterTypeMaterialMappings[currentCharacterType];
        characterTypeCurrentMaterialIndexMappings[currentCharacterType] = (characterTypeCurrentMaterialIndexMappings[currentCharacterType] + dir + currentMaterialOptions.Count) % currentMaterialOptions.Count;
        characterTypeAnimatorGOMappings[currentCharacterType].GetComponentInChildren<SkinnedMeshRenderer>().material = currentMaterialOptions[characterTypeCurrentMaterialIndexMappings[currentCharacterType]];
    }

}
