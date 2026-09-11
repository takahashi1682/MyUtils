# MyUtils

Unity用の個人ユーティリティライブラリ(UPMローカルパッケージ / `com.takahashi.myutils`)。
入力・UI演出・データ保存・DI(VContainer)・リアクティブプログラミング(R3)・オーディオなど、Unityプロジェクト横断で使う汎用機能をまとめたもの。

実際の使い方は、サンプルプロジェクト `MyUtilsProject`(`Assets/Projects/Samples` 以下)にカテゴリ別・番号順で動く実例が用意されているので、あわせて参照。

## 動作環境

- Unity 6000.5.5f1 で動作確認(それ以降のUnity 6系を想定)
- **Active Input Handling** を `Input System Package (New)` または `Both` にしておく必要がある
  (`Edit > Project Settings > Player > Other Settings > Active Input Handling`。Input Systemに依存する機能があるため、`Input Manager (Old)` のままだと動作しない)

## 依存パッケージ

`package.json` に定義されている依存関係:

| パッケージ | バージョン | 用途 |
| --- | --- | --- |
| `com.cysharp.unitask` | 2.5.11 | async/await ベースの非同期処理 |
| `org.nuget.r3` | 1.3.1 | R3(Rx)本体 |
| `com.cysharp.r3` | 1.3.1 | R3のUnity統合(TextMeshPro連携等) |
| `net.tnrd.serializableinterface` | 2.2.1 | インターフェース参照のシリアライズ |
| `jp.hadashikick.vcontainer` | 1.18.0 | DIコンテナ |

上記に加えて、Unity公式パッケージとして以下が必要(Package Managerから通常どおり追加すればよい):

- `com.unity.inputsystem`(新Input System)
- `com.unity.ugui`(TextMeshProを含む。Unity 6系では別パッケージではなくuguiに統合されている)

## インストール手順(Package Managerの設定方法)

### 1. Scoped Registry(OpenUPM)を追加する

`com.cysharp.*` / `org.nuget.*` / VContainer は OpenUPM 経由で配布されているため、Scoped Registryの登録が必要。

`Edit > Project Settings > Package Manager > Scoped Registries` を開き、`+` で以下を追加:

| 項目 | 値 |
| --- | --- |
| Name | `OpenUPM` |
| URL | `https://package.openupm.com` |
| Scope(s) | `com.cysharp`, `org.nuget`, `jp.hadashikick.vcontainer`, `net.tnrd.serializableinterface` |

保存すると `Packages/manifest.json` に以下が追記される(直接編集してもよい):

```json
"scopedRegistries": [
  {
    "name": "OpenUPM",
    "url": "https://package.openupm.com",
    "scopes": [
      "com.cysharp",
      "org.nuget",
      "jp.hadashikick.vcontainer",
      "net.tnrd.serializableinterface"
    ]
  }
]
```

### 2. 依存パッケージをインストールする

`Window > Package Manager` を開き、左上のドロップダウンを `My Registries` に切り替えると、手順1で登録したスコープ配下のパッケージが一覧に出てくる。以下をインストール:

- R3 (`com.cysharp.r3`)
- R3 (`org.nuget.r3` ※R3本体側も別途必要)
- UniTask (`com.cysharp.unitask`)
- VContainer (`jp.hadashikick.vcontainer`)

`net.tnrd.serializableinterface` はOpenUPMに公開されていないため、Package Manager左上の `+` ボタン → `Add package from git URL...` から以下を直接指定する:

```
https://github.com/Thundernerd/Unity3D-SerializableInterface.git
```

`manifest.json` に直接書く場合は以下のようになる(実プロジェクトの例):

```json
"dependencies": {
  "com.cysharp.r3": "1.3.1",
  "com.cysharp.unitask": "2.5.11",
  "net.tnrd.serializableinterface": "https://github.com/Thundernerd/Unity3D-SerializableInterface.git",
  "org.nuget.r3": "1.3.1"
}
```

### 3. MyUtils本体を追加する

**方法A: ローカルパスから追加(このリポジトリを別ディレクトリにcloneしている場合)**

Package Manager左上の `+` → `Add package from disk...` でこのリポジトリの `package.json` を選択する。
または `manifest.json` に直接パスを追記する:

```json
"com.takahashi.myutils": "file:../MyUtils"
```

(パスはcloneした場所に合わせて調整。`MyUtilsProject` では `file:C:/Users/h.takahashi/Documents/GitHub/MyUtils` のように絶対パスで指定している)

**方法B: Gitから直接追加**

```
https://github.com/takahashi1682/MyUtils.git
```

いずれの場合も、Package Managerで追加すると `com.takahashi.myutils` の `dependencies` に書かれている上記パッケージ群が連鎖的に解決される。

## フォルダ構成

```
MyUtils/
├── Runtime/    実行時スクリプト本体(namespace: MyUtils, asmdef: MyUtils.Runtime)
├── Editor/     エディタ拡張(DebugMethod属性のInspector表示、Separator等)
├── Fonts/      付随フォントアセット
├── Materials/  付随マテリアル
├── Sprites/    付随スプライト
└── package.json
```

## 機能一覧(カテゴリ別)

対応する `MyUtilsProject` のサンプル番号もあわせて記載。

### 入力・カーソル
- `InputTrigger`系: `AbstractKeyInputTrigger` / `AbstractActionInputTrigger` / `UnityEventOnKeyTrigger` / `UnityEventOnActionTrigger` / `PlaySEOnInputTrigger`(キー・アクション入力をUnityEventに変換) — Samples 01 / 02
- `Cursor`: `CursorTexture` / `CursorSetting`(カスタムカーソル表示) — Samples 03
- `MouseToWorldRaycaster` / `LookAtMouseCursor`(マウス位置のワールド座標変換と追従) — Samples 10

### 3D・カメラ・検知
- `Detector`系: `IDetector` / `IDetector2d` を実装した `RayCastDetector` / `BoxCastDetector` / `SphereCastDetector` / `CapsuleCastDetector`(3D)、`RayCast2dDetector` / `BoxCast2dDetector` / `CircleCast2dDetector` / `LineCast2dDetector`(2D)、および連続移動時のすり抜けを検知する `AbstractSweepDetector` — Samples 11
- `FPSController`: `BasicCameraController` / `BasicPlayerMove`(FPS視点カメラ・移動) — Samples 12

### UI部品
- `Gauge`: `FillSegmentGauge` / `MemoryGauge`、`UI.Gauge`(RectTransformベースのゲージ) — Samples 20
- `UIPrefsBinder`系: `SliderPrefsBinder` / `TogglePrefsBinder` / `DropdownPrefsBinder` / `InputFieldPrefsBinder`(UIとPlayerPrefsの自動バインド) — Samples 21
- `CustomScrollView<T>` / `CustomScrollViewItem`(ファン型など自由レイアウトのスクロールリスト) — Samples 22
- `ObjectGroup` / `ObjectGroupSwitcher`(オブジェクト群のタブ切り替え表示) — Samples 23
- `ApplicationUtils`: `FullScreenToggle` / `QuitGameButton` / `ResolutionSelect` / `ResolutionApplier` — Samples 24
- `PopupWindow`: `PopupPanel` — Samples 25
- `AbstractList` / `AbstractListItem`(テンプレートInstantiateによる動的リスト) — Samples 26
- `UIBinder`系: `FloatBinder` / `IntBinder` / `StringBinder` / `SliderBinder` / `FloatAnimatedBinder` / `GaugeBinder` / `MemoryGaugeBinder` / `RateToTextBinder` / `RateToGradientBinder` / `ViewSwitchBinder` 等の値⇔UIバインダー群 — Samples 27
- `AudioMixerManager.UI`: `VolumeSliderBinder` / `VolumeInputFieldBinder`(AudioMixerパラメータのUIバインド) — Samples 43
- `UI`: `UIViewToggler` / `UISwitchAnimationPlayer` / `UISelectedOnEnable` / `UICursorTrackingObject`

### 演出・アニメーション
- `OnSelectBehaviour`系: `ColorOnSelect` / `MoveOnSelect` / `ScaleOnSelect` / `PlaySEOnSelect`(選択時演出) — Samples 30
- `TweenUtils`系: `TweenLocalMove` / `TweenScale` / `TweenJump` / `TweenFadeGraphic` / `TweenFadeSprite` / `TweenImageAmount` / `TweenShake`、`Ease` / `LoopType` — Samples 30
- `AnimatorUtils`: `AnimatorStateObserver`(Animatorの状態遷移をR3で購読) / `AnimatorBinder` / `AnimatorExtensions`(UniTaskベースの再生待機) — Samples 31
- `SpriteAnimation`系: `AbstractAnimation<T>`(Time.timeScale非依存の`AnimatorUpdateMode`対応) / `SpriteAnimation` / `ImageAnimation` — Samples 32
- `FadeScreen`: `FadeScreenManager` / `FadeSetting`(画面フェード)、`SplashController` — Samples 33 / 62
- `Movement`: `MaterialOffsetMover` / `SpriteScroller` / `ObjectMover` / `ObjectRotator` / `DelayTrack`、`ParticleSystemSimulator` / `GradientImage` 等の小物演出系 — Samples 90

### ゲームシステム
- `Parameter`系: `AbstractIntParameter` / `AbstractFloatParameter` / `AbstractLongParameter` / `FlagsParameterBase` / `ParameterManager`、および `Parameter.Basic` の `Health` / `Level` / `Exp` — Samples 40
- `BasicTimer`(`ETimerType`によるカウントダウン/カウントアップ、`IBasicTimerObservable` / `IBasicTimerHandler`) — Samples 41
- `TalkUtils`: `TalkManager` / `LineViewer` / CSVベースの会話データ管理 — Samples 42
- `AudioManager`(`BGMManager` / `SEManager` / `VoiceManager` と各 `Player`)、`AudioMixerManager`(`EAudioMixerParam`) — Samples 43
- `BGMController`: `BGMControllerSetting` / `EAudioPlayMode`(シーン遷移に応じたBGMの自動再生・停止・フェード・クロスフェード切り替え) — Samples 44

### データ管理・通信
- `DataStore`系: `AbstractDataStore` / `AbstractDataAsset` / `AbstractDataStoreSingleton`(JSONセーブデータ管理) — Samples 50 / 51
- `HTTPUtils`(`HTTPRequestUtils` / `HTTPConfig`)、`JsonUtils`(`AESEncryption` / `EncryptedJsonFileHandler` / `PersistentDataTextFileHandler`) — Samples 52
- `Csv`: `CsvUtils` / `AbstractCsvData`

### シーン管理
- `SceneLoader`系: `SceneLoaderButton` / `SceneLoaderInputTrigger` / `SceneUnloadButton` / `SceneUnloadInputTrigger` / `SceneLoaderUtils` — Samples 60
- `SceneReference`(SceneAssetをguidで安全に参照) — Samples 61
- `SceneChangeDetector`系: `AbstractSceneChangeDetector` / `CurrentSceneTransitionActivator` / `CurrentSceneTransitionAnimationPlayer` / `CurrentSceneTransitionUISelecter` — Samples 62

### アーキテクチャ(DI)
- `VContainerExtensions`: `IScopeInitializable` / `IScopeRoot` / `AbstractScopeRoot<T>`(型ごとのスコープ自動収集・登録・解決) / `SceneLifetimeScope`(VContainerを使ったスコープ構築) — Samples 70

### 汎用ユーティリティ
- `MonoBehaviourLifecycleEvents`(Awake/Start/OnEnable/OnDisable/OnDestroy/OnBecameVisible/OnBecameInvisibleをUnityEventとしてInspectorから配線)
- `DelayDestroy` / `TimeScaler` / `OnBecameInvisibleDestroy` / `ProjectVersionViewer` / `SerializableKeyPair` / `CustomBounds` / `PlaySEOnSliderChanged` / `RigidbodyImpact` / `DontDestroyOnLoad` / `TransformExtensions` / `VectorUtils` / `UniTaskUtils` など、単機能の小物コンポーネント群 — Samples 90
- `DebugMethodAttribute` / `ReadOnlyAttribute`(Inspector拡張属性)

### エディタ拡張(Editor)
- `DebugMethod`系: `[DebugMethod]` 属性を付けたメソッドをInspectorからボタン実行できるようにする機能一式
- `SceneReferenceDrawer` / `FlagsParameterDrawer` / `ReadOnlyDrawer`(カスタムプロパティドロワー)
- `SceneOpenWindow` / `FolderPainter` / `BuildSettingOptimizer` / `Separator`系(エディタ作業効率化ツール)

## 関連プロジェクト

- `MyUtilsProject` — 本ライブラリの全機能をカテゴリ別に確認できるサンプルプロジェクト。`Assets/Projects/Samples/`配下が採番・カテゴリ分けされており、各サンプルシーンに機能説明のテキストが付いている。

## ライセンス

個人用ライブラリ。外部公開用のライセンス表記は特に設けていない。
