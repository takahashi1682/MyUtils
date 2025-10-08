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

        private void Start()
        {
            _target.Simulate(SimulationTime);
            _target.Play();
        }
    }
}