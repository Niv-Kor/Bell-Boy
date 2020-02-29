using System.Collections.Generic;
using UnityEngine;

public class EntranceHighlightersAccessor : MonoBehaviour
{
    private List<EntranceHighlighter> highlighters;

    public IHighlightable UserInput {
        get { return highlighters.Find(x => x.HighlightType == HighlightType.UserInput); }
    }

    public IHighlightable GameIndicator {
        get { return highlighters.Find(x => x.HighlightType == HighlightType.GameIndication); }
    }

    private void Start() {
        this.highlighters = new List<EntranceHighlighter>(GetComponents<EntranceHighlighter>());
    }
}