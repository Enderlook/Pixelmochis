using Solana.Unity.SDK;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Pixelmochis
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class PanelsManager : MonoBehaviour
    {
        private static PanelsManager Instance;

        [SerializeField]
        private Panel initialPanel;

        private readonly Stack<Panel> panels = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Can't have multiples {nameof(PanelsManager)}.", this);
                return;
            }

            Instance = this;

            if (Web3.Instance != null)
            {
                Work();
            }
            else
            {
                Web3.OnWalletInstance += Work;
            }

            void Work()
            {
                GetComponent<UIDocument>().rootVisualElement.visible = false;
                PushPanel(initialPanel);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PopPanel();
            }
        }

        public static void PushPanel(Panel panel)
        {
            PanelsManager instance = Instance;
            Stack<Panel> panels = instance.panels;
            if (panels.TryPeek(out Panel previous))
            {
                previous.Cover();
            }

            panels.Push(panel);
            panel.Open();
        }

        public static void PopPanel()
        {
            Stack<Panel> panels = Instance.panels;
            if (panels.Count > 1)
            {
                panels.Pop().Close();
                panels.Peek().Uncover();
            }
        }

        public static void OnLogged()
        {
            SceneManager.LoadSceneAsync("Game");
        }
    }
}