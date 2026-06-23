using Alchemy.Inspector;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class ProcessingVisualStages : MonoBehaviour
{
    [SerializeField] private VisualStage[] Stages;
    [SerializeField] [ReadOnly] private int _currentStage = 0;


    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Collider _collider;

    private ParticleSystem[] _particles;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();


        _particles = GetComponentsInChildren<ParticleSystem>(true);
    }

    public void NextStage()
    {
        _currentStage++;

        if(_currentStage - 1 < Stages.Length)
            LoadStage();

        
    }

    private void LoadStage()
    {
        if (Stages[_currentStage - 1].StageMaterial)
            _meshRenderer.material = Stages[_currentStage - 1].StageMaterial;

        if (Stages[_currentStage - 1].StageMesh)
            _meshFilter.mesh = Stages[_currentStage - 1].StageMesh;

        //Maybe I could do some kind of particle effect here. Could pass through the new mesh and have the effect appear off of it
        if(Stages[_currentStage - 1].ParticleEffectOnLoad)
        {
            foreach (ParticleSystem particle in _particles)
            {
                var psShape = particle.shape;

                if (Stages[_currentStage - 1].StageMesh)
                    psShape.mesh = Stages[_currentStage - 1].StageMesh;

                particle.Play();
            }
        }
    }

    public void FinalStageParticles()
    {
        foreach(ParticleSystem particle in _particles)
        {
            particle.transform.parent = null;
            particle.transform.localScale = Vector3.one;

            var psMain = particle.main;

            //psMain.stopAction = ParticleSystemStopAction.Destroy;

            particle.Play();
        }
    }


}

[Serializable]
class VisualStage
{
    [AssetsOnly] public Material StageMaterial;
    [AssetsOnly] public Mesh StageMesh;
    public bool ParticleEffectOnLoad = false;
    //Maybe add some kind of way for the collider to change as well
}