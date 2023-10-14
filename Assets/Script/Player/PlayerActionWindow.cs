using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionWindow : MonoBehaviour
{
    [SerializeField]
    private DigimochiSelector digimochiSelector;

    [SerializeField] private GameObject confirmationWindow;
    [SerializeField] private GameObject waitingWindow;
    [SerializeField] private GameObject wrongWindow;
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
        currentAction = action;

        if (action.Type == PlayerAction.ActionType.Blockchain)
        {
            succesWindow.SetActive(false);
            wrongWindow.SetActive(false);
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
        switch(currentAction.Action)
        {
            case PlayerAction.PlayerActions.Bath:
                digimochiSelector.CurrentDigimochi.Bath(OnActionRealized);
                break;

            case PlayerAction.PlayerActions.Feed:
                digimochiSelector.CurrentDigimochi.Feed(OnActionRealized);
                break;

            case PlayerAction.PlayerActions.Cure:
                digimochiSelector.CurrentDigimochi.Cure(OnActionRealized);
                break;

            case PlayerAction.PlayerActions.Dance:
                digimochiSelector.CurrentDigimochi.Dance();
                break;

            case PlayerAction.PlayerActions.Pet:
                digimochiSelector.CurrentDigimochi.Pet();
                break;
        }
    }

    private void OnConfirmAction()
    {
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
            confirmationWindow.SetActive(false);
            waitingWindow.SetActive(false);
            wrongWindow.SetActive(true);
        }
    }

    private IEnumerator SuccesOperation()
    {
        succesWindow.SetActive(true);
        yield return new WaitForSeconds(1f);
        succesWindow.SetActive(false);
    }
}

