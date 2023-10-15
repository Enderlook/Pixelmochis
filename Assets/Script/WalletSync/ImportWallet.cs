using Solana.Unity.SDK;
using Solana.Unity.Wallet;

using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;

namespace Pixelmotchis
{
    public sealed class ImportWallet : Panel
    {
        [SerializeField]
        private Panel createWallet;

        private TextField passwordField;
        private TextField mnemonicsField;
        private Label errorLabel;

        protected override void Awake()
        {
            base.Awake();

            VisualElement rootVisualElement = RootVisualElement;

            passwordField = rootVisualElement.Query<TextField>("PasswordField");
            errorLabel = rootVisualElement.Query<Label>("ErrorLabel");
            mnemonicsField = rootVisualElement.Query<TextField>("MnemonicsField");

            rootVisualElement.Query<Button>("CreateButton").First().clicked += () => PanelsManager.PushPanel(createWallet);
            rootVisualElement.Query<Button>("RestoreButton").First().clicked += () => Async.Handle(Import());
        }

        public override void Open()
        {
            base.Open();

            passwordField.value = SignIn.INITIAL_PASSWORD_TEXT;
            mnemonicsField.value = string.Empty;
            errorLabel.visible = false;
        }

        public override void Uncover()
        {
            base.Open();

            passwordField.value = SignIn.INITIAL_PASSWORD_TEXT;
            mnemonicsField.value = string.Empty;
            errorLabel.visible = false;
        }

        private async Task Import()
        {
            string password = passwordField.value;
            string mnemonic = mnemonicsField.value;

            Account account = await Web3.Instance.CreateAccount(mnemonic, password);
            if (account != null)
            {
                PanelsManager.OnLogged();
            }
            else
            {
                errorLabel.visible = true;
            }
        }
    }
}