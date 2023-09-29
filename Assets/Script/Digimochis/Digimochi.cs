using UnityEngine;

public class Digimochi : MonoBehaviour
{
    // Representa que tipo de bichito es.
    [SerializeField]
    public DigimochiTypeSO digimochiType;
    // Contiene los datos especificos del bichito del jugador.
    [SerializeField]
    private IDigimochiData digimochiData;
}