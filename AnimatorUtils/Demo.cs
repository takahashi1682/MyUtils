using UnityEngine;

namespace MyUtils.AnimatorUtils
{
    public class Demo : MonoBehaviour
    {
        public AnimatorStateObserver StateObserver;
        [SerializeField] private TMPro.TextMeshProUGUI _text;

        private void Start()
        {
            // StateObserver.OnUpdateBlend("Blend Rotation", "Right Rotation").Subscribe(_ =>
            // {
            //     Debug.Log("Enter => " + _);
            // }).AddTo(this);
            
            // StateObserver.CurrentStateHash.Subscribe(x => Debug.Log(x)).AddTo(this);
            // StateObserver.CurrentStateHash2.Subscribe(x => Debug.Log(x)).AddTo(this);
            //
            // StateObserver.OnEntryState("Idle").Subscribe(_ =>
            // {
            //     Debug.Log("Enter => Idle");
            // }).AddTo(this);
            //
            // StateObserver.OnExitState("Idle").Subscribe(_ =>
            // {
            //     Debug.Log("Exit => Idle");
            // }).AddTo(this);
            //
            // StateObserver.OnEntryState("Left Rotation").Subscribe(_ =>
            // {
            //     Debug.Log("Enter => Left Rotation");
            // }).AddTo(this);
            //
            // StateObserver.OnExitState("Left Rotation").Subscribe(_ =>
            // {
            //     Debug.Log("Exit => Left Rotation");
            // }).AddTo(this);
            //     
            // StateObserver.OnEntryState("Right Rotation").Subscribe(_ =>
            // {
            //     Debug.Log("Enter => Right Rotation");
            // }).AddTo(this);
            //
            // StateObserver.OnExitState("Right Rotation").Subscribe(_ =>
            // {
            //     Debug.Log("Exit => Right Rotation");
            // }).AddTo(this);
        }
    }
}