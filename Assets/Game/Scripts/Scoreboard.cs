using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField] private MeshRenderer scoreboardRenderer;

    [SerializeField] private float pointerReachTime = 5f;

    public float GetScoreboardHeight()
    {
        return scoreboardRenderer.bounds.size.y;
    }
    public float GetPointerReachTimeToTop()
    {
        return pointerReachTime;
    }
}
