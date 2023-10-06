using EasyButtons;
using System;
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
    [SerializeField, Min(1),Tooltip("Time in days to get dirty")] 
    private int daysToGetDirty = 1;

    [SerializeField,Min(1), Tooltip("Time in days to get hungry")] 
    private int daysToGetHungry = 1;

    [SerializeField, Min(1), Tooltip("Time in days to get sick")]
    private int daysToGetSick = 1;

    [Header("States")]
    [SerializeField] private State feedState;


    private float life;
    private DigimochiSO digimochiSO;
    private IDigimochiData digimochiData;

    public enum DigimochiAnimations
    {
        Idle,
        Feed,
        Cure,
        Pet,
        Dance
    }

    private enum DigimochiStates
    {
        Hungry,
        Dirty,
        Sick
    }

    public IDigimochiData DigimochiData => digimochiData;
    public DigimochiSO DigimochiSO => digimochiSO;


    private void Start()
    {
        Initialize();

        //TO DO: States

        Debug.Log($"Is Hungry? {IsHungry()}");
        // if IsHungry() Enable State Hungry

        Debug.Log($"Is Sick? {IsSick()}");
        // if IsSick() Enable State Hungry

        Debug.Log($"Is Dirty? {IsDirty()}");
        // if IsDirty() Enable State Hungry
    }


    private bool IsHungry()
    {
        return IsDateExpired(digimochiData.GetLastMealTime(), daysToGetHungry);
    }

    private bool IsSick()
    {
        return IsDateExpired(digimochiData.GetLastMedicineTime(), daysToGetSick);
    }

    private bool IsDirty()
    {
        return IsDateExpired(digimochiData.GetLastBathTime(), daysToGetDirty);
    }


    [Button]
    private void FeedAction()
    {
        feedState.EnableState();
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
    public void SetAnimation(DigimochiAnimations animation)
    {
        for (int i = 0; i < animators.Length; i++)
        {
            animators[i].SetFloat("Blend", (int)animation);
        }
    }
    private bool IsDateExpired(DateTime date, int daysTreshold, bool debug = false)
    {
        DateTime todayDateTime = DateTime.Now;

        // Calculate the time difference
        TimeSpan timeSinceLastMeal = todayDateTime - date;

        // Check if the time difference is greater than daysToGetHungry
        bool isExpired = timeSinceLastMeal.TotalDays > daysTreshold;

        if (debug)
        {
            if (isExpired)
            {
                Debug.Log("Date expired");
            }
            else
            {
                Debug.Log("The date did not expire");
            }

            Debug.Log("Start date: " + date.ToString("dd/MM/yyyy HH:mm:ss"));
            Debug.Log("Today's date: " + todayDateTime.ToString("dd/MM/yyyy HH:mm:ss"));
            Debug.Log("Number of days between start and today: " + (int)timeSinceLastMeal.TotalDays);
        }

        return isExpired;
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