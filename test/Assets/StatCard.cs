using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hank.Battles
{
    public class StatCard : Menu
    {
        [Header("UI Elements")]
        [SerializeField]
        private Image _statCardIcon;

        [SerializeField]
        private TextMeshProUGUI _partyMemberNameText;

        [SerializeField]
        private TextMeshProUGUI _healthText;

        [SerializeField]
        private TextMeshProUGUI _maxHealthText;

        [SerializeField]
        private RectTransform _choicesHolder;

        private PartyMemberUnit myPartyUnit;

        public void Setup(PartyMemberUnit myPartyUnit)
        {
            this.myPartyUnit = myPartyUnit;

            // Setup text
            _partyMemberNameText.text = myPartyUnit.MyStats.Name;
            _partyMemberNameText.color = myPartyUnit.VisualGuide.BaseColor;

            _healthText.text = myPartyUnit.MyHealth.CurrentHealth.ToString();
            _maxHealthText.text = $"/ {myPartyUnit.MyStats.MaxHP}";

            // Subscribe to events
            myPartyUnit.MyHealth.OnHealthUpdated += HealthUpdate;
        }

        public void HealthUpdate(int previousHealth, int newHealth)
        {
            // Set text
            _healthText.text = newHealth.ToString();
        }

        public override void Open()
        {
            // Opens the action menu

            _choicesHolder.gameObject.SetActive(true);
        }

        public override void Close()
        {
            // Closes the action menu

            _choicesHolder.gameObject.SetActive(false);
        }

        public override void Reset()
        {
            _partyMemberNameText.text = "NAME";
            _partyMemberNameText.color = Color.white;

            _healthText.text = "0";
            _maxHealthText.text = "/ 0";
        }
    }
}