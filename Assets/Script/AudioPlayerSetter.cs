using UnityEngine;

public class AudioPlayerSetter : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip originalAudioClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originalAudioClip = audioSource.clip;
    }

    // Método para cambiar el AudioClip del AudioSource
    public void ChangeAudioClip(AudioClip newAudioClip)
    {
        if (audioSource != null)
        {
            audioSource.clip = newAudioClip;
            audioSource.Play(); // Opcional: Reproducir el nuevo audio inmediatamente
        }
    }

    // Método para restaurar el AudioClip original
    public void ResetAudioClip()
    {
        if (audioSource != null)
        {
            audioSource.clip = originalAudioClip;
            audioSource.Play(); // Opcional: Reproducir el audio original inmediatamente
        }
    }
}
