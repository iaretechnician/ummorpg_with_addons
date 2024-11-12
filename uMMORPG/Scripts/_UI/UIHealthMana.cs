using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIHealthMana : MonoBehaviour
{
    public GameObject panel;
    public Slider healthSlider;
    public TextMeshProUGUI healthStatus;
    public Slider manaSlider;
    public TextMeshProUGUI manaStatus;

    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            panel.SetActive(true);
            healthSlider.value = player.health.Percent();
            healthStatus.text = player.health.current + " / " + player.health.max;

            manaSlider.value = player.mana.Percent();
            manaStatus.text = player.mana.current + " / " + player.mana.max;
        }
        else panel.SetActive(false);
    }
}
