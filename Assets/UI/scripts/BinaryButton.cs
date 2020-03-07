using UnityEngine;
using DuloGames.UI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Tooltip("True to load a different scene upon click.")]
    [SerializeField] private bool loadScene = false;

    [Tooltip("The scene index to load upon click (only works when 'switchScene' is true).")]
    [SerializeField] private int loadedSceneIndex = 0;

    private CanvasGroup windowCanvas;

    public bool ClickEnabled { get; set; }

    private void Start() {
        this.ClickEnabled = true;
        if (window != null) this.windowCanvas = window.GetComponent<CanvasGroup>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(delegate() { ClickAction(); });
    }

    /// <summary>
    /// The function to invoke when the button is clicked.
    /// </summary>
    private void ClickAction() {
        if (!ClickEnabled) return;

        UIWindow.VisualState action = openOnClick ? UIWindow.VisualState.Shown : UIWindow.VisualState.Hidden;
        if (loadScene) SceneLoader.Instance.LoadScene(loadedSceneIndex);
        if (openOnClick) windowCanvas.interactable = true;
        if (pauseOnClick) GamePauser.Instance.PauseGame();
        else if (resumeOnClick) GamePauser.Instance.ResumeGame();

        if (window != null) window.ApplyVisualState(action);
    }
}