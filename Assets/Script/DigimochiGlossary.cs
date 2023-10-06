using UnityEngine;
using System.Collections.Generic;

public class DigimochiGlossary : MonoBehaviour
{
    // Asegúrate de asignar los ScriptableObjects de los tipos de Digimochi en el inspector de Unity.
    [SerializeField] private List<DigimochiSO> digimochiTypes;

    public DigimochiSO FindDigimochiSO(string digimochiType)
    {
        // Buscar el ScriptableObject en la lista por nombre.
        DigimochiSO foundDigimochiType = digimochiTypes.Find(type => type.digimochiType == digimochiType);

        // Comprobar si se encontró el tipo de Digimochi.
        if (foundDigimochiType != null)
        {
            return foundDigimochiType;
        }
        else
        {
            // En caso de no encontrarlo, puedes devolver null o manejarlo de otra manera según tus necesidades.
            Debug.LogError("Tipo de Digimochi no encontrado: " + digimochiType);
            return null;
        }
    }
}
