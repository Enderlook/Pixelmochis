using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;


public class DigimochiManager : MonoBehaviour
{
    [SerializeField]// TODO: Hacer que esto sea IDigimochiData
    private List<DigimochiDataProxy> userDigimochis = new List<DigimochiDataProxy>();

    [SerializeField]
    private DigimochiGlossary digimochiGlossary;

    [SerializeField]
    private Digimochi digimochiPrefab;

    [SerializeField]
    private GameObject loadingDigimochisSign;

    public event Action<List<Digimochi>> DigimochisLoaded;

    private  List<Digimochi> digimochisIntantied = new List<Digimochi>();

    void Start()
    {
        StartCoroutine(SyncUserDigimochis());
    }

    [Button]
    private void SyncDigimochisDEBUG()
    {
        StartCoroutine(SyncUserDigimochis());
    }

    public IEnumerator SyncUserDigimochis()
    {
        loadingDigimochisSign.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        // Aquí llamas a la API o la clase que te permite obtener los Digimochis del usuario desde la blockchain.
        // userDigimochis = Obtener data de la blockchain y parsearla a la clase de blockchain que implemente IDigimochiData

        // Removemos cualquier digimochi que pueda ser nulo.
        userDigimochis.RemoveAll(x => x == null);

        //Si hay digimochis los instanciamos
        if (userDigimochis.Count > 0 && userDigimochis != null)
        {
            InstantieDigimochis();
            DigimochisLoaded?.Invoke(digimochisIntantied);
        }
        else
        {
            DigimochisLoaded?.Invoke(null);
            Debug.Log("Could not load any digimochis");
        }
        loadingDigimochisSign.SetActive(false);
    }

    // El metodo que se llama luego de sincronizar los digimochis
    private void InstantieDigimochis()
    {
        //Si ya hay digimochis instanciados, los removemos.
        if(digimochisIntantied.Count > 0)
        {
            foreach (var digimochi in digimochisIntantied)
            {
                Destroy(digimochi.gameObject);
            }
            digimochisIntantied = new List<Digimochi>();
        }
        
        for (int i = 0; i < userDigimochis.Count; i++)
        {
            var userDigimochiData = userDigimochis[i];
            var digimochi = Instantiate(digimochiPrefab);
            digimochisIntantied.Add(digimochi);

            var digimochiSOFromGlossary = digimochiGlossary.GetDigimochiSO(userDigimochiData.GetDigimochiType());
            digimochi.SetDigimochiSO(digimochiSOFromGlossary);
            digimochi.SetDigimochiData(userDigimochiData);
            digimochi.Initialize();
        }
    }
}
