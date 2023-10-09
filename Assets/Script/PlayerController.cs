using EasyButtons.Editor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CursorController cursorController;

    [SerializeField]
    private Button bathButton;

    [SerializeField]
    private Button feedButton;

    [SerializeField]
    private Button cureButton;

    [SerializeField]
    private Button danceButton;

    private enum PlayerActions
    {
        Bath,
        Feed,
        Cure,
        Dance,
        Rename,
        Cancel
    }
}
