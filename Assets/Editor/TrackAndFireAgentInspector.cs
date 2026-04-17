using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrackAndFireAgent), true)]
public class TrackAndFireAgentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TrackAndFireAgent agent = (TrackAndFireAgent)target;

        if (GUILayout.Button("Apply Detection Type"))
        {
            if (Application.isPlaying)
            {
                agent.SetTargetDetectorType();
            }
        }
    }
}