using UnityEngine;
using UnityEngine.U2D.Animation;

public class Digimochi : MonoBehaviour
{
    [SerializeField] private SpriteLibrary animationSpriteLibrary;

    private DigimochiTypeSO digimochiType;
    private IDigimochiData digimochiData;

    private void Start()
    {
        Setup();
    }

    public void SetDigimochiType(DigimochiTypeSO type)
    {
        digimochiType = type;
    }

    public void SetDigimochiData(IDigimochiData data)
    {
        digimochiData = data;
    }

    public void Setup() 
    {
        animationSpriteLibrary = GetComponent<SpriteLibrary>();
    }
}