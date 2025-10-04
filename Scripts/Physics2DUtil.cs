using System.Collections.Generic;
using UnityEngine;

namespace TUtils
{
    public class Physics2DUtil
    {
        //最大ヒット回数
        private const int MaxHitCount = 10;

        public static List<Vector2> RefrectionLinePoses(Vector2 position, Vector2 direction, float length,
            LayerMask layerMask)
        {
            var points = new List<Vector2>() { position };
            var hit = Physics2D.Raycast(position, direction, length, layerMask);
            var hitcount = 0;
            while (hit)
            {
                if (hit.point != position)
                {
                    //HITした場所の情報を記録
                    position = hit.point;
                    points.Add(position);
                    length -= hit.distance;
                    direction = Vector2.Reflect(direction, hit.normal);
                }
                else
                {
                    //HITが同じ場所で発生する場合があるためそれを防ぐ目的、direction分少しずらす
                    position = hit.point + direction;
                }

                hit = Physics2D.Raycast(position, direction, length, layerMask);
                //同じ場所で複数回ヒットしてしまったとき無限ループしてしまうので
                //最大ヒット回数以上の場合はループを抜けるようにする
                hitcount += 1;
                if (hitcount > MaxHitCount) break;
            }

            points.Add(position + direction * length);
            return points;
        }
    }
}