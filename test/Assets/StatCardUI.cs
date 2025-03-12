using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatCardUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private Slider _healthSlider;

    [SerializeField]
    private Slider _manaSlider;

    private BattleUnit myUnit;

    public void Initialize(BattleUnit unit)
    {
        myUnit = unit;

        // Set current
        _nameText.text = myUnit.MyStats.Name;
        _healthSlider.maxValue = myUnit.MyStats.MaxHP;
        _healthSlider.value = myUnit.MyHealth.CurrentHealth;

        myUnit.MyHealth.OnHealthUpdated += UpdateHealth;
    }

    private void UpdateHealth(int lastHealth, int newHealth)
    {
        _healthSlider.value = myUnit.MyHealth.CurrentHealth;
    }
}
