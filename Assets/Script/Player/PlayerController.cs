using UnityEngine;
using static PlayerAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerActionWindow playerActionController;

    [SerializeField]
    private PlayerAction[] playerActions;


    private void Start()
    {
        foreach (var action in playerActions)
        {
            action.ActionPerformed += OnPlayerAction;
        }
    }

    private void OnDestroy()
    {
        foreach (var action in playerActions)
        {
            action.ActionPerformed -= OnPlayerAction;
        }
    }

    private void OnPlayerAction(PlayerAction action)
    {
        playerActionController.ReceiveAction(action);
    }
}
