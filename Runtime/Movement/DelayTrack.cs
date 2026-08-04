using UnityEngine;

namespace MyUtils.Movement
{
    /// <summary>
    /// 追従の方式
    /// </summary>
    public enum ETrackMode
    {
        /// <summary>追従しない</summary>
        None,

        /// <summary>毎フレーム即座に対象と同じ値にする</summary>
        Sync,

        /// <summary>Speedに応じて遅延しながら対象へ近づける</summary>
        Delay,
    }

    /// <summary>
    /// 対象(Target)の位置・回転に追従する機能。
    /// TrackMove / TrackLookそれぞれで「追従しない／即座に同期／遅延して追従」を個別に選択できる。
    /// </summary>
    public class DelayTrack : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [field: SerializeField] public float Speed { get; set; } = 5f;
        public ETrackMode TrackMove = ETrackMode.Sync;
        public ETrackMode TrackLook = ETrackMode.Sync;

        private void FixedUpdate()
        {
            if (_target == null) return;
            if (TrackMove == ETrackMode.None && TrackLook == ETrackMode.None) return;

            float t = Speed * Time.fixedDeltaTime;

            transform.position = Track(TrackMove, transform.position, _target.position, t);
            transform.rotation = Track(TrackLook, transform.rotation, _target.rotation, t);
        }

        private static Vector3 Track(ETrackMode mode, Vector3 current, Vector3 target, float t) => mode switch
        {
            ETrackMode.Sync => target,
            ETrackMode.Delay => Vector3.Lerp(current, target, t),
            _ => current
        };

        private static Quaternion Track(ETrackMode mode, Quaternion current, Quaternion target, float t) => mode switch
        {
            ETrackMode.Sync => target,
            ETrackMode.Delay => Quaternion.Lerp(current, target, t),
            _ => current
        };
    }
}