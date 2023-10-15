using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Pixelmochis
{
    public class MuteButton : MonoBehaviour
    {
        [SerializeField] private Image buttonImage; // Referencia a la imagen del botón para cambiar su sprite
        [SerializeField] private AudioMixer audioMixer; // Referencia al Audio Mixer que deseas mutear
        [SerializeField] private Sprite mutedSprite; // Sprite cuando está silenciado
        [SerializeField] private Sprite muteSprite; // Sprite cuando no está silenciado

        private bool isMuted = false; // Estado de silencio

        private void Start()
        {
            UpdateButtonSprite();
        }

        public void ToggleMute()
        {
            isMuted = !isMuted;

            // Activa o desactiva el silencio del Audio Mixer
            if (isMuted)
            {
                audioMixer.SetFloat("MasterVolume", -80f); // Establece el volumen muy bajo (-80dB) para silenciar
            }
            else
            {
                audioMixer.ClearFloat("MasterVolume"); // Borra el ajuste para restaurar el volumen original
            }

            // Actualiza el sprite del botón
            UpdateButtonSprite();
        }

        private void UpdateButtonSprite()
        {
            // Cambia el sprite del botón según el estado de silencio
            buttonImage.sprite = isMuted ? mutedSprite : muteSprite;
        }
    }
}