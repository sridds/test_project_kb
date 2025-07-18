using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

namespace Hank.Battles
{
    public class PartyMemberUnit : BattleUnit
    {
        public List<UnitAttack> MySkills => _skills;
        public UnitAction CurrentAction => currentAction;
        public UnitVisualGuide VisualGuide => _visualGuide;

        [SerializeField]
        private UnitVisualGuide _visualGuide;

        [SerializeField]
        private UnitAttack _bashAttack;

        [SerializeField]
        private ConsumeItem _eatAction;

        [SerializeField]
        private List<UnitAttack> _skills = new List<UnitAttack>(); // this would probably be a list of structs -- the unit attack and the level at which you need to be to use it

        [SerializeField]
        private Color _decommissionColor;

        private UnitAction currentAction;
        private bool isDecommissioned;
        private Color defaultColor;

        #region MonoBehaviour
        private void Start()
        {
            defaultColor = _unitRenderer.color;
        }
        #endregion

        #region Overrides
        // During an update, the player can press Z during an attack
        public override void UpdateBashState()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                (currentAction as UnitAttack).RegisterInput();
            }
        }
        #endregion

        #region [BASH]
        public override void StartBash(BattleUnit target)
        {
            // Create attack animation and hide the battle sprite
            SetRendererEnabled(false);

            // Initalize attack
            UnitAttack attack = Instantiate(_bashAttack, transform.position, Quaternion.identity);
            currentAction = attack;

            attack.Initialize(this, target);
            attack.StartAction();
            attack.OnActionComplete += HandleActionEnding;
        }

        protected override void HandleActionEnding()
        {
            currentAction.OnActionComplete -= HandleActionEnding;
            OnActionComplete?.Invoke();

            Destroy(currentAction.gameObject);
            SetRendererEnabled(true);

            // Advance to the next turn
            base.HandleActionEnding();
        }
        #endregion

        #region [ITEM]
        public void StartUsingItem(int itemIndex, BattleUnit target)
        {
            Debug.Log($"fuck {itemIndex}");

            // Create attack animation and hide the battle sprite
            SetRendererEnabled(false);
            /*
            PartyManager.Instance.Bag.TryRemoveItem(itemIndex, out Item item);

            if (item is Consumable)
            {
                ConsumeItem consumeAction = Instantiate(_eatAction, transform.position, Quaternion.identity);
                consumeAction.Initialize(this, target, item as Consumable);
                consumeAction.StartAction();
                consumeAction.OnActionComplete += HandleActionEnding;

                currentAction = consumeAction;
            }*/
        }
        #endregion

        #region [TACTICS]
        public void StartGuarding()
        {

        }
        #endregion

        #region Methods
        public void UpdateDefendState() { }
        #endregion

        #region Helper Methods
        public void SetDecommissionedVisual(bool isDecommissioned)
        {
            this.isDecommissioned = isDecommissioned;

            _unitRenderer.DOKill(false);
            _unitRenderer.DOColor(isDecommissioned ? _decommissionColor : defaultColor, 0.2f);
        }

        public override void HandleHealthUpdate(int oldHealth, int newHealth) { }
        #endregion
    }
}