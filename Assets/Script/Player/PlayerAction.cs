using System;
using UnityEngine;

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


    public void PerfomAction()
    {
        ActionPerformed?.Invoke(this);
    }
}
