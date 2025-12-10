using UnityEngine;

namespace MyUtils.Grid.Unit
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