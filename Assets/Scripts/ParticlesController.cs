using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem partSys;

    public void StartParticle()
    {
        if (!partSys) { return; }
        partSys.Play();
    }

    public void StopParticle()
    {
        if (!partSys) { return; }
        partSys.Stop();
    }

    public void SetParticleAngle(float rotation)
    {
        ParticleSystem.ShapeModule partShape = partSys.shape;
        partShape.rotation = new Vector3(0.0f, rotation, 0.0f);
    }

    public void SetParticleStartSpeed(float speed)
    {
        ParticleSystem.MainModule partMain = partSys.main;
        partMain.startSpeed = speed;
    }

    public void SetParticleEmissionRate(float rate)
    {
        ParticleSystem.EmissionModule partEmi = partSys.emission;
        partEmi.rateOverTime = rate;
    }

    public bool isPartSysPlaying()
    {
        return partSys.isPlaying;
    }
}
