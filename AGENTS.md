# AGENTS: 開発運用ガイド（Unidux）

本ファイルは AI / 人間共通の開発運用ルールの正（SSOT）です。  
Codex / GitHub Copilot / Claude Code など、すべての AI クライアントはこのファイルに従ってください。

---

## プロジェクト概要

**Unidux** は Unity3D 向けの Redux ライクな状態管理ライブラリです。

- **リポジトリ**: [KanjiKurita/Unidux](https://github.com/KanjiKurita/Unidux)（[mattak/Unidux](https://github.com/mattak/Unidux) のフォーク）
- **パッケージ名**: `me.mattak.unidux`
- **バージョン**: `0.4.2`（UniRx → R3 移行済み）
- **ライセンス**: MIT
- **Unity 最低バージョン**: 2018.3
- **返答言語**: 日本語。コード内コメントは日本語、識別子は英語。

---

## 最重要ルール

- **不要な変更は行わない**。タスクの目的達成に直接寄与しない変更（リファクタリング、コメント追加、スタイル統一等）は禁止。
- 変更フロー: **Discovery → 仕様確認 → Implement → Retrospective** の順でのみ実施する。
- 既存の不適切な現状を肯定せず、To-Be 規約を優先する。
- パブリックリポジトリのため、センシティブな情報（APIキー、内部URL等）を絶対にコミットしない。

---

## 技術スタック

| 技術 | バージョン | 用途 |
|------|-----------|------|
| Unity | 2018.3+ | ゲームエンジン |
| R3 | 1.3.0 | Rx ライブラリ（UniRx からの移行済み） |
| ObservableCollections.R3 | 3.3.4 | R3 対応コレクション |
| NUnit | Unity 同梱 | ユニットテスト |

> **重要**: 本ライブラリは UniRx から **R3** へ移行済みです。新規コードで UniRx の名前空間（`UniRx`）を使用してはいけません。必ず `R3` を使用してください。

---

## リポジトリ構造

```
Assets/
  Plugins/
    Unidux/
      Scripts/            # パッケージ本体（UPM 配布対象）
        Core/             # Store, State, Reducer 等のコアインターフェース
        Experimental/     # 実験的機能
        SceneTransition/  # シーン遷移対応
        Util/             # ユーティリティ
        package.json      # UPM パッケージ定義
        Unidux.asmdef     # Assembly Definition
      Examples/           # 使用例（Counter, Todo, List 等）
      Test/
        Editor/           # EditMode テスト（NUnit）
          Core/
          Performance/
          Rx/
          SceneTransition/
          Util/
```

---

## 実装規約

### 命名規則
- クラス・インターフェース: `PascalCase`
- メソッド・プロパティ: `PascalCase`
- ローカル変数・引数: `camelCase`
- プライベートフィールド: `_camelCase`（アンダースコアプレフィックス）
- 定数: `PascalCase` または `UPPER_SNAKE_CASE`
- namespace: `Unidux` または `Unidux.{SubModule}`

### コーディングスタイル
- インデント: 4スペース
- 波括弧: Allman スタイル（始め括弧は新行に置かない — 既存コードに合わせて K&R スタイル）
- using 宣言はファイル先頭にまとめる

### R3 使用規約
- `Subject<T>` は R3 の `Subject<T>` を使用（`UniRx` 禁止）
- Dispose 管理: `CompositeDisposable` または `DisposableBag` を使用
- `Observable.XXX` は `R3.Observable.XXX` を参照

### アーキテクチャ原則
- **State**: `StateBase` を継承した不変に近いデータクラス。`IStateChanged` で変更検知。
- **Action**: `IAction` を実装した軽量なデータ構造体（または class）
- **Reducer**: `IReducer<TState, TAction>` を実装。副作用なし。
- **Store**: `Store<TState>` が唯一の真実の情報源（Single Source of Truth）

### 禁止事項
- `UniRx` 名前空間の新規使用
- `GameObject.Find` / `FindObjectOfType` 等の新規追加（Examples 内は例外）
- `async void`（Unity イベントハンドラ除く）
- Store への直接変更（Reducer 経由のみ）
- テスト内での `Thread.Sleep` 等のブロッキング待機

---

## テスト規約

### テスト配置
- `Assets/Plugins/Unidux/Test/Editor/` 配下に配置（EditMode テスト）
- テストクラス名: `{対象クラス名}Test`
- テストメソッド名: `{テストする挙動}Test`

### テストフレームワーク
- NUnit を使用（`[Test]` 属性）
- `Assert.AreEqual` / `Assert.IsTrue` / `Assert.IsNull` 等を使用

### テスト作成後の必須手順
1. Unity エディタでコンパイルエラーがないことを確認
2. Unity Test Runner（EditMode）でテストを実行
3. 全テストがグリーンであることを確認してからコミット

---

## ブランチ戦略

| ブランチ | 用途 |
|---------|------|
| `master` | 安定版リリース（フォーク元の master に対応） |
| `feature/kkurita/unidux-to-r3-from-unirx` | UniRx → R3 移行作業（現在のメイン開発ブランチ） |
| `feature/kkurita/*` | 個別機能開発 |

### コミットメッセージ規則
```
<type>: <summary>

<body> (任意)
```

**type**:
- `feat`: 新機能
- `fix`: バグ修正
- `refactor`: リファクタリング（外部から見た動作変化なし）
- `test`: テスト追加・修正
- `docs`: ドキュメントのみの変更
- `chore`: ビルド・補助ツール等の変更

---

## UPM パッケージ公開規約

- `package.json` の `version` を変更した場合は `CHANGELOG.md` も更新する
- 破壊的変更（Breaking Change）は CHANGELOG に明記する
- 依存関係の追加は `package.json` の `dependencies` に記載する（バージョン固定）

---

## AI開発環境（MCP サーバー）

このリポジトリは **Serena** を MCP サーバーとして利用します。

| 項目 | 内容 |
|---|---|
| 役割 | シンボリックコード解析・ファイル操作 |
| 設定ファイル | `.vscode/mcp.json` |
| プロジェクト設定 | `.serena/project.yml` |
| 有効言語 | なし（`.sln` を含むソリューシンファイルが存在しないため C# LSP 起動不可） |

**.sln 不在について**: Unity エディタが生成するもののため、単体リポジトリには存在しない。
LSP なしでも `find_file`, `search_for_pattern`, `read_file`, `list_dir` でコード読み取り・検索は可能。

---

## 振り返りフェーズ（Retrospective）

### トリガー条件
- 問題解決・動作確認完了時
- 実装タスク完了時
- 複数回の試行錯誤を経て解決した時

### 振り返り手順
1. **教訓の抽出**: 詰まったポイント、根本原因、解決策を整理
2. **CHANGELOG.md / README.md への反映提案**: 変更内容を整理し、人間が承認後に反映
3. **SSOT 更新提案**: 具体的な差分形式（before/after）で提案。AI は直接変更しない。

---

## FAQ

### Q: UniRx と R3 の主な API 差分は？
**A**: 本ライブラリでの主な差分:
- `Subject<T>` → そのまま使用可能（namespaceが`R3`に変わる）
- `IObserver<T>` / `IObservable<T>` → R3 では `Observer<T>` / `Observable<T>` に対応
- `Disposable.Empty` → `Disposable.Create(() => {})` または `Disposable.Combine()`
- `MainThreadDispatcher` → R3 では不要（`ObserveOnMainThread()` は `ObserveOn(UnityTimeProvider.Update)` 等）

### Q: Examples は修正対象か？
**A**: Examples は動作確認用です。コアライブラリの変更に伴い Examples も更新が必要な場合は修正します。ただし Examples は UPM 配布対象外です。

### Q: テスト用 asmdef が必要か？
**A**: 現在テストは Editor フォルダに配置されており、`Assembly-CSharp-Editor` に自動含有されます。新規テスト用 asmdef が必要な場合は `overrideReferences: false` を使用してください。
