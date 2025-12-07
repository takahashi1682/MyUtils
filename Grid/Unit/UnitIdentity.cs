using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.Grid
{
    public class UnitIdentity : MonoBehaviour
    {
        public Vector2Int GridPos => new(
            Mathf.FloorToInt(transform.position.x),
            Mathf.FloorToInt(transform.position.y)
        );

        private void Awake()
        {
            UnitManager.AddUnitAt(this);
        }
    }
}