using System.Collections;
using System.Threading.Tasks;

using UnityEngine;

namespace Hackaton
{
    public sealed class Async : MonoBehaviour
    {
        private static Async Instance;

        public static void Handle(Task task)
        {
            Async instance = Instance;
            if (instance == null)
            {
                Instance = instance = new GameObject("Async").AddComponent<Async>();
            }
            instance.StartCoroutine(Work());

            IEnumerator Work()
            {
                while (!task.IsCompleted)
                {
                    yield return null;
                }

                if (task.IsFaulted)
                {
                    Debug.LogException(task.Exception);
                }
            }
        }
    }
}
