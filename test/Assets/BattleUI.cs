using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Hank.Battles
{
    // Scroll through menus and get the current index when selecting

    public class BattleUI : MonoBehaviour
    {
        public enum EBattleUIState
        {
            StatCards,
            Dialogue,
            SelectingPartyMember,
            SelectingAction,
            SelectingSkill,
            SelectingItem,
            SelectingTarget
        }

        [Header("Containers")]
        [SerializeField]
        private RectTransform _headerContainer;

        [SerializeField]
        private RectTransform _menuContainer;

        [SerializeField]
        private RectTransform _dialogueContainer;

        [Header("UI References")]
        [SerializeField]
        private DialogueTextWriter _dialogueTextWriter;

        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private StatCard _statCardPrefab;

        private StatCard[] _statCards;
        private Menu currentMenu;

        private void Start()
        {
            Cleanup();
        }

        private void UpdateUI()
        {
            // Advance one stage
            if(Input.GetKeyDown(KeyCode.Z))
            {
                currentMenu.Select();
            }

            // Backup a stage
            if(Input.GetKeyDown(KeyCode.X))
            {
                currentMenu.Close();
            }
        }

        // Called externally by battle handler when a battle is setup
        public void Setup(Battle currentBattle)
        {
            _statCards = new StatCard[currentBattle.PartyInBattle.Count];

            for (int i = 0; i < currentBattle.PartyInBattle.Count; i++)
            {
                // Instantiate and setup stat card
                StatCard card = Instantiate(_statCardPrefab, _headerContainer);
                card.Setup(currentBattle.PartyInBattle[i]);

                _statCards[i] = card;
            }
        }

        public void OpenContainers()
        {
            _headerContainer.gameObject.SetActive(true);
            _menuContainer.gameObject.SetActive(true);
        }

        public void ShowDialogue(DialogueData dialogueData)
        {
            OpenContainers();

            _dialogueContainer.gameObject.SetActive(true);
            _dialogueTextWriter.QueueDialoguePayload(dialogueData);
        }

        public void Cleanup()
        {
            _dialogueTextWriter.Cleanup();
            _headerContainer.gameObject.SetActive(false);
            _menuContainer.gameObject.SetActive(false);
        }
    }

    public abstract class Menu : MonoBehaviour
    {
        public int CurrentIndex => currentIndex;
        protected int currentIndex = 0;

        public abstract void Reset();
        public abstract void Open();
        public abstract void Close();
        public virtual int Select() => currentIndex;
    }
}
