using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.Grid
{
    public class UnitMarker : MonoBehaviour
    {
        private UnitManager _unitManager;

        public Vector2Int GridPos => new(
            Mathf.FloorToInt(transform.position.x),
            Mathf.FloorToInt(transform.position.y)
        );

        private async void Awake()
        {
            _unitManager = UnitManager.Singleton;
            await _unitManager.OnLoadAsObservable.Task.AttachExternalCancellation(
                destroyCancellationToken);

            _unitManager.AddUnitAt(this);
        }
    }
}