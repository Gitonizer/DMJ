using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Elements", menuName = "ScriptableObjects/Elements")]
public class ElementScriptable : ScriptableObject
{
    public ParticleSystem FireParticles;
    public ParticleSystem IceParticles;
    public ParticleSystem WindParticles;

    public Material FireMat;
    public Material IceMat;
    public Material WindMat;
}
