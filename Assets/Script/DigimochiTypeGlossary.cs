using UnityEngine;
using System.Collections.Generic;

public class DigimochiTypeGlossary : MonoBehaviour
{
    // Aseg�rate de asignar los ScriptableObjects de los tipos de Digimochi en el inspector de Unity.
    [SerializeField] private List<DigimochiTypeSO> digimochiTypes;


    // Recibe un int y te devuelve un scriptable object con las cosas que sabe del digimochi unity
    private DigimochiTypeSO GetDigimochiType(int digimochiTypeIndex)
    {
        if (digimochiTypeIndex >= 0 && digimochiTypeIndex < digimochiTypes.Count)
        {
            DigimochiTypeSO selectedType = digimochiTypes[digimochiTypeIndex];
            return selectedType;
            // Aqu� asignas el tipo de Digimochi seleccionado a tu Digimochi.
        }
        else
        {
            Debug.LogError("�ndice de tipo de Digimochi inv�lido.");
            return null;
        }
    }
}
