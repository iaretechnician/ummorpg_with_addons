using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// UI QUEST TRACKER
public partial class UI_QuestTracker : MonoBehaviour
{

    public GameObject panel;
    public Transform content;
    public UI_QuestSlot slotPrefab;

    public string expandPrefix = "[+] ";
    public string hidePrefix = "[-] ";

    public Color fulfilledQuestColor;
    public Color inprogressQuestColor;

    public int maxActiveQuestsToShow = 5;

    // -----------------------------------------------------------------------------------
    // OnEnable
    // -----------------------------------------------------------------------------------
    private void OnEnable()
    {
        UpdateEvent();
    }

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    public void UpdateEvent()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (panel.activeSelf)
        {
            List<ExtendedQuest> activeQuests = player.playerExtendedQuest.extendedQuests.Where(q => !q.completed || (q.repeatable > 0 && !q.completedAgain)).ToList();

            int maxQuests = Mathf.Min(activeQuests.Count, maxActiveQuestsToShow);

            UIUtils.BalancePrefabs(slotPrefab.gameObject, maxQuests, content);

            // -- refresh all
            for (int i = 0; i < maxQuests; ++i)
            {
                int index = i;
                UI_QuestSlot slot = content.GetChild(index).GetComponent<UI_QuestSlot>();
                ExtendedQuest quest = activeQuests[index];

                // =======================================================================
                // -- check gathered items
                
                int[] gathered = player.playerExtendedQuest.CheckGatheredItems(quest);

                // -- check explored areas
                int explored = 0;
#if _iMMOEXPLORATION
                foreach (Exploration_Area area in quest.exploreTarget)
                {
                    if (player.playerAddonsConfigurator.HasExploredArea(area))
                        explored++;
                }
#endif

                // -- check faction requirement
                bool factionRequirementsMet = true;
#if _iMMOFACTIONS
                factionRequirementsMet = player.playerFactions.CheckFactionRating(quest.factionRequirement);
#endif

                // =======================================================================
                // name button
                    
                GameObject descriptionPanel = slot.descriptionText.gameObject;
                string prefix = descriptionPanel.activeSelf ? hidePrefix : expandPrefix;
                slot.nameButton.GetComponentInChildren<Text>().text = prefix + quest.name;
                slot.nameButton.onClick.SetListener(() =>
                {
                    descriptionPanel.SetActive(!descriptionPanel.activeSelf);
                });


                if (quest.IsFulfilled(gathered, explored, factionRequirementsMet))
                    slot.nameButton.GetComponent<Image>().color = fulfilledQuestColor;
                else
                    slot.nameButton.GetComponent<Image>().color = inprogressQuestColor;

                // -- cancel button
                slot.cancelButton.gameObject.SetActive(false);

                // -- update description
                slot.descriptionText.text = quest.TrackerTip(gathered, explored, factionRequirementsMet, player.level.current);
            }
        }
    }

    // -----------------------------------------------------------------------------------
}