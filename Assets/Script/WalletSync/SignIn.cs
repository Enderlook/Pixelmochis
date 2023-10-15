using Solana.Unity.SDK;
using Solana.Unity.Wallet;

using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;

namespace Pixelmochis
{
    public sealed class SignIn : Panel
    {
        private const string KEY = "RpcCluster";
        public const string INITIAL_PASSWORD_TEXT = "Enter Password...";

        [SerializeField]
        private Panel importWallet;

        [SerializeField]
        private Panel createWallet;

        private TextField passwordField;
        private Label incorrectPasswordLabel;

        protected override void Awake()
        {
            base.Awake();

            VisualElement rootVisualElement = RootVisualElement;

            RpcCluster cluster = (RpcCluster)PlayerPrefs.GetInt(KEY, (int)RpcCluster.MainNet);
            EnumField dropdown = rootVisualElement.Query<EnumField>("RpcClusterEnum");
            dropdown.value = cluster;
            dropdown.RegisterValueChangedCallback(@event => Async.Handle(SelectNetwork((RpcCluster)@event.newValue)));

            Web3.OnWalletInstance += () => Async.Handle(SelectNetwork(cluster));

            passwordField = rootVisualElement.Query<TextField>("PasswordField");
            incorrectPasswordLabel = rootVisualElement.Query<Label>("IncorrectPasswordLabel");

            rootVisualElement.Query<Button>("LoginButton").First().clicked += () => Async.Handle(LoginChecker());
            rootVisualElement.Query<Button>("WalletButton").First().clicked += () => Async.Handle(LoginWalletAdapter());
            rootVisualElement.Query<Button>("CreateWalletButton").First().clicked += () => PanelsManager.PushPanel(createWallet);
            rootVisualElement.Query<Button>("ImportWalletButton").First().clicked += () => PanelsManager.PushPanel(importWallet);

            foreach (Provider value in Enum.GetValues(typeof(Provider)))
            {
                Button button = rootVisualElement.Query<Button>($"{value}Button").First();
                if (button != null)
                {
                    button.clicked += () => Async.Handle(LoginCheckerWeb3Auth(value));
                }
            }
        }

        public override void Open()
        {
            base.Open();

            passwordField.value = INITIAL_PASSWORD_TEXT;
            incorrectPasswordLabel.visible = false;
        }

        public override void Uncover()
        {
            base.Open();

            passwordField.value = INITIAL_PASSWORD_TEXT;
            incorrectPasswordLabel.visible = false;
        }

        private async Task SelectNetwork(RpcCluster cluster)
        {
            Web3 instance = Web3.Instance;
            instance.rpcCluster = cluster;
            instance.customRpc = GetCustomRPC(cluster);
            instance.webSocketsRpc = GetWebSocketsRPC(cluster);
            await instance.LoginXNFT();

            PlayerPrefs.SetInt(KEY, (int)cluster);
            PlayerPrefs.Save();
        }

        private async Task LoginChecker()
        {
            string password = passwordField.value;
            Account account = await Web3.Instance.LoginInGameWallet(password);
            CheckAccount(account);
        }

        private async Task LoginCheckerWeb3Auth(Provider provider)
        {
            Account account;
            try
            {
                account = await Web3.Instance.LoginWeb3Auth(provider);
            }
            catch
            {
                // User cancelled login.
                account = null;
            }
            CheckAccount(account);
        }

        private async Task LoginWalletAdapter()
        {
            Account account;
            try
            {
                account = await Web3.Instance.LoginWalletAdapter();
            }
            catch
            {
                // Use cancelled sync.
                account = null;
            }
            CheckAccount(account);
        }

        private void CheckAccount(Account account)
        {
            if (account != null)
            {
                incorrectPasswordLabel.visible = false;
                PanelsManager.OnLogged();
            }
            else
            {
                passwordField.value = string.Empty;
                incorrectPasswordLabel.visible = true;
            }
        }

        private static string GetCustomRPC(RpcCluster cluster) => cluster switch
        {
            RpcCluster.MainNet => "https://rpc.magicblock.app/mainnet/",
            RpcCluster.DevNet => "https://rpc.magicblock.app/testnet/",
            RpcCluster.TestNet => "https://rpc.magicblock.app/devnet/",
            _ => "http://localhost:8899",
        };

        private static string GetWebSocketsRPC(RpcCluster cluster) => cluster switch
        {
            RpcCluster.MainNet => "wss://rpc.magicblock.app/mainnet/",
            RpcCluster.DevNet => "wss://rpc.magicblock.app/testnet/",
            RpcCluster.TestNet => "wss://rpc.magicblock.app/devnet/",
            _ => "ws://localhost:8900",
        };
    }
}