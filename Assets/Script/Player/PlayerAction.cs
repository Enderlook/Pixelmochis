using System;

using UnityEngine;

namespace Pixelmochis
{
    public class PlayerAction : MonoBehaviour
    {
        public enum PlayerActions
        {
            Bath,
            Feed,
            Cure,
            Dance,
            Pet,
            Cancel
        }

        public enum ActionType
        {
            Blockchain,
            Instant,
        }

        [SerializeField]
        private PlayerActions playerAction;

        [SerializeField]
        private ActionType actionType;

        [SerializeField, TextArea]
        private string actionText;

        public string ActionText => actionText;

        public PlayerActions Action => playerAction;
        public ActionType Type => actionType;

        public event Action<PlayerAction> ActionPerformed;

        private Func<bool> ActionCondition = () => true;

        public void PerfomAction()
        {
            if (ActionCondition() == false)
                return;

            ActionPerformed?.Invoke(this);
        }

        public void SetCondition(Func<bool> condition)// Conditions are WIP
        {
            ActionCondition = condition;
        }
    }
}