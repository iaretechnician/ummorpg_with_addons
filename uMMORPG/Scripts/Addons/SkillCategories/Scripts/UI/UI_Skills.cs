using UnityEngine;
using UnityEngine.UI;

public partial class UI_Skills : MonoBehaviour
{
    public KeyCode hotKey = KeyCode.R;
    public GameObject panel;
    public UI_SkillSlot slotPrefab;
    public Transform content;
    public Text skillExperienceText;
    public GameEvent ui_skillsUpdateEvent;
    public string currentCategory;

    private void Start()
    {

        Player player = Player.localPlayer;
        if (!player) { panel.SetActive(false); return; }

    }
    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    /*private void Update()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // hotkey (not while typing in chat, etc.)
        if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
            panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
            Refresh();
    }*/
    public void ShowHide(bool value)
    {
        ui_skillsUpdateEvent.TriggerEventBool(value);
    }
    // -----------------------------------------------------------------------------------
    // Refresh
    // -----------------------------------------------------------------------------------
    public void Refresh()
    {
        Player player = Player.localPlayer;
        if (!player) return;

        // only update the panel if it's active
        if (panel.activeSelf)
        {
            // instantiate/destroy enough slots
            // (we only care about non status skills)
            UIUtils.BalancePrefabs(slotPrefab.gameObject, GetSkillCount(), content);

            int t_index = -1;
            int s_index = 0;

            // refresh all
            for (int i = 0; i < player.skills.skills.Count; ++i)
            {
                Debug.Log("Bouh");
                if (CanShow(i))
                {
                    s_index = i;
                    t_index++;

                    UI_SkillSlot slot = content.GetChild(t_index).GetComponent<UI_SkillSlot>();
                    Skill skill = player.skills.skills[s_index];

                    bool isPassive = skill.data is PassiveSkill;

                    // drag and drop name has to be the index in the real skill list,
                    // not in the filtered list, otherwise drag and drop may fail
                    int skillIndex = player.skills.skills.FindIndex(s => s.name == skill.name);
                    slot.dragAndDropable.name = skillIndex.ToString();

                    // click event
                    slot.button.interactable = skill.level > 0 && !isPassive && player.skills.CastCheckSelf(skill); // checks mana, cooldown etc.
                    slot.button.onClick.SetListener(() =>
                    {
                        ((PlayerSkills)player.skills).TryUse(skillIndex);
                    });

                    // set state
                    slot.dragAndDropable.dragable = skill.level > 0 && !isPassive;

                    // image
                    if (skill.level > 0)
                    {
                        slot.image.color = Color.white;
                        slot.image.sprite = skill.image;
                    }

                    // description
                    slot.descriptionText.text = skill.ToolTip(showRequirements: skill.level == 0);

                    // learn / upgrade
                    if (skill.level < skill.maxLevel && ((PlayerSkills)player.skills).CanUpgrade(skill))
                    {
                        slot.upgradeButton.gameObject.SetActive(true);
                        slot.upgradeButton.GetComponentInChildren<Text>().text = skill.level == 0 ? "Learn" : "Upgrade";
                        slot.upgradeButton.interactable = true;
                        slot.upgradeButton.onClick.SetListener(() =>
                        {
                            ((PlayerSkills)player.skills).CmdUpgrade(skillIndex);
                            Debug.Log("refresh!!");
                            Invoke(nameof(Refresh), 0.5f);
                        });
                    }
                    else slot.upgradeButton.gameObject.SetActive(false);

                    // cooldown overlay
                    float cooldown = skill.CooldownRemaining();
                    slot.cooldownOverlay.SetActive(skill.level > 0 && cooldown > 0);
                    slot.cooldownText.text = cooldown.ToString("F0");
                    slot.cooldownCircle.fillAmount = skill.cooldown > 0 ? cooldown / skill.cooldown : 0;
                }
            }

            // skill experience
            skillExperienceText.text = ((PlayerSkills)player.skills).skillExperience.ToString();
        }
    }

    // -----------------------------------------------------------------------------------
    // OnEnable
    // -----------------------------------------------------------------------------------
    private void OnEnable()
    {
        Refresh();
    }

    private void OnDisable()
    {
        content.RemoveAllChildren();
    }
    // -----------------------------------------------------------------------------------
    // changeCategory
    // -----------------------------------------------------------------------------------
    public void ChangeCategory(string newCategory)
    {
        currentCategory = newCategory;

        content.RemoveAllChildren();

        Invoke(nameof(Refresh), .1f);

        panel.SetActive(true);
    }

    // -----------------------------------------------------------------------------------
    // getSkillCount
    // -----------------------------------------------------------------------------------
    private int GetSkillCount()
    {
        Player player = Player.localPlayer;
        if (!player) return 0;

        int count = 0;
        Debug.Log("ou ou");
        for (int i = 0; i < player.skills.skills.Count; ++i)
        {

            if (CanShow(i))
                count++;
        }

        return count;
    }

    // -----------------------------------------------------------------------------------
    // canShow
    // -----------------------------------------------------------------------------------
    private bool CanShow(int index)
    {
        //Debug.Log("oh oh");
        //Player player = Player.localPlayer;
        //if (!player) return false;

        //bool valid = ((player.skills.skills[index].category == currentCategory || currentCategory == "") && (!player.skills.skills[index].unlearnable || (player.skills.skills[index].unlearnable && player.skills.skills[index].level > 0)));

#if _iMMOPRESTIGECLASSES
        //valid = (player.playerAddonsConfigurator.CheckPrestigeClass(player.skills.skills[index].data.learnablePrestigeClasses)) && valid;
#endif

        //return valid;

        return true;
    }

    // -----------------------------------------------------------------------------------
}