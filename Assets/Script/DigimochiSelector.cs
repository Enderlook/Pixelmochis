using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigimochiSelector : MonoBehaviour
{
    [SerializeField] private DigimochiManager digimochiManager;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private GameObject noDigimochisSign;

    private List<Digimochi> digimochis = new List<Digimochi>();

    private int digimochiIndex;

    public event Action<Digimochi> DigimochiSelected;
    public Digimochi CurrentDigimochi => digimochis[digimochiIndex];

    private void Awake()
    {
        nextButton.onClick.AddListener(NextDigimochi);
        previousButton.onClick.AddListener(PreviousDigimochi);

        digimochiManager.DigimochisLoaded += OnDigimochisLoaded;

        SetButtonsInteractable(false);
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(NextDigimochi);
        previousButton.onClick.RemoveListener(PreviousDigimochi);

        digimochiManager.DigimochisLoaded -= OnDigimochisLoaded;
    }

    private void OnDigimochisLoaded(List<Digimochi> digimochis)
    {
        if(digimochis == null)
        {
            UpdateCurrentDigimochi();
            return;
        }

        this.digimochis = digimochis;
        for (int i = 0; i < digimochis.Count; i++)
        {
            digimochis[i].transform.SetParent(transform.transform, false);
            digimochis[i].ActionPerformed += OnDigimochiPerformAction;
            digimochis[i].ActionFinished += OnDigimochiFinishAction;
        }

        UpdateCurrentDigimochi();
    }

    private void PreviousDigimochi()
    {
        if (digimochiIndex == 0)
        {
            digimochiIndex = digimochis.Count - 1;
        }
        else
        {
            digimochiIndex--;
        }

        UpdateCurrentDigimochi();
    }

    private void NextDigimochi()
    {
        if (digimochiIndex >= digimochis.Count - 1)
        {
            digimochiIndex = 0;
        }
        else
        {
            digimochiIndex++;
        }

        UpdateCurrentDigimochi();
    }

    private void UpdateCurrentDigimochi()
    {
        bool hasDigimochis = digimochis.Count > 0;

        SetButtonsInteractable(digimochis.Count > 1);
        noDigimochisSign.SetActive(!hasDigimochis);

        for (int i = 0; i < digimochis.Count; i++)
        {
            if (i == digimochiIndex)
            {
                digimochis[i].gameObject.SetActive(true);
                digimochis[digimochiIndex].ActiveDigimochi();
            }
            else 
            {
                digimochis[digimochiIndex].DisableDigimochi();
                digimochis[i].gameObject.SetActive(false);
            }
        }

        DigimochiSelected?.Invoke(digimochis[digimochiIndex]);
    }

    private void OnDigimochiPerformAction()
    {
        SetButtonsInteractable(false);
    }

    private void OnDigimochiFinishAction()
    {
        SetButtonsInteractable(digimochis.Count > 1);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        nextButton.gameObject.SetActive(interactable);
        previousButton.gameObject.SetActive(interactable);
    }

}
