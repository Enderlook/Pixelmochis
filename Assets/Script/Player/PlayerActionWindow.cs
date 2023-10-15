using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Pixelmotchis
{
    public class PlayerActionWindow : MonoBehaviour
    {
        [SerializeField]
        private DigimochiSelector digimochiSelector;

        [SerializeField] private GameObject confirmationWindow;
        [SerializeField] private GameObject waitingWindow;
        [SerializeField] private GameObject failedWindow;
        [SerializeField] private GameObject succesWindow;
        [SerializeField] private TextMeshProUGUI actionText;

        [SerializeField] private Button confirmActionButton;
        [SerializeField] private Button cancelActionButton;

        private PlayerAction currentAction;

        private void Start()
        {
            confirmActionButton.onClick.AddListener(OnConfirmAction);
            cancelActionButton.onClick.AddListener(OnCancelAction);
        }

        public void ReceiveAction(PlayerAction action)
        {
            if (digimochiSelector.CurrentDigimochi.IsPerfomingAction)
            {
                return;
            }

            currentAction = action;

            if (action.Type == PlayerAction.ActionType.Blockchain)
            {
                succesWindow.SetActive(false);
                failedWindow.SetActive(false);
                confirmationWindow.SetActive(true);
                actionText.text = action.ActionText;
            }
            else
            {
                DoAction();
            }
        }

        //TODO: Emprolijar esto, hacerlo generico
        public void DoAction()
        {
            var digimochi = digimochiSelector.CurrentDigimochi;

            switch (currentAction.Action)
            {
                case PlayerAction.PlayerActions.Bath:
                    //currentAction.SetCondition(digimochi.IsDirty);
                    digimochi.Bath(OnActionRealized);
                    break;

                case PlayerAction.PlayerActions.Feed:
                    //currentAction.SetCondition(digimochi.IsHungry);
                    digimochi.Feed(OnActionRealized);
                    break;

                case PlayerAction.PlayerActions.Cure:
                    //currentAction.SetCondition(digimochi.IsSick);
                    digimochi.Cure(OnActionRealized);
                    break;

                case PlayerAction.PlayerActions.Dance:
                    digimochi.Dance();
                    break;

                case PlayerAction.PlayerActions.Pet:
                    digimochi.Pet();
                    break;
            }
        }

        private void OnConfirmAction()
        {
            confirmationWindow.SetActive(false);
            waitingWindow.SetActive(true);
            DoAction();
        }

        private void OnCancelAction()
        {
            confirmationWindow.SetActive(false);
            waitingWindow.SetActive(false);
        }

        private void OnActionRealized(bool succes)
        {
            confirmationWindow.SetActive(false);
            waitingWindow.SetActive(false);
            if (succes)
            {
                StartCoroutine(SuccesOperation());
            }
            else
            {
                StartCoroutine(FailedOperation());
            }
        }

        private IEnumerator SuccesOperation()
        {
            succesWindow.SetActive(true);
            yield return new WaitForSeconds(1f);
            succesWindow.SetActive(false);
        }

        private IEnumerator FailedOperation()
        {
            failedWindow.SetActive(true);
            yield return new WaitForSeconds(1f);
            failedWindow.SetActive(false);
        }
    }

}