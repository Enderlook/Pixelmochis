using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using Hackaton;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.ExceptionServices;

public class DigimochiManager : MonoBehaviour
{
    private List<IDigimochiData> userDigimochis = new List<IDigimochiData>();


    [SerializeField]
    private bool useDigimochiDataProxy;

    [SerializeField]
    private List<DigimochiDataProxy> userDigimochisProxy = new List<DigimochiDataProxy>();

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
        if (useDigimochiDataProxy)
        {
            StartCoroutine(SyncUserDigimochisLocally());
        }
        else
        {
            StartCoroutine(SyncUserDigimochis());
        }
    }

    public IEnumerator SyncUserDigimochis()
    {
        loadingDigimochisSign.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        // Aquí llamas a la API o la clase que te permite obtener los Digimochis del usuario desde la blockchain.
        // userDigimochis = Obtener data de la blockchain y parsearla a la clase de blockchain que implemente IDigimochiData

        List<IDigimochiData> digimochis = new();

        IAsyncEnumerator<DigimochiNFT> digimochisProducer = DigimochiNFT.GetAllDigimochis().GetAsyncEnumerator();

        while (true)
        {
            ValueTask<bool> task = digimochisProducer.MoveNextAsync();
            while (!task.IsCompleted)
            {
                yield return null;
            }

            ExceptionDispatchInfo exception = null;
            try
            {
                if (task.GetAwaiter().GetResult())
                {
                    digimochis.Add(digimochisProducer.Current);
                }
                else
                {
                    goto dispose;
                }
            }
            catch (Exception e)
            {
                exception = ExceptionDispatchInfo.Capture(e);
            }

            if (exception is not null)
            {
                ValueTask disposeTask = digimochisProducer.DisposeAsync();
                while (!disposeTask.IsCompleted)
                {
                    yield return null;
                }
                try
                {
                    disposeTask.GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    throw new AggregateException(e, exception.SourceException);
                }
                exception.Throw();
            }
        }
    dispose:
        {
            ValueTask disposeTask = digimochisProducer.DisposeAsync();
            while (!disposeTask.IsCompleted)
            {
                yield return null;
            }
            disposeTask.GetAwaiter().GetResult();
        }

        userDigimochis = digimochis;
        
        // Removemos cualquier digimochi que pueda ser nulo.
        //userDigimochis.RemoveAll(x => x == null);

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

    public IEnumerator SyncUserDigimochisLocally()
    {
        loadingDigimochisSign.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        userDigimochis = userDigimochisProxy.Select(x => x as IDigimochiData).ToList();
        
        // Removemos cualquier digimochi que pueda ser nulo.
        //userDigimochis.RemoveAll(x => x == null);

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

            var digimochiSOFromGlossary = digimochiGlossary.FindDigimochiSO(userDigimochiData.GetDigimochiType());
            if (digimochiSOFromGlossary == null)
                continue;

            var digimochi = Instantiate(digimochiPrefab);
            digimochisIntantied.Add(digimochi);
            
            digimochi.SetDigimochiSO(digimochiSOFromGlossary);
            digimochi.SetDigimochiData(userDigimochiData);
            digimochi.Initialize();
        }
    }
}
