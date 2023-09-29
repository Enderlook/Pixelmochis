using UnityEngine;
using UnityEngine.UIElements;

namespace Hackaton
{
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    public abstract class Panel : MonoBehaviour
    {
        private UIDocument document;

        public VisualElement RootVisualElement => document.rootVisualElement;

        protected virtual void Awake()
        {
            UIDocument document = this.document = GetComponent<UIDocument>();

            VisualElement rootVisualElement = document.rootVisualElement;
            rootVisualElement.visible = false;

            Button back = rootVisualElement.Query<Button>("BackButton");
            if (back != null)
            {
                back.clicked += PanelsManager.PopPanel;
            }
        }

        public virtual void Cover()
        {
            RootVisualElement.visible = false;
        }

        public virtual void Uncover()
        {
            RootVisualElement.visible = true;
        }

        public virtual void Open()
        {
            RootVisualElement.visible = true;
        }

        public virtual void Close()
        {
            RootVisualElement.visible = false;
        }
    }
}