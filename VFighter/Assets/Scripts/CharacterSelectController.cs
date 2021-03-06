﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]

public class CharacterSelectController : MonoBehaviour
{

    public GameObject previewPrefab;
    public GameObject currentIconGameObject;
    public GameObject descriptionPrefab;
    public float secondForCharacterTip;

    private readonly List<PlayerCharacterType> CharacterTypes = Enum.GetValues(typeof(PlayerCharacterType)).Cast<PlayerCharacterType>().ToList();

    private GameObject currentIcon;
    [SerializeField]
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
    [SerializeField]
    private List<Material> _gunMaterials= new List<Material>();



    public Dictionary<PlayerCharacterType, List<Material>> CharacterTypeMaterialMappings = new Dictionary<PlayerCharacterType, List<Material>>();
    public Dictionary<PlayerCharacterType, Material> CharacterTypeIconMappings = new Dictionary<PlayerCharacterType, Material>();
    public Dictionary<PlayerCharacterType, string> CharacterTypeDescriptionMappings = new Dictionary<PlayerCharacterType, string>();
    public Dictionary<PlayerCharacterType, GameObject> characterTypeAnimatorGOMappings = new Dictionary<PlayerCharacterType, GameObject>();
    public int currentMaterialIndex = 0;

    public List<Color> PlayerColors;
    public Color CurrentPlayerColor {
        get
        {
            return PlayerColors[currentMaterialIndex];
        }
    }

    private bool _hasFoundReticle = false;
    private float timeOnSelection;
    public Material CurrentPlayerMaterial;

    public delegate void OnCharacterChangedDelegate(PlayerCharacterType type);
    public OnCharacterChangedDelegate OnCharacterChanged;

    public delegate void OnPlayerColorChangedDelegate(Color color);
    public OnPlayerColorChangedDelegate OnPlayerColorChanged;

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
        //characterDataList.ForEach(x => characterTypeCurrentMaterialIndexMappings.Add(x.CharacterType, x.currentMaterialIndex));

        if (LevelManager.Instance.ShowTutorialPrompt == true)
        {
            descriptionCanvas = Instantiate(descriptionPrefab);
            descriptionCanvas.transform.SetParent(transform);
            descriptionCanvas.transform.position = Vector3.down * 2 + new Vector3(0, 0, -3);
        }

        //initialize materials
        foreach(CharacterData cd in characterDataList){
            cd.AnimatorGameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = cd.materials[cd.currentMaterialIndex];
        }

        timeOnSelection = 0;
        CurrentPlayerMaterial = characterTypeAnimatorGOMappings[currentCharacterType].GetComponentInChildren<SkinnedMeshRenderer>().material;
        GameManager.Instance.OnLevelChanged += RefreshCurrentMaterial;
        OnCharacterChanged += (x) => { };
        OnPlayerColorChanged += (x) => { };
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLevelChanged -= RefreshCurrentMaterial;
    }

    void Update()
    {    
        if (!GameManager.Instance.IsInCharacterSelect)
        {
            return;
        }

        timeOnSelection += Time.deltaTime;

        bool ready = playerController.IsReady;

        //set actives for different characters
        characterTypeAnimatorGOMappings[nextCharacterType].SetActive(!ready);
        characterTypeAnimatorGOMappings[previousCharacterType].SetActive(!ready);
        characterTypeAnimatorGOMappings[currentCharacterType].SetActive(true);

        if (LevelManager.Instance.ShowTutorialPrompt == true)
        {
            descriptionCanvas.SetActive(!ready && timeOnSelection > secondForCharacterTip);
        }

        GravityObjectRigidBody rb = GetComponent<GravityObjectRigidBody>();

        if (GetComponent<PlayerController>().InputDevice == null)
        {
            return;
        }

        if (!_hasFoundReticle && GetComponent<PlayerController>().Reticle)
        {
            _hasFoundReticle = true;
            ChangeToNextCharacterType(1);
            ChangeMaterialType(1);
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

        if (LevelManager.Instance.ShowTutorialPrompt == true)
        {
            if (GetComponent<GravityObjectRigidBody>().GravityDirection.y < 0)
            {
                descriptionCanvas.transform.localPosition = Vector3.up * 4 + new Vector3(0, 0, -3);
            }
            else
            {
                descriptionCanvas.transform.localPosition = Vector3.down * 5 + new Vector3(0, 0, -3);
            }
        }

    }

    public void ChangeToNextCharacterType(int dir)
    {
        //reset the player material incase its flashing
        SetCurrentMaterialLossy(CurrentPlayerMaterial);

        //get the indexing right
        int index = CharacterTypes.IndexOf(GetComponent<PlayerController>().CharacterType);
        int indexRight, indexLeft;
        index += dir;
        index = (index + CharacterTypes.Count) % CharacterTypes.Count;
        indexRight = (index - 1 + CharacterTypes.Count) % CharacterTypes.Count;
        indexLeft = (index + 1 + CharacterTypes.Count) % CharacterTypes.Count;

        //set the current character
        currentCharacterType = CharacterTypes[index];

        GetComponent<PlayerController>().CharacterType = currentCharacterType;
        GameObject currentGO = characterTypeAnimatorGOMappings[currentCharacterType];
        CurrentPlayerMaterial = currentGO.GetComponentInChildren<SkinnedMeshRenderer>().material;
        GetComponent<CharacterAnimScript>().currentAnimator = currentGO.GetComponent<Animator>();
        currentGO.transform.localPosition = new Vector3(0, -1.33f, 0);
        currentGO.transform.localScale = new Vector3(5, 5, 5);
        RefreshCurrentMaterial();
        OnCharacterChanged(currentCharacterType);

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

        if (LevelManager.Instance.ShowTutorialPrompt == true)
        {
            //reset selection time and update description
            descriptionCanvas.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetComponent<PlayerController>().CharacterType.ToString();
            descriptionCanvas.transform.GetChild(0).transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = CharacterTypeDescriptionMappings[GetComponent<PlayerController>().CharacterType];
            timeOnSelection = 0;
            descriptionCanvas.SetActive(false);
        }
    }
    
    private void ChangeMaterialType(int dir)
    {
        //reset the player material incase its flashing
        SetCurrentMaterialLossy(CurrentPlayerMaterial);

        //get a list of all currently active characterselectcontroller
        List<CharacterSelectController> characterSelectControllers = FindObjectsOfType<CharacterSelectController>().ToList();

        //find remaining available materials, based color regardless of what character type;
        List<int> availableMaterialIndexes = new List<int>() { 0, 1, 2, 3 };
        List<int> takenMaterialIndexes = new List<int>();
        foreach(CharacterSelectController c in characterSelectControllers)
        {
            takenMaterialIndexes.Add(c.currentMaterialIndex);
        }

        takenMaterialIndexes.Remove(currentMaterialIndex);

        availableMaterialIndexes.RemoveAll(x => takenMaterialIndexes.Contains(x));

        //find the next available material
        int currentIndexInAvailable = availableMaterialIndexes.IndexOf(currentMaterialIndex);
        currentIndexInAvailable += dir;
        currentIndexInAvailable = (currentIndexInAvailable + availableMaterialIndexes.Count) % availableMaterialIndexes.Count;
        currentMaterialIndex = availableMaterialIndexes[currentIndexInAvailable];

        RefreshCurrentMaterial();
    }

    public void RefreshCurrentMaterial()
    {
        OnCharacterChanged(currentCharacterType);

        //get the indexing right
        int index = CharacterTypes.IndexOf(GetComponent<PlayerController>().CharacterType);
        int indexRight, indexLeft;
        index = (index + CharacterTypes.Count) % CharacterTypes.Count;
        indexRight = (index - 1 + CharacterTypes.Count) % CharacterTypes.Count;
        indexLeft = (index + 1 + CharacterTypes.Count) % CharacterTypes.Count;

        nextCharacterType = CharacterTypes[indexRight];
        GameObject nextGO = characterTypeAnimatorGOMappings[nextCharacterType];
        List<Material> nextMaterialOptions = CharacterTypeMaterialMappings[nextCharacterType];
        characterTypeAnimatorGOMappings[nextCharacterType].GetComponentInChildren<SkinnedMeshRenderer>().material = nextMaterialOptions[currentMaterialIndex];
        characterTypeAnimatorGOMappings[nextCharacterType].GetComponentInChildren<MeshRenderer>().material = _gunMaterials[currentMaterialIndex];

        previousCharacterType = CharacterTypes[indexLeft];
        GameObject previousGO = characterTypeAnimatorGOMappings[previousCharacterType];
        List<Material> prevMaterialOptions = CharacterTypeMaterialMappings[previousCharacterType];
        characterTypeAnimatorGOMappings[previousCharacterType].GetComponentInChildren<SkinnedMeshRenderer>().material = prevMaterialOptions[currentMaterialIndex];
        characterTypeAnimatorGOMappings[previousCharacterType].GetComponentInChildren<MeshRenderer>().material = _gunMaterials[currentMaterialIndex];

        List<Material> currentMaterialOptions = CharacterTypeMaterialMappings[currentCharacterType];
        characterTypeAnimatorGOMappings[currentCharacterType].GetComponentInChildren<MeshRenderer>().material = _gunMaterials[currentMaterialIndex];
        SetCurrentMaterial(currentMaterialOptions[currentMaterialIndex]);
        OnPlayerColorChanged(CurrentPlayerColor);
    }

    public Material GetCurrentPlayerMaterial()
    {
        return CurrentPlayerMaterial;
    }

    public Material GetCurrentPlayerRenderingMaterial()
    {
        return characterTypeAnimatorGOMappings[currentCharacterType].GetComponentInChildren<SkinnedMeshRenderer>().material;
    }

    public void SetCurrentMaterial(Material mat)
    {
        CurrentPlayerMaterial = mat;
        SetCurrentMaterialLossy(mat);
    }

    //use this when you only want to change the material temporaraly
    public void SetCurrentMaterialLossy(Material mat)
    {
        //set the material to the decided up index
        characterTypeAnimatorGOMappings[currentCharacterType].GetComponentInChildren<SkinnedMeshRenderer>().material = mat;
    }

    public void ResetToCurrentMaterial()
    {
        SetCurrentMaterialLossy(CurrentPlayerMaterial);
    }
}
