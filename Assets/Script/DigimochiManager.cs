using System.Collections.Generic;
using UnityEngine;

public class DigimochiManager : MonoBehaviour
{
    [SerializeField]
    private List<DigimochiDataProxy> userDigimochis;

    void Start()
    {
        userDigimochis = new List<DigimochiDataProxy>();
        SyncUserDigimochis();
        // Cuando termine de cargar... hacemos GetFirstDigimochi
        // Provisoriamente
    }

    public void SyncUserDigimochis()
    {
        // Aqu� llamas a la API o la clase que te permite obtener los Digimochis del usuario desde la blockchain.
        // A�ade cada Digimochi obtenido a la lista userDigimochis.
    }

    public DigimochiDataProxy GetFirstDigimochiData()
    {
        if (userDigimochis.Count > 0)
            return userDigimochis[0];
        else
            return null; // El usuario no tiene Digimochis.
    }
}
