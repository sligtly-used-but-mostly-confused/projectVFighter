using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerController))]

public class CharacterSelectController : NetworkBehaviour {

    public GameObject previewPrefab;
    public GameObject currentIconGameObject;

    private readonly List<PlayerCharacterType> CharacterTypes = Enum.GetValues(typeof(PlayerCharacterType)).Cast<PlayerCharacterType>().ToList();

    private GameObject previousCharacterPreview;
    private GameObject nextCharacterPreview;
    private GameObject currentIcon;

    private PlayerController playerController;

    [System.Serializable]
    public struct CaracterTypeMaterialMap
    {
        public Material Material;
        public PlayerCharacterType CharacterType;
    }

    [SerializeField]
    private List<CaracterTypeMaterialMap> CharacterTypeMaterialMappingsInternal = new List<CaracterTypeMaterialMap>();
    [SerializeField]
    private List<CaracterTypeMaterialMap> CharacterTypeIconMappingsInternal = new List<CaracterTypeMaterialMap>();

    public Dictionary<PlayerCharacterType, Material> CharacterTypeMaterialMappings = new Dictionary<PlayerCharacterType, Material>();
    public Dictionary<PlayerCharacterType, Material> CharacterTypeIconMappings = new Dictionary<PlayerCharacterType, Material>();

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        CharacterTypeMaterialMappingsInternal.ForEach(x => CharacterTypeMaterialMappings.Add(x.CharacterType, x.Material));
        CharacterTypeIconMappingsInternal.ForEach(x => CharacterTypeIconMappings.Add(x.CharacterType, x.Material));

        previousCharacterPreview = Instantiate(previewPrefab);
        previousCharacterPreview.transform.SetParent(transform);
        previousCharacterPreview.transform.position = Vector3.left;

        nextCharacterPreview = Instantiate(previewPrefab);
        nextCharacterPreview.transform.SetParent(transform);
        nextCharacterPreview.transform.position = Vector3.right;

        currentIcon = Instantiate(currentIconGameObject);
        currentIcon.transform.SetParent(transform);
        currentIcon.transform.position = Vector3.down;

        ChangeToNextCharacterTypeInternal(0);
    }

    void Update() {

        bool ready = playerController.IsReady;

        nextCharacterPreview.SetActive(!ready);
        previousCharacterPreview.SetActive(!ready);
        currentIcon.SetActive(!ready);

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
        
        if(GetComponent<PlayerController>().InputDevice.GetButtonDown(MappedButton.SubmitCharacterChoice))
        {
            GetComponent<GravityObjectRigidBody>().CanMove = true;
        }

        if(GetComponent<PlayerController>().InputDevice.GetIsAxisTapped(MappedAxis.ChangeCharacter))
        {
            float ChangeCharacterDir = GetComponent<PlayerController>().InputDevice.GetAxis(MappedAxis.ChangeCharacter);
            ChangeToNextCharacterType(ChangeCharacterDir > 0 ? 1 : -1);
        }


    }

    public void ChangeToNextCharacterType(int dir)
    {
        if(!playerController.IsReady)CmdChangeToNextCharacterType(dir);
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

        //set the current prefab material
        GetComponent<PlayerController>().CharacterType = CharacterTypes[index];
        GetComponent<PlayerController>().ChangeMaterial(GetComponent<PlayerController>().CharacterType);
        currentIcon.GetComponent<Renderer>().material = CharacterTypeIconMappings[GetComponent<PlayerController>().CharacterType];

        //set the right character preview
        PlayerCharacterType nextCharacterType = CharacterTypes[indexRight];
        nextCharacterPreview.GetComponent<Renderer>().material = CharacterTypeMaterialMappings[nextCharacterType];
        nextCharacterPreview.transform.GetChild(0).GetComponent<Renderer>().material = CharacterTypeIconMappings[nextCharacterType];

        //set the left character preview
        PlayerCharacterType previousCharacterType = CharacterTypes[indexLeft];
        previousCharacterPreview.GetComponent<Renderer>().material = CharacterTypeMaterialMappings[previousCharacterType];
        previousCharacterPreview.transform.GetChild(0).GetComponent<Renderer>().material = CharacterTypeIconMappings[previousCharacterType];


    }
}
