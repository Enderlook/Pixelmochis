using System.Collections;
using TMPro;
using UnityEngine;

namespace Nova.Utilities
{
    public class LoadingTextEffect : MonoBehaviour
    {
        private string text = "";
        [SerializeField] private TextMeshProUGUI cargandoText;
        [SerializeField] private float delay;

        void Start()
        {
            text = cargandoText.text;
            StartCoroutine(LoadingEffect());
        }

        IEnumerator LoadingEffect()
        {
            while (true)
            {
                cargandoText.text = " " + text + ".";
                yield return new WaitForSeconds(delay);
                cargandoText.text = "  " + text + "..";
                yield return new WaitForSeconds(delay);
                cargandoText.text = "   " + text + "...";
                yield return new WaitForSeconds(delay);
            }
        }
    }
}