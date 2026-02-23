using UnityEngine;

namespace MyUtils.AnimatorUtils
{
    public class AnimatorBinder : AbstractTargetBehaviour<Animator>
    {
        public string ParameterName = "Idle";
        public int MinIntParameterValue = 0;
        public int MaxIntParameterValue = 4;

        private int? _parameterHash;
        private int ParamHash => _parameterHash ??= Animator.StringToHash(ParameterName);

        public void SetBoolParameter(bool value) => Target.SetBool(ParamHash, value);
        public void SetIntParameter(int value) => Target.SetInteger(ParamHash, value);
        public void SetFloatParameter(float value) => Target.SetFloat(ParamHash, value);
        public void SetTriggerParameter() => Target.SetTrigger(ParamHash);

        public void SwitchBoolParameter() => Target.SetBool(ParamHash, !Target.GetBool(ParamHash));

        public void SetIncrementIntParameter()
        {
            int currentStep = Target.GetInteger(ParamHash);
            int nextStep = Mathf.Clamp(currentStep + 1, MinIntParameterValue, MaxIntParameterValue);
            Target.SetInteger(ParamHash, nextStep);
        }

        public void SetDecrementIntParameter()
        {
            int currentStep = Target.GetInteger(ParamHash);
            int nextStep = Mathf.Clamp(currentStep - 1, MinIntParameterValue, MaxIntParameterValue);
            Target.SetInteger(ParamHash, nextStep);
        }

        public void StepCycleParameter()
        {
            int current = Target.GetInteger(ParamHash);
            if (current <= MinIntParameterValue)
            {
                SetIncrementIntParameter();
            }
            else
            {
                SetDecrementIntParameter();
            }
        }
    }
}