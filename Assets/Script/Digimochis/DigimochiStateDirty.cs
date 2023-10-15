using UnityEngine;

namespace Pixelmotchis
{
    public class DigimochiStateDirty : DigimochiState
    {
        [SerializeField] private SpriteRenderer dirtySprite;

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
}