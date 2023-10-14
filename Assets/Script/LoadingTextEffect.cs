using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingTextEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float animationSpeed = 1.0f;
    private string originalText;
    private int dotsCount = 0;

    private void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TMP_Text>();
        }

        if (textMeshPro != null)
        {
            originalText = textMeshPro.text;

            StartCoroutine(AnimateLoadingText());
        }
        else
        {
            Debug.LogError("No se encontró un componente TextMeshProUGUI asignado.");
        }
    }

    private IEnumerator AnimateLoadingText()
    {
        while (true)
        {
            // Construye el texto con los puntos.
            textMeshPro.text = originalText;

            // Incrementa la cantidad de puntos y reinicia si alcanza 3.
            dotsCount = (dotsCount + 1) % 4;

            yield return new WaitForSeconds(1.0f / animationSpeed);
        }
    }
}
