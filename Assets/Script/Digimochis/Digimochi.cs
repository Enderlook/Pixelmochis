using UnityEngine;
using UnityEngine.U2D.Animation;

public class Digimochi : MonoBehaviour
{
    [SerializeField] private SpriteLibrary animationSpriteLibrary;

    private DigimochiSO digimochiType;
    private IDigimochiData digimochiData;

    private void Start()
    {
        Initialize();
    }

    public void SetDigimochiType(DigimochiSO type)
    {
        digimochiType = type;
    }

    public void SetDigimochiData(IDigimochiData data)
    {
        digimochiData = data;
    }

    public void Initialize() 
    {
        gameObject.name = $"Digimochi_{digimochiType.digimochiType}";
        animationSpriteLibrary.spriteLibraryAsset = digimochiType.spriteLibraryIdleAnimation;
    }
}