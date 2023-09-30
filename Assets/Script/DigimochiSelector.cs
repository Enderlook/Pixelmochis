using System.Collections.Generic;
using System.Linq;
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

    private void Awake()
    {
        nextButton.onClick.AddListener(NextDigimochi);
        previousButton.onClick.AddListener(PreviousDigimochi);

        digimochiManager.DigimochisLoaded += OnDigimochisLoaded;
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
            digimochis[i].gameObject.SetActive(i == digimochiIndex);
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        nextButton.interactable = interactable;
        previousButton.interactable = interactable;
    }

}
