using UnityEngine;

public class State : MonoBehaviour
{
    protected Digimochi digimochi;

    public void EnableState()
    {
        OnEnter();
    }

    public void DisableState()
    {
        OnExit();
    }

    protected virtual void OnEnter()
    {

    }

    protected virtual void OnExit()
    {

    }
}
