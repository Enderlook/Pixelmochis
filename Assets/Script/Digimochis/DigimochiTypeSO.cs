using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "DigimochiType", menuName = "Digimochis/DigimochiTypeSO")]
public class DigimochiTypeSO : ScriptableObject
{
    public string digimochiTypeName;
    public Sprite digimochiMainSprite;
    public SpriteLibraryAsset spriteLibraryIdleAnimation;
}
