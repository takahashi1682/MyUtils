using System.IO;
using MyUtils.Grid.Core;
using UnityEditor;
using UnityEngine;

namespace MyUtils.Grid.Editor
{
    public class GridPainter2D : EditorWindow
    {
        // --- 設定値 ---
        private int _width = 50;
        private int _height = 50;
        private float _cellSize = 1f;

        // GUI編集用（確定は Update Grid Size ボタンで）
        private int _editWidth;
        private int _editHeight;
        private float _editCellSize;

        // --- 機能フラグ ---
        // _isGridActiveを置き換え、より細かく制御
        private bool _isGridDrawingEnabled = true; // グリッドの表示を制御
        private bool _isGridInteractionEnabled = true; // マウス操作（ペイント/消去）を制御
        private Grid<int> _grid;
        private bool _painting;
        private bool _erasing;
        private TextAsset _jsonAsset;
        //  private string _jsonFilePath = "Assets/grid_data.json";

        [MenuItem("Tools/Grid Painter 2D")]
        public static void Open()
        {
            GetWindow<GridPainter2D>("Grid Painter 2D");
        }

        private void OnEnable()
        {
            // 初期編集フィールドを実体と同期
            _editWidth = _width;
            _editHeight = _height;
            _editCellSize = _cellSize;

            SceneView.duringSceneGui += OnSceneGUI;
            InitGrid();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void InitGrid()
        {
            // 実際に使うサイズを保証
            if (_width < 1) _width = 1;
            if (_height < 1) _height = 1;
            if (_cellSize <= 0f) _cellSize = 1f;

            // Grid<int> の定義が不明ですが、Row, Column の順番と仮定
            _grid = new Grid<int>(_height, _width);
        }

        private void OnGUI()
        {
            GUILayout.Label("2D Grid Painter", EditorStyles.boldLabel);

            // --- ON/OFF 切り替え (表示と入力を分離) ---
            GUILayout.BeginHorizontal();
            GUILayout.Label("Grid Drawing (Display)", GUILayout.Width(150));
            _isGridDrawingEnabled = EditorGUILayout.Toggle(_isGridDrawingEnabled);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Grid Interaction (Input)", GUILayout.Width(150));
            _isGridInteractionEnabled = EditorGUILayout.Toggle(_isGridInteractionEnabled);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
       
            _jsonAsset = (TextAsset)EditorGUILayout.ObjectField(_jsonAsset, typeof(TextAsset), false);

            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("保存"))
            {
                ExportToJson();
            }

            if (GUILayout.Button("読み込み"))
            {
                ImportFromJson();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            // GUIは編集用フィールドを使う（確定は Update ボタン）
            _editWidth = EditorGUILayout.IntField("Width", _editWidth);
            _editHeight = EditorGUILayout.IntField("Height", _editHeight);
            _editCellSize = EditorGUILayout.FloatField("Cell Size", _editCellSize);

            // 最小値を一時的に制限（UI上の不正値で困らないように）
            if (_editWidth < 1) _editWidth = 1;
            if (_editHeight < 1) _editHeight = 1;
            if (_editCellSize <= 0f) _editCellSize = 1f;


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("初期化"))
            {
                // Initialize は「編集値」を実体に適用して初期化する（分かりやすさのため）
                _width = _editWidth;
                _height = _editHeight;
                _cellSize = _editCellSize;
                InitGrid();
            }

            if (GUILayout.Button("更新"))
            {
                // Update は既存データを保ちつつサイズを変更する
                UpdateGridSizeFromEditFields();
            }

            GUILayout.EndHorizontal();


            EditorGUILayout.HelpBox("※ Width/Height を編集後は必ず [Update Grid Size] を押してください。\nSceneビューで左クリックで追加、右クリックで削除",
                MessageType.Info);
        }

        private void UpdateGridSizeFromEditFields()
        {
            // 入力値を検証してから適用
            int newWidth = Mathf.Max(1, _editWidth);
            int newHeight = Mathf.Max(1, _editHeight);
            float newCellSize = Mathf.Max(0.0001f, _editCellSize);

            // もしグリッドが null なら単純に初期化
            if (_grid == null)
            {
                _width = newWidth;
                _height = newHeight;
                _cellSize = newCellSize;
                InitGrid();
                return;
            }

            // 新しいGridを作ってできるだけデータを引き継ぐ
            var newGrid = new Grid<int>(newHeight, newWidth);
            int minW = Mathf.Min(newWidth, _grid.ColumnCount);
            int minH = Mathf.Min(newHeight, _grid.RowCount);

            for (int y = 0; y < minH; y++)
            {
                for (int x = 0; x < minW; x++)
                {
                    // Debug.Log($"{y} {x}"); // デバッグログを削除
                    int a = _grid[y, x];
                    newGrid[y, x] = a;
                }
            }

            // 適用
            _grid = newGrid;
            _width = newWidth;
            _height = newHeight;
            _cellSize = newCellSize;

            // 編集フィールドも同期（安全）
            _editWidth = _width;
            _editHeight = _height;
            _editCellSize = _cellSize;

            Repaint();
            SceneView.RepaintAll();
            Debug.Log($"🔄 Grid size updated: {_width}x{_height} (cellSize: {_cellSize})");
        }

        // **注意**: ご提示のコードでは Grid<T> クラスが JsonUtility.ToJson(_grid, true); でシリアライズできる前提ですが、
        // 実際には Grid<T> をそのままシリアライズできない可能性が高いため、ExportToJson はラッパー処理を推奨します。
        // ここでは、ご提示のロジックを保持します。
        private void ExportToJson()
        {
            if (_grid == null)
            {
                Debug.LogWarning("Grid が未初期化です。");
                return;
            }

            // Grid を JsonUtility でシリアライズできる形にすること（ここでは Grid がシリアライズ可能と仮定）
            string json;
            try
            {
                json = JsonUtility.ToJson(_grid, true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to serialize grid: {ex.Message}");
                return;
            }

            if (_jsonAsset == null)
            {
                Debug.LogWarning("JSONアセットが未設定です");
                return;
            }

            var jsonFilePath = AssetDatabase.GetAssetPath(_jsonAsset);

            try
            {
                File.WriteAllText(jsonFilePath, json);
                AssetDatabase.Refresh();
                Debug.Log($"✅ JSON Exported: {jsonFilePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to write JSON: {ex.Message}");
            }
        }

        private void ImportFromJson()
        {
            var jsonFilePath = AssetDatabase.GetAssetPath(_jsonAsset);
            if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
            {
                Debug.LogWarning("JSONファイルパスが無効です");
                return;
            }

            string json = File.ReadAllText(jsonFilePath);
            try
            {
                // JsonUtility.FromJson<Grid<int>> が使えない場合はラッパーを使う必要あり
                var loaded = JsonUtility.FromJson<Grid<int>>(json);
                if (loaded == null || loaded.RowCount <= 0 || loaded.ColumnCount <= 0)
                {
                    Debug.LogWarning("JSONの内容が不正です。");
                    return;
                }

                _grid = loaded;
                // 実際のサイズを更新（json 内のサイズを信頼）
                _width = _grid.ColumnCount;
                _height = _grid.RowCount;
                _editWidth = _width;
                _editHeight = _height;
                Repaint();
                SceneView.RepaintAll();
                Debug.Log($"✅ JSON Imported: {jsonFilePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse JSON: {ex.Message}");
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            // --- 全体の描画と入力の有効化チェック ---
            // 表示も入力も不要な場合はRepaintしない
            if (!_isGridDrawingEnabled && !_isGridInteractionEnabled)
            {
                return;
            }

            if (_grid == null) return;

            var e = Event.current;
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float t = -ray.origin.z / ray.direction.z;
            var world = ray.origin + ray.direction * t;

            // --- 表示オフセット（左下0.5セルずらす） ---
            var offset = new Vector3(-0.5f * _cellSize, -0.5f * _cellSize, 0f);

            // クリック判定もオフセット分ずらす
            var adjustedWorld = world - offset;
            int gx = Mathf.FloorToInt(adjustedWorld.x / _cellSize);
            int gy = Mathf.FloorToInt(adjustedWorld.y / _cellSize);

            // 範囲内判定
            bool inBounds = gx >= 0 && gy >= 0 && gx < _width && gy < _height;


            // --- 描画処理 ---
            if (_isGridDrawingEnabled)
            {
                // ホバー枠（黄色）
                Handles.color = Color.yellow;
                var hoverCenter = new Vector3((gx + 0.5f) * _cellSize, (gy + 0.5f) * _cellSize, 0f) + offset;
                Handles.DrawWireCube(hoverCenter, Vector3.one * _cellSize);

                // セル描画（緑色）
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        // Gridのアクセスを [y, x] に統一
                        if (_grid[y, x] == 1)
                        {
                            Handles.color = new Color(0, 1, 0, 0.4f);
                            var center = new Vector3((x + 0.5f) * _cellSize, (y + 0.5f) * _cellSize, 0f) + offset;

                            Handles.DrawSolidRectangleWithOutline(
                                new Vector3[]
                                {
                                    new(center.x - _cellSize * 0.5f, center.y - _cellSize * 0.5f, 0f),
                                    new(center.x + _cellSize * 0.5f, center.y - _cellSize * 0.5f, 0f),
                                    new(center.x + _cellSize * 0.5f, center.y + _cellSize * 0.5f, 0f),
                                    new(center.x - _cellSize * 0.5f, center.y + _cellSize * 0.5f, 0f),
                                },
                                new Color(0, 1, 0, 0.3f),
                                Color.green
                            );
                        }
                    }
                }
            }
            // ------------------


            // --- 入力操作処理 ---
            if (_isGridInteractionEnabled)
            {
                // マウス操作（範囲内のみ）
                if (inBounds)
                {
                    switch (e.type)
                    {
                        case EventType.MouseDown when e.button == 0:
                            _painting = true;
                            _erasing = false;
                            SetCell(gx, gy, 1);
                            e.Use();
                            break;

                        case EventType.MouseDown when e.button == 1:
                            _erasing = true;
                            _painting = false;
                            SetCell(gx, gy, 0);
                            e.Use();
                            break;

                        case EventType.MouseUp:
                            _painting = false;
                            _erasing = false;
                            break;

                        case EventType.MouseDrag:
                        {
                            if (_painting) SetCell(gx, gy, 1);
                            if (_erasing) SetCell(gx, gy, 0);
                            // マウス操作の入力を消費
                            if (_painting || _erasing) e.Use();
                            break;
                        }
                    }
                }

                // マウス操作がない場合でも、SceneViewの制御を奪われないようにする
                if (e.type == EventType.Layout || e.type == EventType.Repaint)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                }
            }
            // --------------------

            SceneView.RepaintAll();
        }

        private void SetCell(int x, int y, int value)
        {
            if (_grid == null) return;
            // x, y の境界チェックは OnSceneGUI で行われているため、ここでは Row/Column のアクセスに集中
            // Grid アクセスは [y, x] (Row, Column) を想定
            if (x < 0 || y < 0 || y >= _grid.RowCount || x >= _grid.ColumnCount) return; // 最終チェック

            if (_grid[y, x] != value)
            {
                Undo.RecordObject(this, "Grid Paint");
                _grid[y, x] = value;
                EditorUtility.SetDirty(this);
            }
        }
    }
}