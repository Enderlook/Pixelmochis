using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "DigimochiType", menuName = "Digimochis/DigimochiTypeSO")]
public class DigimochiSO : ScriptableObject
{
    public string digimochiType;
    public Sprite digimochiMainSprite;
    public SpriteLibraryAsset spriteLibraryIdleAnimation;
}
