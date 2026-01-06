using UnityEngine;

namespace MyUtils.Grid
{
    public class UnitAnimation : SpriteAnimation.SpriteAnimation
    {
        [Tooltip("移動中でなくてもアニメーションを再生するかどうか")]
        public bool IsRun;
        public bool AlwaysAnimateMovement = true;
        private int _directionIndex;
        [field: SerializeField] public Vector2Int LastDirectionalPos { get; private set; } = Vector2Int.down;

        protected override void Start()
        {
            base.Start();
            SetDirection(LastDirectionalPos);
        }

        public void SetDirection(Vector2Int direction)
        {
            LastDirectionalPos = direction;
            if (direction == Vector2Int.down) _directionIndex = 0;
            else if (direction == Vector2Int.left) _directionIndex = 3;
            else if (direction == Vector2Int.right) _directionIndex = 6;
            else if (direction == Vector2Int.up) _directionIndex = 9;
        }

        protected override void SetSprite(int index)
        {
            Target.sprite = IsRun || AlwaysAnimateMovement
                ? Sprites[_directionIndex + (int)Mathf.PingPong(index, 2)] // アニメーション
                : Sprites[_directionIndex + 1]; // 静止
        }
    }
}