using UnityEngine;

public class DigimochiStateDirty : DigimochiState
{
    [SerializeField] private SpriteRenderer dirtySprite;

    private void Start()
    {
        dirtySprite.enabled = false;
    }

    protected override void OnEnter()
    {
        base.OnEnter();

        dirtySprite.enabled = true;
    }

    protected override void OnExit()
    {
        base.OnExit();

        dirtySprite.enabled = false;
    }

}
