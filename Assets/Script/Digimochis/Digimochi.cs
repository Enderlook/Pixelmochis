using EasyButtons;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Digimochi : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator[] animators;
    [SerializeField] private SpriteLibrary animationSpriteLibrary;

    [Header("State Levels")]
    [SerializeField, Range(0,100)] 
    private int hungryLevel;

    [SerializeField, Range(0, 100)] 
    private int sucietyLevel;

    [SerializeField, Range(0, 100)] 
    private int sickLevel;

    [Header("State Thresholds")]
    [SerializeField,Tooltip("Time in seconds to get dirty")] 
    private int timeToGetDirty = 86400;

    [SerializeField, Tooltip("Time in seconds to get hungry")] 
    private int timeToGetHungry = 86400;

    [SerializeField, Tooltip("Time in seconds to get sick")]
    private int timeToGetSick = 86400;

    private float life;
    private DigimochiSO digimochiSO;
    private IDigimochiData digimochiData;

    private enum DigimochiAnimations
    {
        Idle,
        Feed,
        Cure,
        Pet,
        Dance
    }

    public IDigimochiData DigimochiData => digimochiData;
    public DigimochiSO DigimochiSO => digimochiSO;


    private void Start()
    {
        Initialize();
    }

    [Button]
    private void FeedAction()
    {
        //SetAnimation(DigimochiAnimations.Feed);
    }

    [Button]
    private void CureAction()
    {

    }

    [Button]
    private void CleanAction()
    {

    }

    [Button]
    private void DanceAction()
    {

    }

    [Button]
    private  void SetAnimation(DigimochiAnimations animation)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetFloat("Blend", (int)animation);
        }
    }

    public void SetDigimochiSO(DigimochiSO type)
    {
        digimochiSO = type;
    }

    public void SetDigimochiData(IDigimochiData data)
    {
        digimochiData = data;
    }

    public void Initialize() 
    {
        gameObject.name = $"Digimochi_{digimochiSO.digimochiType}";
        animationSpriteLibrary.spriteLibraryAsset = digimochiSO.spriteLibraryIdleAnimation;
        SetAnimation(DigimochiAnimations.Idle);
    }

}