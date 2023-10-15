using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelmochis
{
    public class OpenURL : MonoBehaviour
    {
        public string urlToOpen = "https://www.ejemplo.com"; // Cambia esta URL a la que desees abrir.

        public void OpenURLInBrowser()
        {
            Application.OpenURL(urlToOpen);
        }
    }
}