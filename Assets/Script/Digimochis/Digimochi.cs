using DG.Tweening;

using EasyButtons;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Pixelmotchis
{
    public class Digimochi : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private Animator[] animators;
        [SerializeField] private SpriteLibrary animationSpriteLibrary;

        [Header("State Thresholds")]
        [SerializeField, Min(1), Tooltip("Time in minutes to get dirty, is random value")]
        private Vector2Int timeToGetDirty = new(600, 2400);

        [SerializeField, Min(1), Tooltip("Time in minutes to get hungry")]
        private Vector2Int timeToGetHungry = new(30, 600);

        [SerializeField, Min(1), Tooltip("Time in minutes to get sick")]
        private Vector2Int timeToGetSick = new(800, 1080);

        [Header("States")]
        [SerializeField] private List<DigimochiState> states;

        [SerializeField] private ParticleSystem bathParticles;
        [SerializeField] private ParticleSystem loveParticles;
        [SerializeField] private GameObject manzana;
        [SerializeField] private GameObject medicine;

        [SerializeField] private float delayToDoAction = 1f;

        private DigimochiSO digimochiSO;
        private IDigimochiData digimochiData;
        private HapinessBarController hapinessBar;
        private bool isActive;
        private bool isDigimochiPerfomingAction;

        public event Action ActionPerformed;
        public event Action ActionFinished;
        public event Action ActionFailed;

        public enum DigimochiAnimations
        {
            Idle,
            Feed,
            Cure,
            Pet,
            Dance
        }

        public IDigimochiData DigimochiData => digimochiData;
        public DigimochiSO DigimochiSO => digimochiSO;
        public bool IsPerfomingAction => isDigimochiPerfomingAction;

        private void Start()
        {
            Initialize();
            UpdateCurrentState();

            ActionPerformed += OnDigimichiPerfomAction;
            ActionFinished += OnDigimichiFinishAction;
            ActionFailed += OnDigimichiActionFailed;
        }

        private void OnDigimichiPerfomAction()
        {
            isDigimochiPerfomingAction = true;
        }

        private void OnDigimichiFinishAction()
        {
            isDigimochiPerfomingAction = false;
        }

        private void OnDigimichiActionFailed()
        {
            Debug.LogError("Action failed");
            isDigimochiPerfomingAction = false;
        }

        public void ActiveDigimochi()
        {
            isActive = true;
            UpdateCurrentState();
        }

        public void DisableDigimochi()
        {
            isActive = false;
        }

        private void UpdateCurrentState()
        {
            SetState(DigimochiState.StateTypes.Hungry, IsHungry());
            SetState(DigimochiState.StateTypes.Sick, IsSick());
            SetState(DigimochiState.StateTypes.Dirty, IsDirty());

            UpdateHapinessBar();
        }

        private void UpdateHapinessBar()
        {
            //TODO: Definir mejor las condiciones que manejan la felicidad
            var statesActiveCount = states.Where(x => x.IsStateActive).Count();

            if (statesActiveCount == 0)
            {
                hapinessBar.SetHapinessState
                   (HapinessBarController.HapinessState.VeryHappy);

                return;
            }

            if (statesActiveCount == 1)
            {
                hapinessBar.SetHapinessState
                   (HapinessBarController.HapinessState.Happy);

                return;
            }

            if (statesActiveCount == 2)
            {
                hapinessBar.SetHapinessState
                   (HapinessBarController.HapinessState.Normal);

                return;
            }

            if (statesActiveCount >= 3)
            {
                hapinessBar.SetHapinessState
                   (HapinessBarController.HapinessState.Sad);

                return;
            }
        }

        [Button]
        private void SetState(DigimochiState.StateTypes stateType, bool toggle)
        {
            // Debug.Log($"Set state: {stateType} {toggle}");

            var stateToEnable = states.Find(x => x.StateType == stateType);

            if (toggle)
            {
                stateToEnable.EnableState();
            }
            else
            {
                stateToEnable.DisableState();
            }
        }

        public bool IsHungry()
        {
            return IsDateExpired(digimochiData.GetLastMealTime(), timeToGetHungry.x, timeToGetHungry.y);
        }

        public bool IsSick()
        {
            return IsDateExpired(digimochiData.GetLastMedicineTime(), timeToGetSick.x, timeToGetSick.y);
        }

        public bool IsDirty()
        {
            return IsDateExpired(digimochiData.GetLastBathTime(), timeToGetDirty.x, timeToGetSick.y);
        }

        [Button]
        public void SetAnimation(DigimochiAnimations animation)
        {
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].SetFloat("Blend", (int)animation);
            }
        }

        private bool IsDateExpired(DateTime date, int minMinutesThreshold, int maxMinutesThreshold, bool debug = false)
        {
            DateTime todayDateTime = DateTime.Now.ToUniversalTime();

            // Calculate the time difference
            TimeSpan timeSinceLastMeal = todayDateTime - date;

            bool isExpired = GetExpirationDateOf(date, (uint)minMinutesThreshold, (uint)maxMinutesThreshold) <= todayDateTime;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DateTime GetExpirationDateOf(DateTime from, uint minimumMinutes, uint maximumMinutes)
        {
            unchecked
            {
                ulong seed = ShiftSeed((ulong)((from - DateTime.UnixEpoch).TotalSeconds % ulong.MaxValue));
                ulong value = (ulong)Remap(seed, 0, ulong.MaxValue, minimumMinutes, maximumMinutes);
                return from.AddMinutes(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ShiftSeed(ulong seed)
        {
            unchecked
            {
                seed ^= seed >> 12;
                seed ^= seed << 25;
                seed ^= seed >> 27;
                seed *= 0x2545F4914F6CDD1D;
                return seed;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static decimal Remap(decimal value, decimal oldMin, decimal oldMax, decimal newMin, decimal newMax)
            => unchecked(((value - oldMin) / (oldMax - oldMin) * (newMax - newMin)) + newMin);

        //TODO: Emprolijar metodos de acciones, hacerlo generico para evitar repeticion de codigo

        [Button]
        public void Bath(Action<bool> response)
        {
            if (isDigimochiPerfomingAction)
                return;

            StartCoroutine(BathCoroutine(response));
        }

        [Button]
        public void Feed(Action<bool> response)
        {
            if (isDigimochiPerfomingAction)
                return;

            StartCoroutine(FeedCoroutine(response));
        }

        [Button]
        public void Cure(Action<bool> response)
        {
            if (isDigimochiPerfomingAction)
                return;

            StartCoroutine(CureCoroutine(response));
        }

        [Button]
        public void Dance()
        {
            if (isDigimochiPerfomingAction)
                return;

            StartCoroutine(DanceCoroutine());
        }

        [Button]
        public void Pet()
        {
            if (isDigimochiPerfomingAction)
                return;

            StartCoroutine(PetCoroutine());
        }

        private IEnumerator BathCoroutine(Action<bool> response)
        {
            ActionPerformed?.Invoke();

            yield return new WaitForSeconds(0.5f);

            Task<bool> operation = digimochiData.Bath();
            while (!operation.IsCompleted)
                yield return null;

            bool wasRunSuccessfully = operation.GetAwaiter().GetResult();

            response?.Invoke(wasRunSuccessfully);
            if (!wasRunSuccessfully)
            {
                ActionFailed?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(delayToDoAction);

            yield return new WaitForEndOfFrame();
            // Start bath animation...
            bathParticles.Play(true);
            SetAnimation(DigimochiAnimations.Pet);

            yield return new WaitForSeconds(5f);

            // Stop bath animation...
            bathParticles.Stop(true);
            SetAnimation(DigimochiAnimations.Idle);

            // Update States
            UpdateCurrentState();
            ActionFinished?.Invoke();
        }

        private IEnumerator FeedCoroutine(Action<bool> response)
        {
            ActionPerformed?.Invoke();

            yield return new WaitForSeconds(0.5f);

            Task<bool> operation = digimochiData.Feed();
            while (!operation.IsCompleted)
                yield return null;

            bool wasRunSuccessfully = operation.GetAwaiter().GetResult();

            response?.Invoke(wasRunSuccessfully);
            if (!wasRunSuccessfully)
            {
                ActionFailed?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(delayToDoAction);

            manzana.transform.position = new Vector2(0, 0.4f);
            manzana.SetActive(true);

            yield return new WaitForSeconds(0.5f);


            manzana.transform.DOMove(transform.position, 1.5f).SetEase(Ease.InSine);
            yield return new WaitForSeconds(1.5f);

            manzana.SetActive(false);
            SetAnimation(DigimochiAnimations.Feed);
            yield return new WaitForSeconds(1f);

            SetAnimation(DigimochiAnimations.Pet);
            loveParticles.Play(true);
            yield return new WaitForSeconds(2f);

            loveParticles.Stop(false);
            SetAnimation(DigimochiAnimations.Idle);

            digimochiData.Feed();
            UpdateCurrentState();
            ActionFinished?.Invoke();
        }

        private IEnumerator CureCoroutine(Action<bool> response)
        {
            ActionPerformed?.Invoke();

            yield return new WaitForSeconds(0.5f);

            Task<bool> operation = digimochiData.Cure();
            while (!operation.IsCompleted)
                yield return null;

            bool wasRunSuccessfully = operation.GetAwaiter().GetResult();

            response?.Invoke(wasRunSuccessfully);
            if (!wasRunSuccessfully)
            {
                ActionFailed?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(delayToDoAction);

            medicine.gameObject.SetActive(true);

            SetAnimation(DigimochiAnimations.Cure);
            yield return new WaitForSeconds(1.35f);

            SetAnimation(DigimochiAnimations.Idle);
            medicine.gameObject.SetActive(false);

            digimochiData.Cure();
            UpdateCurrentState();
            ActionFinished?.Invoke();
        }

        private IEnumerator DanceCoroutine()
        {
            ActionPerformed?.Invoke();

            yield return new WaitForEndOfFrame();

            SetAnimation(DigimochiAnimations.Dance);

            yield return new WaitForSeconds(2f);

            SetAnimation(DigimochiAnimations.Idle);

            UpdateCurrentState();

            ActionFinished?.Invoke();
        }

        private IEnumerator PetCoroutine()
        {
            ActionPerformed?.Invoke();
            yield return new WaitForEndOfFrame();

            SetAnimation(DigimochiAnimations.Pet);
            loveParticles.Play(true);
            yield return new WaitForSeconds(2f);
            loveParticles.Stop(false);

            SetAnimation(DigimochiAnimations.Idle);
            ActionFinished?.Invoke();
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

            //TODO: Emprolijar esto
            hapinessBar = FindAnyObjectByType<HapinessBarController>();
        }

    }
}