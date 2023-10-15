using EasyButtons;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Pixelmochis
{
    public class EnviromentController : MonoBehaviour
    {
        [SerializeField] private List<Enviroment> enviroments;

        [Serializable]
        private struct Enviroment
        {
            public DigimochiState.StateTypes state;
            public GameObject[] objects;
        }

        [Button]
        public void SetEnviromentState(DigimochiState.StateTypes state, bool value)
        {
            var selectedEnviroment = enviroments.Find(x => x.state == state);

            if (!selectedEnviroment.Equals(null))
            {
                SetEnviroment(selectedEnviroment, value);
            }
        }

        private void SetEnviroment(Enviroment enviroment, bool value)
        {
            foreach (var obj in enviroment.objects)
            {
                obj.SetActive(value);
            }
        }
    }
}