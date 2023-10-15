using Solana.Unity.SDK;
using Solana.Unity.Wallet;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

namespace Pixelmotchis
{
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

        private List<Digimochi> digimochisIntantied = new List<Digimochi>();

        void Start()
        {
            if (useDigimochiDataProxy)
            {
                StartCoroutine(SyncUserDigimochisLocally());
            }
            else
            {
                if (Web3.Instance != null)
                {
                    OnWalletInstanced();
                }
                else
                {
                    Web3.OnWalletInstance += OnWalletInstanced;
                }
            }
        }

        private void OnWalletInstanced()
        {
            if (Web3.Account != null)
            {
                StartCoroutine(SyncUserDigimochisFromNet());
            }
            else
            {
                Async.Handle(LoginWithWaletAdapter());
            }
        }

        private async Task LoginWithWaletAdapter()
        {
            Account account;
            try
            {
                account = await Web3.Instance.LoginWalletAdapter();
            }
            catch
            {
                // User cancelled sync.
                account = null;
            }
            if (account != null)
            {
                StartCoroutine(SyncUserDigimochisFromNet());
            }
        }

        public IEnumerator SyncUserDigimochisFromNet()
        {
            loadingDigimochisSign.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            yield return Async.Handle(GetDigimochisFromNet());

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
            if (digimochisIntantied.Count > 0)
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

        private async Task GetDigimochisFromNet()
        {
            List<IDigimochiData> digimochis = new();

            await foreach (DigimochiNFT digimochi in DigimochiNFT.GetAllDigimochis())
            {
                digimochis.Add(digimochi);
            }

            userDigimochis = digimochis;
        }
    }
}