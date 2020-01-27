using UnityEngine;
using DuloGames.UI;
using UnityEngine.UI;

public class BinaryButton : MonoBehaviour
{
    [Tooltip("The window to close upon click.")]
    [SerializeField] private UIWindow window;

    [Tooltip("True to open the window or false to hide it.")]
    [SerializeField] private bool openOnClick;

    [Tooltip("True to pause the game upon click.")]
    [SerializeField] private bool pauseOnClick;

    [Tooltip("True to resume (unpause) the game upon click.")]
    [SerializeField] private bool resumeOnClick;

    private CanvasGroup windowCanvas;

    private void Start() {
        this.windowCanvas = window.GetComponent<CanvasGroup>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(delegate() { ClickAction(); });
    }

    /// <summary>
    /// The function to invoke when the button is clicked.
    /// </summary>
    private void ClickAction() {
        UIWindow.VisualState action = openOnClick ? UIWindow.VisualState.Shown : UIWindow.VisualState.Hidden;
        if (openOnClick) windowCanvas.interactable = true;
        if (pauseOnClick) GamePauser.Instance.PauseGame();
        else if (resumeOnClick) GamePauser.Instance.ResumeGame();

        window.ApplyVisualState(action);
    }
}