using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DropdownParty : MonoBehaviour
{
    public GameObject partyDropdown;
    public Button[] activeButtons;
    private GraphicRaycaster m_Raycaster;
    private EventSystem m_EventSystem;
    private Player player;
    private PointerEventData m_PointerEventData;
    private UIPartyHUDMemberSlot partySlot;

    private Entity target = null;
    private float distance = 0;

    private void Start()
    {
        m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Update()
    {
        // Checks for the local player if we don't have a player yet.
        player = Player.localPlayer;
        if (player == null) return;

        // On right mouse click check if on a component that has a dropdown.
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            // Check results of our UI raycast if its a valid target then show the dropdown.
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "SlotPartyHUDMember(Clone)")
                {
                    partySlot = result.gameObject.GetComponent<UIPartyHUDMemberSlot>();
                    partyDropdown.GetComponent<RectTransform>().position = result.screenPosition;
                    partyDropdown.SetActive(true);
                }
            }
        }

        // If the dropdown is shown then check to see what buttons can be used.
        if (partyDropdown.activeSelf)
        {
            // Grab the clicked party member.
            string clickedMember = string.Empty;
            int clickedIndex = 0;
            for (int i = 0; i < player.party.party.members.Length; i++)
                if (player.party.party.members[i] == partySlot.nameText.text)
                {
                    clickedMember = player.party.party.members[i];
                    clickedIndex = i;
                    player.clickedPartyIndex = clickedIndex;

                    Player member = Player.onlinePlayers[clickedMember];
                    player.CmdSetTarget(member.netIdentity);
                }

            // action button:
            // dismiss: if i=0 and member=self and master
            // kick: if i > 0 and player=master
            // leave: if member=self and not master
            if (player.name == player.party.party.master)
            {
                // promote button
                activeButtons[1].gameObject.SetActive(true);
                activeButtons[1].onClick.SetListener(() =>
                {
                    player.CmdPartyPromote(clickedIndex);
                    partyDropdown.SetActive(false);
                });

                // kick button
                activeButtons[0].gameObject.SetActive(true);
                activeButtons[0].onClick.SetListener(() =>
                {
                    player.party.CmdKick(clickedMember);
                    partyDropdown.SetActive(false);
                });
            }
            else
            {
                activeButtons[0].gameObject.SetActive(false);
                activeButtons[1].gameObject.SetActive(false);
            }

            // trade button
            activeButtons[4].gameObject.SetActive(true);
            activeButtons[4].onClick.SetListener(() =>
            {
                player.trading.CmdSendRequest();
                partyDropdown.SetActive(false);
            });

            // guild invite button
            if (target is Player && player.guild.InGuild())
            {
                activeButtons[2].gameObject.SetActive(true);
                activeButtons[2].interactable = !((Player)target).guild.InGuild() &&
                                                 player.guild.guild.CanInvite(player.name, target.name) &&
                                                 NetworkTime.time >= player.nextRiskyActionTime &&
                                                 distance <= player.interactionRange;
                activeButtons[2].onClick.SetListener(() =>
                {
                    player.guild.CmdInviteTarget();
                    partyDropdown.SetActive(false);
                });
            }
            else activeButtons[2].gameObject.SetActive(false);

#if _iMMOPVP && _iMMOFRIENDS
            if (player.target && player.target is Player && player.target != player && player.Tools_SameRealm((Player)player.target))
            {
                activeButtons[3].gameObject.SetActive(true);
                activeButtons[3].interactable = player.playerAddonsConfigurator.Friends.FindIndex(x => x.name == ((Player)(player.target)).name) == -1 ? true : false;
                activeButtons[3].onClick.SetListener(() =>
                {
                    player.playerAddonsConfigurator.Cmd_AddFriend(((Player)(player.target)).name);
                    partyDropdown.SetActive(false);
                });
            }
            else activeButtons[3].gameObject.SetActive(false);

#elif _iMMOFRIENDS
 		    if (player.target && player.target is Player && player.target != player) {
                activeButtons[3].gameObject.SetActive(true);
                activeButtons[3].interactable = player.playerAddonsConfigurator.Friends.FindIndex(x=> x.name == ((Player)(player.target)).name) == -1  ? true : false;
                activeButtons[3].onClick.SetListener(() => {
                    player.playerAddonsConfigurator.Cmd_AddFriend(((Player)(player.target)).name);
                    partyDropdown.SetActive(false);
                });
            }
            else activeButtons[3].gameObject.SetActive(false);

#endif

            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            // Check results of our UI raycast if its a valid player then show the dropdown.
            int foundMenu = 0;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "SlotPartyHUDMember(Clone)" || result.gameObject.name == "PartyDropdown")
                    foundMenu++;
            }

            if (foundMenu == 0)
                partyDropdown.SetActive(false);
        }
    }
}