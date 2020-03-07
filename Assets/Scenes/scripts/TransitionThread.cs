using UnityEngine;

public class TransitionThread : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float timeLerped, transitionTime, BGMVolume;
    private bool transitionIn, transitionOut;

    private void Start() {
        this.canvasGroup = GetComponent<CanvasGroup>();
        this.timeLerped = 0;
        this.transitionIn = false;
        this.transitionOut = false;
        canvasGroup.alpha = 0;
    }

    private void Update() {
        if (transitionIn || transitionOut) {
            timeLerped += Time.deltaTime;

            //black screen transition
            float baseAlpha = transitionIn ? 1 : 0;
            float targetAlpha = transitionIn ? 0 : 1;
            canvasGroup.alpha = Mathf.Lerp(baseAlpha, targetAlpha, timeLerped / transitionTime);

            //bgm volume transition (only when transitioning out of the scene)
            if (transitionOut) {
                float volumeLerp = Mathf.Lerp(BGMVolume, 0, timeLerped / transitionTime);
                print("vol lerp " + volumeLerp);
                VolumeController.Instance.SetVolume(Genre.BGM, volumeLerp);
            }

            if (timeLerped >= transitionTime) {
                //return bgm back to its origin value
                if (transitionOut) {
                    FindObjectOfType<BGM>().Stop();
                    VolumeController.Instance.SetVolume(Genre.BGM, BGMVolume);
                }

                transitionIn = false;
                transitionOut = false;
            }
        }
    }

    /// <summary>
    /// Start transition effect between scenes.
    /// </summary>
    /// <param name="time">The time it takes the transition to occur</param>
    /// <param name="transistIn">True to transist into the scene or false to transist out of it</param>
    public void Transist(bool transistIn, float time) {
        timeLerped = 0;
        transitionTime = time;
        transitionIn = transistIn;
        transitionOut = !transistIn;

        if (transitionOut) {
            BGMVolume = VolumeController.Instance.GetVolume(Genre.BGM);
            print("bgm: " + BGMVolume);
        }
    }
}