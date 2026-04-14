using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TrackAndFireAgent))]
public class TrackAndFireAgentEditor : Editor
{
    private readonly string[] detectionTypeTabs = { "Global", "Radar" };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

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