using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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

    [SerializeField]
    private DigimochiSelector digimochiSelector;

    [SerializeField]
    private Button bathButton;

    [SerializeField]
    private Button feedButton;

    [SerializeField]
    private Button cureButton;

    [SerializeField]
    private Button danceButton;

    public event Action <PlayerActions> PlayerDoAction;

    private void Start()
    {
        bathButton.onClick.AddListener(OnPlayerActionBath);
        feedButton.onClick.AddListener(OnPlayerActionFeed);
        cureButton.onClick.AddListener(OnPlayerActionCure);
        danceButton.onClick.AddListener(OnPlayerActionDance);
    }

    private void OnDestroy()
    {
        bathButton.onClick.RemoveListener(OnPlayerActionBath);
        feedButton.onClick.RemoveListener(OnPlayerActionFeed);
        cureButton.onClick.RemoveListener(OnPlayerActionCure);
        danceButton.onClick.RemoveListener(OnPlayerActionDance);
    }

    private void OnPlayerActionBath()
    {
        PlayerDoAction?.Invoke(PlayerActions.Bath);
    }

    private void OnPlayerActionFeed()
    {
        PlayerDoAction?.Invoke(PlayerActions.Feed);
    }

    private void OnPlayerActionCure()
    {
        PlayerDoAction?.Invoke(PlayerActions.Cure);
    }

    private void OnPlayerActionDance()
    {
        PlayerDoAction?.Invoke(PlayerActions.Dance);
    }

    private void OnPlayerActionPet()
    {
        PlayerDoAction?.Invoke(PlayerActions.Pet);
    }
}
