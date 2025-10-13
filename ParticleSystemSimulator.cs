using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// ParticleSystemのシミュレーションを行う機能
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemSimulator : AbstractTargetBehaviour<ParticleSystem>
    {
        public float SimulationTime = 10f;

        protected override void Start()
        {
            base.Start();
            Target.Simulate(SimulationTime);
            Target.Play();
        }
    }
}