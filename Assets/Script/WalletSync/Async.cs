using System.Collections;
using System.Threading.Tasks;

using UnityEngine;

namespace Hackaton
{
    public sealed class Async : MonoBehaviour
    {
        private static Async Instance;

        public static Coroutine Handle(Task task)
        {
            Async instance = Instance;
            if (instance == null)
            {
                Instance = instance = new GameObject("Async").AddComponent<Async>();
            }
            return instance.StartCoroutine(Work());

            IEnumerator Work()
            {
                while (!task.IsCompleted)
                {
                    yield return null;
                }
                task.GetAwaiter().GetResult();
            }
        }
    }
}
