// Simple character selection list. The charcter prefabs are known, so we could
// easily show 3D models, stats, etc. too.
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public partial class UI_CharacterSelection : MonoBehaviour
{
    public UI_CharacterCreation uiCharacterCreation;
    public UIConfirmation uiConfirmation;
    public NetworkManagerMMO manager; // singleton is null until update
    public GameObject panel;
    public Button startButton;
    public Button deleteButton;
    public Button createButton;
    public Button quitButton;

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
}
