using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TransitionThread))]
[RequireComponent(typeof(CanvasGroup))]
public class SceneLoader : Singleton<SceneLoader>
{
    [Tooltip("The time it takes to transition between two scenes (in seconds).")]
    [SerializeField] private float transitionTime;

    private TransitionThread transitionThread;

    private void Start() {
        this.transitionThread = GetComponent<TransitionThread>();
        transitionThread.Transist(true, transitionTime);
    }

    /// <summary>
    /// Load a scene.
    /// </summary>
    /// <param name="sceneIndex">The loaded scene's index</param>
    public void LoadScene(int sceneIndex) {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    /// <summary>
    /// A corouitine for loading a scene.
    /// </summary>
    /// <param name="sceneIndex">The loaded scene's index</param>
    /// <returns>The coroutine of loading the scene.</returns>
    private IEnumerator LoadSceneCoroutine(int sceneIndex) {
        transitionThread.Transist(false, transitionTime);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
    }
}