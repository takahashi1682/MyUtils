namespace MyUtils.AnimatorUtils
{
    public class AnimatorBinder : AbstractTargetBehaviour<UnityEngine.Animator>
    {
        public string ParameterName = "Idle";

        public void SetBoolParameter(bool value) => Target.SetBool(ParameterName, value);
        public void SetIntParameter(int value) => Target.SetInteger(ParameterName, value);
        public void SetFloatParameter(float value) => Target.SetFloat(ParameterName, value);
        public void SetTriggerParameter() => Target.SetTrigger(ParameterName);
    }
}