using Solana.Unity.SDK;
using Solana.Unity.Wallet.Bip39;

using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;

namespace Hackaton
{
    public sealed class CreateWallet : Panel
    {
        [SerializeField]
        private Panel importWallet;

        private TextField passwordField;
        private Label needsPasswordLabel;
        private TextField mnemonicsField;

        protected override void Awake()
        {
            base.Awake();

            VisualElement rootVisualElement = RootVisualElement;

            passwordField = rootVisualElement.Query<TextField>("PasswordField");
            needsPasswordLabel = rootVisualElement.Query<Label>("NeedsPasswordLabel");
            mnemonicsField = rootVisualElement.Query<TextField>("MnemonicsField");

            rootVisualElement.Query<Button>("RestoreButton").First().clicked += () => PanelsManager.PushPanel(importWallet);
            rootVisualElement.Query<Button>("CreateButton").First().clicked += () => Async.Handle(Create());
        }

        public override void Open()
        {
            base.Open();

            mnemonicsField.value = new Mnemonic(WordList.English, WordCount.TwentyFour).ToString();
            needsPasswordLabel.visible = false;
        }

        public override void Uncover()
        {
            base.Uncover();

            mnemonicsField.value = new Mnemonic(WordList.English, WordCount.TwentyFour).ToString();
            needsPasswordLabel.visible = false;
        }

        private async Task Create()
        {
            string password = passwordField.text;
            if (string.IsNullOrEmpty(password))
            {
                needsPasswordLabel.visible = true;
                return;
            }

            string mnemonic = mnemonicsField.text.Trim();
            try
            {
                await Web3.Instance.CreateAccount(mnemonic, password);
                Debug.Log("Created Account");
            }
            catch (Exception exception)
            {
                passwordField.value = exception.ToString();
            }
        }
    }
}