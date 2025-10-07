using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// ParticleSystemのシミュレーションを行う機能
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemSimulator : MonoBehaviour
    {
        public float SimulationTime = 10f;

        private void Start()
        {
            if (TryGetComponent(out ParticleSystem particle))
            {
                particle.Simulate(SimulationTime);
                particle.Play();
            }
        }
    }
}