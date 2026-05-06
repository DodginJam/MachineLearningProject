using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FlyingAgent), true)]
public class FlyingAgentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FlyingAgent agent = (FlyingAgent)target;

        if (GUILayout.Button("Apply Detection Type"))
        {
            if (Application.isPlaying)
            {
                // agent.SetTargetDetectorType();
            }
        }

        if (GUILayout.Button("End Episode"))
        {
            if (Application.isPlaying)
            {
                agent.EndEpisode();
            }
        }
    }
}
