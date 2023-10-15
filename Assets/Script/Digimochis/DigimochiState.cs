using UnityEngine;

namespace Pixelmochis
{
    public class DigimochiState : MonoBehaviour
    {
        public enum StateTypes
        {
            Hungry,
            Dirty,
            Sick
        }

        [SerializeField] private StateTypes stateType;

        EnviromentController enviromentController;

        protected Digimochi digimochi;
        protected bool isStateActive;

        public StateTypes StateType => stateType;
        public bool IsStateActive => isStateActive;

        private void Awake()
        {
            Initialize();
        }

        public void EnableState()
        {
            OnEnter();
            isStateActive = true;
            enviromentController.SetEnviromentState(stateType, true);
        }

        public void DisableState()
        {
            OnExit();
            isStateActive = false;
            enviromentController.SetEnviromentState(stateType, false);
        }

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnExit()
        {

        }

        private void Initialize()
        {
            //TODO: Emprolijar esto
            enviromentController = FindObjectOfType<EnviromentController>();
        }
    }
}