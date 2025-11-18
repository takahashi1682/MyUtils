using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.Grid
{
    public class UnitLookDirection : MonoBehaviour, IUnitEvent
    {
        [SerializeField] protected UnitAnimation _unitAnimation;
        [SerializeField] protected Vector2Int _lookDirection = Vector2Int.up;

        public UniTask OnInteract()
        {
            _unitAnimation.SetDirection(_lookDirection);
            return UniTask.CompletedTask;
        }
    }
}