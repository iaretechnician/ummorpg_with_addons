﻿// Simple character selection list. The charcter prefabs are known, so we could
// easily show 3D models, stats, etc. too.
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public partial class UI_CharacterSelectionV2 : MonoBehaviour
{
    [Header("Assign this variable")]
    [Space(20)]
    public UI_CharacterCreation uiCharacterCreation;
    public UIConfirmation uiConfirmation;
    public NetworkManagerMMO manager; // singleton is null until update
    public Transform mycamera;

    [Header("[-=-[ Default Configuration ]-=-]")]
#pragma warning disable
    [SerializeField] bool showConfiguration;
#pragma warning restore
    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue:true)]
    public UI_CharacterSlotV2 slotCharacter;
    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue: true)]
    public Transform content;
    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue: true)]
    public GameObject panel;
    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue: true)]
    public Button startButton;
    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue: true)]
    public Button deleteButton;
    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue: true)]
    public Button createButton;
    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue: true)]
    public Button quitButton;

    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue: true)]
    public bool useSelectionAnimation = false;
    [BoolShowConditional(conditionFieldName: "showConfiguration", conditionValue: true)]
    public string boolNameAnimation;
    [HideInInspector] public int curid = 0;

    void Update()
    {
        // show while in lobby and while not creating a character
        if (manager.state == NetworkState.Lobby && !uiCharacterCreation.IsVisible())
        {
            panel.SetActive(true);

            // characters available message received already?
            if (manager.charactersAvailableMsg.characters != null)
            {
                CharactersAvailableMsg.CharacterPreview[] characters = manager.charactersAvailableMsg.characters;
                UIUtils.BalancePrefabs(slotCharacter.gameObject, characters.Length, content);
                manager.selection = (manager.selection >=0 && characters.Length >= 1) ? manager.selection : 0;
                for (int c = 0; c < characters.Length; c++)
                {
                    int currentID = c;
                    
                    Player plyer = manager.selectionLocations[currentID].gameObject.GetComponentInChildren<Player>();
                    UI_CharacterSlotV2 slot = content.GetChild(currentID).GetComponent<UI_CharacterSlotV2>();

                    slot.characterName.text = characters[currentID].name;
                    slot.characterLevel.text = characters[currentID].level.ToString();

                    slot.characterClasse.text = characters[currentID].className
#if _iMMOTRAITS
                       + " | " + characters[currentID].classPlayer
#endif
                        ;
                    slot.isGM.SetActive(
#if !_iMMO2D
                        characters[currentID].isGameMaster
#else
                        false
#endif
                    );


                    slot.button.interactable = (currentID != manager.selection);
                    slot.button.onClick.SetListener(() =>
                    {
                        manager.selection = currentID;
                        //slot.button.interactable = false;
                        //MoveCameraToPlayer(manager.selectionLocations[currentID].transform.position);
                    });
                    manager.selectionLocations[currentID].gameObject.SetActive(manager.selection == currentID);
                    if (useSelectionAnimation && boolNameAnimation != "")
                    {
                        if (plyer != null)
                            plyer.animator.SetBool(boolNameAnimation, true);
                    }
                    if(plyer != null) 
                        plyer.nameOverlay.gameObject.SetActive(false);
                    if(plyer && plyer.portraitIcon != null)
                        slot.characterImage.sprite = plyer.portraitIcon;
                    curid = manager.selection;
                }
                // start button: calls AddPLayer which calls OnServerAddPlayer
                // -> button sends a request to the server
                // -> if we press button again while request hasn't finished
                //    then we will get the error:
                //    'ClientScene::AddPlayer: playerControllerId of 0 already in use.'
                //    which will happen sometimes at low-fps or high-latency
                // -> internally ClientScene.AddPlayer adds to localPlayers
                //    immediately, so let's check that first
                startButton.gameObject.SetActive(manager.selection != -1);
                startButton.onClick.SetListener(() => {
                    // set client "ready". we will receive world messages from
                    // monsters etc. then.


                    NetworkClient.Ready();
#if _iMMOLOBBY
                    Tools_UI_Tools.FadeOutScreen(false);
#endif

                    // send CharacterSelect message (need to be ready first!)
                    NetworkClient.Send(new CharacterSelectMsg { index = manager.selection });
#if _iMMOSCENELOADER
                    SceneLoaderAsync.Instance.LoadScene(characters[manager.selection].startingScene);
#else
                    // clear character selection previews
                    manager.ClearPreviews();
                    // make sure we can't select twice and call AddPlayer twice
                    panel.SetActive(false);

#endif
                });

                // delete button
                deleteButton.gameObject.SetActive(manager.selection != -1);
                deleteButton.onClick.SetListener(() => {
                    uiConfirmation.Show(
                        "Do you really want to delete <b>" + characters[manager.selection].name + "</b>?",
                        () => { NetworkClient.Send(new CharacterDeleteMsg { index = manager.selection }); }
                    );
                });

                // create button
                createButton.interactable = characters.Length < manager.characterLimit;
                createButton.onClick.SetListener(() => {
                    panel.SetActive(false);
                    uiCharacterCreation.Show();
                });

                // quit button
                quitButton.onClick.SetListener(() => { NetworkManagerMMO.Quit(); });
            }
#if _iMMOSCENELOADER
            if (SceneLoaderAsync.Instance.LoadingProgress == 100 && SceneLoaderAsync.Instance.LoadedScene)
            {
                //
                
                // clear character selection previews
                manager.ClearPreviews();

                // make sure we can't select twice and call AddPlayer twice
                panel.SetActive(false);
                // Debug.Log("ok?");
                //loadAfter = false;

                //backgroundDefault.SetActive(false);
            }
#endif
        }
        else panel.SetActive(false);
    }

    public void MoveCameraToPlayer(Vector3 playerpos)
    {
            mycamera.position = playerpos - new Vector3(-2, -2,0); // Ajustez les valeurs selon vos besoins
    }
}
