using UnityEngine;
using UnityEngine.UI;

namespace Pixelmochis
{
    public class ButtonSoundPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;

        private Button[] buttons;

        private void Start()
        {
            Invoke(nameof(FindButtons), 1f);
        }

        private void FindButtons()
        {
            buttons = FindObjectsOfType<Button>(true);

            foreach (var button in buttons)
            {
                button.onClick.AddListener(OnPressedButton);
            }
        }

        private void OnPressedButton()
        {
            audioSource.Play();
        }
    }
}