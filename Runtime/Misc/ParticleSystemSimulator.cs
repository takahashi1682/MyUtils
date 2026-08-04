using System;
using UnityEngine;
using MyUtils.Abstract;

namespace MyUtils.Misc
{
    /// <summary>
    /// ParticleSystemのシミュレーションを行う機能
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemSimulator : AbstractTargetBehaviour<ParticleSystem>
    {
        public float SimulationTime = 10f;
        
        private void OnEnable()
        {
            OnSimulate();
        }

        private void OnSimulate()
        {
            Target.Simulate(SimulationTime);
            Target.Play();
        }
    }
}