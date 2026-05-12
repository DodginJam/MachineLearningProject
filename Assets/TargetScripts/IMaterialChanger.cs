using ProjectEnums;
using System;
using UnityEngine;

public interface IMaterialChanger
{
    public MeshRenderer MeshRendererRef
    { get; set; }

    public Material FriendlyMaterial
    { get; set; }

    public Material EnemyMaterial
    { get; set; }

    public void SetMaterial();
}
