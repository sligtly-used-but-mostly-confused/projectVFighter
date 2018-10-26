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
    public GameObject descriptionPrefab;
    public float secondForCharacterTip;

    private readonly List<PlayerCharacterType> CharacterTypes = Enum.GetValues(typeof(PlayerCharacterType)).Cast<PlayerCharacterType>().ToList();

    private GameObject previousCharacterPreview;
    private GameObject nextCharacterPreview;
    private GameObject currentIcon;
    private GameObject descriptionCanvas;

    private PlayerController playerController;

    [System.Serializable]
    public struct CaracterData
    {
        public Material characterMaterial;
        public Material IconMaterial;
        public string description;
        public PlayerCharacterType CharacterType;
    }

    [SerializeField]
    private List<CaracterData> characterDataList = new List<CaracterData>();

    public Dictionary<PlayerCharacterType, Material> CharacterTypeMaterialMappings = new Dictionary<PlayerCharacterType, Material>();
    public Dictionary<PlayerCharacterType, Material> CharacterTypeIconMappings = new Dictionary<PlayerCharacterType, Material>();
    public Dictionary<PlayerCharacterType, string> CharacterTypeDescriptionMappings = new Dictionary<PlayerCharacterType, string>();

    private bool _hasFoundReticle = false;
    private float timeOnSelection;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        characterDataList.ForEach(x => CharacterTypeMaterialMappings.Add(x.CharacterType, x.characterMaterial));
        characterDataList.ForEach(x => CharacterTypeIconMappings.Add(x.CharacterType, x.IconMaterial));
        characterDataList.ForEach(x => CharacterTypeDescriptionMappings.Add(x.CharacterType, x.description));

        previousCharacterPreview = Instantiate(previewPrefab);
        previousCharacterPreview.transform.SetParent(transform);
        previousCharacterPreview.transform.position = Vector3.left + new Vector3(0, 0, -3);

        nextCharacterPreview = Instantiate(previewPrefab);
        nextCharacterPreview.transform.SetParent(transform);
        nextCharacterPreview.transform.position = Vector3.right + new Vector3(0, 0, -3);

        currentIcon = Instantiate(currentIconGameObject);
        currentIcon.transform.SetParent(transform);
        currentIcon.transform.position = Vector3.down + new Vector3(0, 0, -3);

        descriptionCanvas = Instantiate(descriptionPrefab);
        descriptionCanvas.transform.SetParent(transform);
        descriptionCanvas.transform.position = Vector3.down * 2 + new Vector3(0,0,-3);

        ChangeToNextCharacterTypeInternal(0);
        timeOnSelection = 0;
    }

    void Update() {

        timeOnSelection += Time.deltaTime;

        bool ready = playerController.IsReady;

        nextCharacterPreview.SetActive(!ready);
        previousCharacterPreview.SetActive(!ready);
        currentIcon.SetActive(false);
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
        
        if(!_hasFoundReticle && GetComponent<PlayerController>().Reticle)
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

        if(GetComponent<GravityObjectRigidBody>().GravityDirection.y < 0){
            descriptionCanvas.transform.localPosition = Vector3.up * 4 + new Vector3(0, 0, -3);
        }
        else{
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

        //reset selection time and update description
        descriptionCanvas.transform.GetChild(0).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = CharacterTypeDescriptionMappings[GetComponent<PlayerController>().CharacterType];
        timeOnSelection = 0;
        descriptionCanvas.SetActive(false);
    }
}
