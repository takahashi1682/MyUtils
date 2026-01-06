using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

namespace MyUtils.Grid.Map
{
    public sealed class Node
    {
        public int Value { get; set; }
        public Vector2 Pos { get; set; }

        public Node(int value, Vector2 pos)
        {
            Value = value;
            Pos = pos;
        }
    }

    // public sealed class Step
    // {
    //     public Vector2Int Pos { get; set; }
    //     public int Direction { get; set; }
    //
    //     public Step(Vector2Int pos, int direction)
    //     {
    //         Pos = pos;
    //         Direction = direction;
    //     }
    //
    //     public static int DirectionReverse(int direction)
    //     {
    //         return direction switch
    //         {
    //             0 => 2,
    //             1 => 3,
    //             2 => 0,
    //             3 => 1,
    //             _ => -1
    //         };
    //     }
    // }

    public class RouteUtils : MonoBehaviour
    {
        public static readonly Vector2Int[] CardinalDirections =
        {
            Vector2Int.down, // (0, -1)
            Vector2Int.left, // (-1, 0)
            Vector2Int.right, // (1, 0)
            Vector2Int.up, // (0, 1)
        };

        [SerializeField] private MapLoader _mapLoader;
        private Grid<int> _map;

        private void Awake()
        {
            _mapLoader.OnLoadAsObservable
                .Where(grid => grid != null)
                .Subscribe(grid =>
                {
                    _map = grid;
                    Debug.Log($"✅ マップデータを受信しました ({grid.RowCount}x{grid.ColumnCount})");
                }).AddTo(this);
        }

        /// <summary>
        /// 選択したユニットの行動エリアの算出
        /// </summary>
        /// <param name="unitPos"></param>
        /// <param name="moveRange"></param>
        /// <param name="attackRange"></param>
        /// <param name="moveType"></param>
        /// <param name="armyType"></param>
        /// <returns></returns>
        // public UI.Area GetActiveArea(UnitBase unit, Vector2 unitPos, bool isMoveRange = true)
        // {
        //     // 移動エリアの検証と生成
        //     var moveArea = GetMoveArea(unit, unitPos, isMoveRange);
        //
        //     // 攻撃エリアの検証と生成
        //     var activeArea = GetAttackArea(moveArea, unit, unitPos);
        //
        //     return activeArea;
        // }

        /// <summary>
        /// 選択したユニットの移動エリアの算出
        /// </summary>
        /// <param name="unitPos"></param>
        /// <returns></returns>
        public Area GetMoveArea(Vector2Int unitPos)
        {
            var resultArea = new Area(_map.Height, _map.Width);

            // BFS/Flood Fill に適したキュー (次に向かう座標を保持)
            var queue = new Queue<Vector2Int>();

            // 1. 開始地点の初期化
            var startCell = resultArea[unitPos];
            startCell.CellType = CellType.Move;
            startCell.Cost = 0; // 開始地点のコストは 0

            queue.Enqueue(unitPos); // 開始座標をキューに追加

            while (queue.TryDequeue(out var currentPos))
            {
                // デキューした時点でのコストを取得
                var currentAreaCell = resultArea[currentPos];
                int newCost = currentAreaCell.Cost + 1;

                // 2. 隣接セル（上下左右）の探索
                foreach (var dir in CardinalDirections)
                {
                    var nextPos = currentPos + dir;

                    // マップ内チェック
                    if (!resultArea.IsInMap(nextPos)) continue;

                    // マップの元のセル情報（壁かどうか）を取得
                    if (_map[nextPos] == 0)
                    {
                        // 移動不可能マスの場合、Wallとしてマークしてスキップ
                        resultArea[nextPos].CellType = CellType.Wall;
                        continue;
                    }

                    // 他ユニットチェック
                    var targetUnit = UnitManager.GetPosUnit(nextPos);
                    if (targetUnit != null)
                    {
                        // 他ユニットがいるマスはUnitとしてマークしてスキップ（開始地点を除く）
                        if (resultArea[nextPos].CellType != CellType.Move)
                        {
                            resultArea[nextPos].CellType = CellType.Unit;
                            continue;
                        }
                    }

                    // 3. コストの評価と更新 (ダイクストラ法のコア)
                    var nextAreaCell = resultArea[nextPos];

                    // 既に訪問済みで、かつより短い経路が既に見つかっている場合はスキップ
                    if (nextAreaCell.CellType == CellType.Move && nextAreaCell.Cost <= newCost)
                    {
                        continue;
                    }

                    // コストを更新し、移動可能としてマークし、キューに登録
                    // (CellType.Noneの場合も、Cost > newCostは常にtrueとなる)
                    nextAreaCell.Cost = newCost;
                    nextAreaCell.CellType = CellType.Move;

                    queue.Enqueue(nextPos);
                }
            }

            return resultArea;
        }

        // /// <summary>
        // /// 選択したユニットの攻撃エリアの算出
        // /// </summary>
        // /// <param name="startPos"></param>
        // /// <param name="attackRange"></param>
        // /// <returns></returns>
        // public UI.Area GetAttackArea(UI.Area moveArea, UnitBase unit, Vector2 unitPos)
        // {
        //     UI.Area list = moveArea;
        //     if (list == null)
        //     {
        //         list = new UI.Area(_map.Height, _map.Width);
        //         list.GetData(unitPos).cellType = AreaCell.Current;
        //     }
        //
        //     int minAttackRange = unit.MinAttackRange;
        //     int maxAttackRange = unit.MaxAttackRange;
        //     var checkPos = new List<(int x, int y, int range)>();
        //     for (int x = -maxAttackRange; x <= maxAttackRange; x++)
        //     {
        //         for (int y = -maxAttackRange; y <= maxAttackRange; y++)
        //         {
        //             int absX = Math.Abs(x);
        //             int absY = Math.Abs(y);
        //             int sum = absX + absY;
        //             if (minAttackRange <= sum && sum <= maxAttackRange)
        //             {
        //                 checkPos.Add((x, y, sum));
        //             }
        //         }
        //     }
        //
        //     for (int y = 0; y < list.Height; y++)
        //     {
        //         for (int x = 0; x < list.Width; x++)
        //         {
        //             var cell = list.GetData(x, y);
        //             if (AreaCell.Attack < cell.cellType)
        //             {
        //                 foreach (var cp in checkPos)
        //                 {
        //                     int posX = x + cp.x;
        //                     int posY = y + cp.y;
        //
        //                     // 配列の外（マップ外）なら終了
        //                     if (!IsInMap(posX, posY)) continue;
        //
        //                     var checkCell = list.GetData(posX, posY);
        //                     if (checkCell.cellType == AreaCell.None)
        //                     {
        //                         checkCell.cellType = AreaCell.Attack;
        //                         checkCell.range = cp.range;
        //                         checkCell.cost = cell.cost + cp.range;
        //                     }
        //                     else if (checkCell.cellType == AreaCell.Attack && cp.range < cell.range)
        //                     {
        //                         checkCell.range = cp.range;
        //                         checkCell.cost = cell.cost + cp.range;
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     return list;
        // }

        /// <summary>
        /// 移動エリア内の特定マスまでの最短ルートを返す
        /// </summary>
        /// <param name="moveArea"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
        public static Queue<Vector2Int> GetAreaBestRoute(Area moveArea, Vector2Int startPos, Vector2Int endPos)
        {
            // 1. 同一地点なので移動する必要なし
            if (startPos == endPos) return new Queue<Vector2Int>();

            // 2. 初期設定
            var moveRoute = new Stack<Vector2Int>();
            var currentPos = endPos;

            // Areaクラスから現在のコストを取得 (CellType.Currentに到達するまで必要)
            int currentCost = moveArea[currentPos].Cost;

            // 4. 経路の逆追跡ループ
            while (currentPos != startPos)
            {
                (int cost, Vector2Int direction) bestNeighbor = (-1, Vector2Int.zero);

                // 最小コストの隣接セルを探索
                foreach (var dir in CardinalDirections)
                {
                    var nextPos = currentPos + dir;

                    // マップ内チェック
                    if (!moveArea.IsInMap(nextPos)) continue;

                    var cell = moveArea[nextPos];

                    // スタート地点に到達した場合の特別な処理
                    if (nextPos == startPos)
                    {
                        // スタート地点に到達。経路をスタックに追加してループ終了準備
                        moveRoute.Push(dir * -1);
                        goto EndLoop; // ループを抜けて経路キューに変換
                    }

                    // 移動可能かつ現在のコストより小さいセルのみを対象とする
                    if (cell.CellType == CellType.Move && cell.Cost < currentCost)
                    {
                        // 初期値設定またはより良い経路が見つかった場合
                        if (bestNeighbor.cost == -1 || cell.Cost < bestNeighbor.cost)
                        {
                            bestNeighbor = (cell.Cost, dir);
                        }
                    }
                }

                // 5. 経路が見つからなかった場合のガード
                if (bestNeighbor.cost == -1)
                {
                    return new Queue<Vector2Int>(moveRoute.ToArray());
                }

                // 6. 最適経路を更新
                var chosenDirection = bestNeighbor.direction;
                moveRoute.Push(chosenDirection * -1); // 逆方向への移動なので反転
                currentPos += chosenDirection;
                currentCost = bestNeighbor.cost; // 次のループのためにコストを更新
            }

            EndLoop:

            // StackからQueueに変換し、キューを返す
            return new Queue<Vector2Int>(moveRoute.ToArray());
        }

        /// <summary>
        /// 移動可能エリア内で、ターゲットに最も近いセルを探索。
        /// </summary>
        public static Vector2Int FindNearestMovablePoint(Area moveArea, Vector2Int targetPos)
        {
            int minDistance = int.MaxValue;
            int minCost = int.MaxValue;
            var nearestPos = targetPos;

            moveArea.ForEachCell((y, x, cell) =>
            {
                if (cell.CellType != CellType.Move) return;

                Vector2Int cellPos = new(x, y);
                int distance = GridMath.GetManhattanDistance(cellPos, targetPos);

                bool isCloser = distance < minDistance ||
                                (distance == minDistance && cell.Cost < minCost);

                if (isCloser)
                {
                    nearestPos = cellPos;
                    minDistance = distance;
                    minCost = cell.Cost;
                }
            });

            return nearestPos;
        }


        // /// <summary>
        // /// マップ内で攻撃できる一番近い敵までのルートを返す
        // /// </summary>
        // /// <param name="activeArea"></param>
        // /// <param name="unit"></param>
        // /// <returns></returns>
        // public Queue<Step> GetMapNearTargetRoute(UnitBase unit)
        // {
        //     var unitPos = unit.Pos;
        //     var maxActiveArea = GetActiveArea(unit, unitPos, false);
        //
        //     // エリア内で一番近い敵ユニット取得
        //     var targetUnit = GetAreaNearTarget(maxActiveArea, unit);
        //     if (!targetUnit) return new Queue<Step>();
        //
        //     // エリア内で一番近い敵ユニットに攻撃できるマスを取得
        //     var locations = GetAreaAttackLocations(maxActiveArea, unit, targetUnit.Pos, false);
        //     if (locations.Count == 0) return new Queue<Step>();
        //
        //     // 攻撃場所までの最短ルート取得
        //     locations.Sort((a, b) => a.Value.CompareTo(b.Value));
        //
        //     var moveRoute = GetAreaBestRoute(maxActiveArea, unitPos, locations[0].Pos).ToArray();
        //     var moveRange = unit.MoveRange;
        //
        //     int length = moveRoute.Length - 1;
        //     int i = length;
        //     for (; 0 <= i; i--)
        //     {
        //         if ((maxActiveArea.GetData(moveRoute[i].Pos).cost <= moveRange) &&
        //          (!_unitManager.GetUnit(moveRoute[i].Pos)))
        //         {
        //             break;
        //         }
        //     }
        //     return new Queue<Step>(moveRoute[0..^(length - i)]);
        // }

        /// <summary>
        /// アクティブエリア内で一番近い攻撃できる敵Unitを返す
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        // public UnitBase GetAreaNearTarget(UI.Area activeArea, UnitBase unit)
        // {
        //     // 攻撃できる敵の位置をリスト化する
        //     var list = new List<Node>();
        //     for (int y = 0; y < activeArea.Height; y++)
        //     {
        //         for (int x = 0; x < activeArea.Width; x++)
        //         {
        //             if (activeArea.cells[y, x].cellType == AreaCell.Attack)
        //             {
        //                 var targetUnit = _unitManager.GetUnitStatus(x, y);
        //                 if (targetUnit != null &&
        //                     UnitManager.IsTarget(unit.Status.armyType, targetUnit.armyType))
        //                 {
        //                     list.Add(new Node(activeArea.cells[y, x].cost, new Vector2(x, y)));
        //                 }
        //             }
        //         }
        //     }
        //
        //     // マップ上に攻撃が届く敵がいなかった。
        //     if (list.Count == 0) return null;
        //
        //     // リストを昇順降順で並び替え、一番近い敵を変えす
        //     list.Sort((a, b) => a.Value.CompareTo(b.Value));
        //
        //     return (_unitManager.GetUnit(list[0].Pos));
        // }

        /// <summary>
        /// アクティブエリア内の特定の条件のUnitを返す
        /// </summary>
        /// <param name="activeArea"></param>
        /// <param name="unitInfo"></param>
        /// <returns></returns>
        // public List<UnitBase> FindAreaUnits(UI.Area activeArea, Func<StrategicSimulation2.UI.Area.Cell, UnitBase, bool> filter)
        // {
        //     var list = new List<UnitBase>();
        //     for (int y = 0; y < activeArea.Height; y++)
        //     {
        //         for (int x = 0; x < activeArea.Width; x++)
        //         {
        //             UnitBase unit = _unitManager.GetUnit(x, y);
        //             if (unit && filter(activeArea.cells[y, x], unit))
        //             {
        //                 list.Add(unit);
        //             }
        //         }
        //     }
        //     return list;
        // }

        /// <summary>
        /// ターゲットリストの中で一番攻撃する条件の良いユニットを返す
        /// </summary>
        /// <returns>The attack target selection.</returns>
        /// <param name="targetList">Target list.</param>
        // public UnitBase GetBestAttackTarget(UnitBase unit, List<UnitBase> targetList)
        // {
        //     // リストが空ならnullを返す
        //     if (targetList.Count == 0)
        //     {
        //         return null;
        //     }
        //
        //     // 攻撃可能な全ての敵に対して、攻撃シュミレーションを行う
        //     // id(int)と評価ポイント(int)のリストを生成
        //     var list = new List<(UnitBase, int)>();
        //     foreach (var target in targetList)
        //     {
        //         // 戦闘シュミレーション
        //         int damage = UnitCalcUtils.GetAttackDamage(unit, target);
        //         int hitRate = UnitCalcUtils.GetHitRate(unit, target);
        //         int deathBlowRate = UnitCalcUtils.GetCriticalRete(unit, target);
        //         int attackCount = UnitCalcUtils.GetAttackCount(unit, target);
        //
        //         // 評価ポイントの計算( ダメージ * 攻撃回数 + 命中率 / 2 + 必殺率)
        //         list.Add((target, damage * attackCount + hitRate / 2 + deathBlowRate));
        //     }
        //
        //     // 評価点リストを降順で並び替え
        //     list.Sort((a, b) => b.Item2 - a.Item2);
        //
        //     return list[0].Item1;
        // }
        //
        // /// <summary>
        // /// 行動範囲内で、ターゲットに攻撃できる場所のリストを返す
        // /// </summary>
        // /// <param name="unit"></param>
        // /// <param name="targetPos"></param>
        // /// <returns></returns>
        // public List<Node> GetAreaAttackLocations(UI.Area activeArea, UnitBase unit, Vector2 targetPos, bool isCheckUnit)
        // {
        //     var list = new List<Node>();
        //
        //     // 下列の左側からチェックしていく
        //     Vector2 currentPos = unit.Pos;
        //     var minAttackRange = unit.MinAttackRange;
        //     var maxAttackRange = unit.MaxAttackRange;
        //     int startY = (int)targetPos.y - maxAttackRange;
        //     int startX = (int)targetPos.x - maxAttackRange;
        //     int endY = (int)targetPos.y + maxAttackRange;
        //     int endX = (int)targetPos.x + maxAttackRange;
        //     for (int y = startY; y <= endY; y++)
        //     {
        //         for (int x = startX; x <= endX; x++)
        //         {
        //             Vector2 checkPos = new Vector2(x, y);
        //
        //             // 配列の外（マップ外）は飛ばす
        //             if (!IsInMap(checkPos)) continue;
        //
        //             // 自分以外の他のユニットがいるなら飛ばす
        //             if (isCheckUnit &&
        //                 _unitManager.GetUnit(checkPos) != null &&
        //                 currentPos != checkPos)
        //             {
        //                 continue;
        //             }
        //
        //             // 攻撃が届かない範囲を除く
        //             int distance = UnitCalcUtils.GetCellDistance(checkPos, targetPos);
        //             if (distance < minAttackRange ||
        //                 maxAttackRange < distance)
        //             {
        //                 continue;
        //             }
        //
        //             // 自分のいるセルまたは移動可能セルならリストに追加する
        //             var cell = activeArea.cells[y, x];
        //             if (cell.cellType == AreaCell.Move ||
        //                 cell.cellType == AreaCell.Current)
        //             {
        //                 list.Add(new Node(cell.cost, checkPos));
        //             }
        //         }
        //     }
        //     return list;
        // }

        /// <summary>
        /// ターゲットに攻撃できる場所リストの中から、一番条件の良い位置を選択する
        /// </summary>
        /// <returns>The attack location selection.</returns>
        /// <param name="locationList">Location list.</param>
        // public Vector2 GetBestAttackLocation(List<Node> locationList)
        // {
        //     var list = new List<(int, Vector2)>();
        //     foreach (var l in locationList)
        //     {
        //         // セルの評価
        //         Cell cellInfo = _map.GetCell(l.Pos);
        //
        //         // 評価ポイントの計算
        //         int currentEval = cellInfo.Eva; // 回避率
        //         currentEval += cellInfo.Def * 10; // 防御値 * 10
        //         currentEval += l.Value * 10; // 距離 * 10
        //
        //         list.Add((currentEval, l.Pos));
        //     }
        //
        //     // 評価点リストを降順で並び替え
        //     list.Sort((a, b) => b.Item1 - a.Item1);
        //
        //     // 一番評価の高い座標を返す
        //     return list[0].Item2;
        // }

        /// <summary>
        /// 移動ルートのコストを返す
        /// </summary>
        /// <param name="moveRoot"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        // public int GetMoveRootCost(Queue<Step> moveRoot, Vector2 startPos)
        // {
        //     int cost = 0;
        //     foreach (var step in moveRoot)
        //     {
        //         cost += _map.GetCell(step.Pos).Mc;
        //     }
        //
        //     return cost;
        // }
        //
        // /// <summary>
        // /// 他のunitを通り抜けできるか
        // /// </summary>
        // /// <param name="army"></param>
        // /// <param name="targetUnit"></param>
        // /// <returns></returns>
        // public bool IsThrough(Army army, Army targetArmy)
        // {
        //     // 同じ勢力なら通過できる
        //     if (army == targetArmy) return true;
        //
        //     // 別の勢力で通過できる勢力
        //     switch (army, targetArmy)
        //     {
        //         case (Army.Player, Army.Ally): return true;
        //
        //         case (Army.Ally, Army.Player): return true;
        //     }
        //
        //     return false;
        // }
        //
        // /// <summary>
        // /// ユニットタイプ毎の移動可能セルのチェック
        // /// </summary>
        // /// <returns><c>true</c>, if moveing was ised, <c>false</c> otherwise.</returns>
        // /// <param name="terrain">Cell category.</param>
        // /// <param name="moveType">Move type.</param>
        // private bool IsPass(int terrain, MoveType moveType)
        // {
        //     return terrain <= (int)moveType;
        // }

        /// <summary>
        /// 座標がマップ内ならtrueを返す
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
    }
}