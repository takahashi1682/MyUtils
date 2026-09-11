using MyUtils.Abstract;
using UnityEngine;

namespace MyUtils
{
    public class RigidbodyImpact : AbstractTargetBehaviour<Rigidbody>
    {
        public Vector3 Force = new(0, 0, 10);
        public bool IsGlobal;

        protected override void Start()
        {
            base.Start();
            Target.linearVelocity = IsGlobal ? Force : transform.rotation * Force;
        }
    }
}