using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_PetStatus : MonoBehaviour
{
    public GameObject panel;
    public Button backgroundButton;
    public Image image;
    public Slider healthSlider;
    public Slider experienceSlider;
    public Text nameText;
    public Text levelText;
    public Button autoAttackButton;
    public Button defendOwnerButton;
    public Button unsummonButton;


    [Header("[-=-=-[ Refresh Time ]-=-=-]")]
    public float refreshSpeed = .5f;

    public void Start()
    {
        StartCoroutine(CR_PetStatus());
    }
    private IEnumerator CR_PetStatus()
    {
        while (true)
        {

            UpdatePetStatus();

            yield return new WaitForSeconds(refreshSpeed);

        }
    }
    void UpdatePetStatus()
    {
        Player player = Player.localPlayer;

        if (player != null &&
            player.petControl.activePet != null)
        {
            Pet pet = player.petControl.activePet;
            panel.SetActive(true);

            image.sprite = pet.portraitIcon;

            backgroundButton.onClick.SetListener(() => {
                // pet variable might be null by the TimeLogout button gets
                // clicked. can't target null, otherwise we get a
                // MissingReferenceException.
                if (pet != null)
                    player.CmdSetTarget(pet.netIdentity);
            });

            healthSlider.value = pet.health.Percent();
            healthSlider.GetComponent<UIShowToolTip>().text = "Health: " + pet.health.current + " / " + pet.health.max;

            experienceSlider.value = pet.experience.Percent();
            experienceSlider.GetComponent<UIShowToolTip>().text = "Experience: " + pet.experience.current + " / " + pet.experience.max;

            nameText.text = pet.name;
            levelText.text = "Lv." + pet.level.current;

            autoAttackButton.GetComponentInChildren<Text>().fontStyle = pet.autoAttack ? FontStyle.Bold : FontStyle.Normal;
            autoAttackButton.onClick.SetListener(() => {
                if (pet != null)
                    pet.CmdSetAutoAttack(!pet.autoAttack);
            });

            defendOwnerButton.GetComponentInChildren<Text>().fontStyle = pet.defendOwner ? FontStyle.Bold : FontStyle.Normal;
            defendOwnerButton.onClick.SetListener(() => {
                if (pet != null)
                    pet.CmdSetDefendOwner(!pet.defendOwner);
            });

            //unsummonButton.interactable = player.CanUnsummonPet(); <- looks too annoying if button flashes rapidly
            unsummonButton.onClick.SetListener(() => {
                if (player.petControl.CanUnsummon())
                    player.petControl.CmdUnsummon();
            });
        }
        else panel.SetActive(false);
    }
}
