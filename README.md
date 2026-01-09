# TTS Voice Wizard

<div align="center">

![TTS Voice Wizard Logo](https://img.shields.io/badge/TTS%20Voice%20Wizard-VRChat%20%E9%9F%B3%E5%A3%B0%E3%82%BD%E3%83%AA%E3%83%A5%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3-purple?style=for-the-badge)

**VRChat・PCVR環境向け リアルタイム音声認識＆音声合成アプリケーション**

[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://www.microsoft.com/windows)
[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![VRChat](https://img.shields.io/badge/VRChat-OSC-1E90FF.svg)](https://hello.vrchat.com/)

[English](README_EN.md) | **日本語**

</div>

---

## 📋 目次

1. [プロジェクト概要](#-プロジェクト概要)
2. [主要機能](#-主要機能)
3. [システム要件](#-システム要件)
4. [インストール方法](#-インストール方法)
5. [クイックスタート](#-クイックスタート)
6. [詳細設定ガイド](#-詳細設定ガイド)
7. [音声認識エンジン](#-音声認識エンジン)
8. [音声合成エンジン](#-音声合成エンジン)
9. [VRChat OSC連携](#-vrchat-osc連携)
10. [外部サービス連携](#-外部サービス連携)
11. [PICO4/Quest向け設定](#-pico4quest向け設定)
12. [VOICEVOX連携](#-voicevox連携)
13. [低遅延最適化](#-低遅延最適化)
14. [トラブルシューティング](#-トラブルシューティング)
15. [開発者向け情報](#-開発者向け情報)
16. [ライセンス](#-ライセンス)

---

## 🎯 プロジェクト概要

### TTS Voice Wizardとは

TTS Voice Wizard（TTSボイスウィザード）は、VRChat環境において**リアルタイムの音声認識（STT: Speech-to-Text）** と **音声合成（TTS: Text-to-Speech）** を実現するWindowsデスクトップアプリケーションです。

本アプリケーションは、以下のような用途に最適化されています：

- **VRChatでの音声コミュニケーション支援**
- **多言語間リアルタイム翻訳**
- **音声障害を持つユーザーのコミュニケーション支援**
- **アバターへの字幕・テキスト表示**
- **カスタム音声でのロールプレイ**

### コアコンセプト

```
┌─────────────────────────────────────────────────────────────────┐
│                    TTS Voice Wizard アーキテクチャ               │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│   [マイク入力] ──► [音声認識(STT)] ──► [テキスト処理] ──►        │
│                                              │                  │
│                                              ▼                  │
│                                        [翻訳 (オプション)]       │
│                                              │                  │
│                                              ▼                  │
│   [VRChat OSC] ◄── [音声合成(TTS)] ◄── [テキスト出力]           │
│        │                  │                                     │
│        ▼                  ▼                                     │
│   [Chatbox表示]    [仮想マイク出力]                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 開発の背景と目的

VRChatやその他のソーシャルVRプラットフォームにおいて、音声コミュニケーションは最も重要な要素の一つです。しかし、以下のような課題が存在します：

1. **言語の壁**: 異なる言語を話すユーザー間でのコミュニケーション障壁
2. **音声障害**: 声を出すことが困難なユーザーのコミュニケーション手段の制限
3. **プライバシー**: 自分の声を隠したいユーザーのニーズ
4. **表現の幅**: 異なる声質やキャラクターボイスでの表現欲求

TTS Voice Wizardは、これらの課題を解決し、すべてのユーザーが快適にVRソーシャル体験を楽しめることを目指して開発されました。

---

## 🚀 主要機能

### 音声認識（Speech-to-Text）

TTS Voice Wizardは、複数の音声認識エンジンに対応しており、用途や環境に応じて最適なエンジンを選択できます。

| エンジン名 | 特徴 | オフライン対応 | 日本語精度 |
|-----------|------|--------------|-----------|
| **OpenAI Whisper** | 高精度・多言語対応 | ✅ | ⭐⭐⭐⭐⭐ |
| **Vosk** | 軽量・オフライン専用 | ✅ | ⭐⭐⭐⭐ |
| **Azure Speech** | クラウド・高精度 | ❌ | ⭐⭐⭐⭐⭐ |
| **ElevenLabs** | 高品質音声処理 | ❌ | ⭐⭐⭐⭐ |
| **Web Speech API** | ブラウザベース | ❌ | ⭐⭐⭐ |
| **DeepL Speech** | 翻訳統合 | ❌ | ⭐⭐⭐⭐ |

### 音声合成（Text-to-Speech）

多様なTTSエンジンをサポートし、ユーザーの好みに合わせた音声出力が可能です。

| エンジン名 | 特徴 | 音声品質 | 日本語対応 |
|-----------|------|---------|-----------|
| **VOICEVOX** | 無料・高品質日本語 | ⭐⭐⭐⭐⭐ | ✅ |
| **Azure TTS** | 多言語・自然な音声 | ⭐⭐⭐⭐⭐ | ✅ |
| **ElevenLabs** | AI音声クローン | ⭐⭐⭐⭐⭐ | ✅ |
| **Amazon Polly** | AWS統合 | ⭐⭐⭐⭐ | ✅ |
| **TikTok TTS** | ユニークな音声 | ⭐⭐⭐ | ❌ |
| **System Speech** | Windows標準 | ⭐⭐⭐ | ✅ |
| **UberDuck** | キャラクターボイス | ⭐⭐⭐⭐ | ❌ |

### VRChat連携機能

- **OSC（Open Sound Control）プロトコル対応**
- **Chatbox テキスト送信**
- **アバターパラメータ制御**
- **リップシンク連動**
- **表情パラメータ連動**

### 追加機能

- **リアルタイム翻訳**: DeepL、Google翻訳、Azure Translator対応
- **心拍数連携**: Pulsoid、HypeRate対応でアバターに心拍情報を反映
- **メディア連携**: Spotify、Windows Mediaの再生情報をChatboxに表示
- **ボイスプリセット**: 複数の音声設定を保存・切り替え
- **ホットキー対応**: キーボードショートカットで各種機能を制御
- **多言語UI**: 日本語を含む複数言語のインターフェース

---

## 💻 システム要件

### 最小要件

| 項目 | 要件 |
|-----|------|
| **OS** | Windows 10 64bit (Version 1903以降) |
| **CPU** | Intel Core i5 / AMD Ryzen 5 以上 |
| **メモリ** | 8GB RAM |
| **ストレージ** | 2GB以上の空き容量 |
| **ネットワーク** | インターネット接続（クラウドサービス利用時） |
| **.NET** | .NET 6.0 Runtime |

### 推奨要件

| 項目 | 要件 |
|-----|------|
| **OS** | Windows 11 64bit |
| **CPU** | Intel Core i7 / AMD Ryzen 7 以上 |
| **メモリ** | 16GB RAM以上 |
| **GPU** | NVIDIA GPU (Whisper CUDA対応時) |
| **ストレージ** | SSD 5GB以上の空き容量 |

### PICO4/Quest PCVR利用時の追加要件

| 項目 | 要件 |
|-----|------|
| **VRヘッドセット** | PICO4、Meta Quest 2/3/Pro |
| **接続方式** | USB 3.0ケーブル または Wi-Fi 6 |
| **ストリーミングソフト** | PICO Streaming Assistant / Virtual Desktop / Steam Link |
| **SteamVR** | 最新版 |

---

## 📥 インストール方法

### 方法1: リリースビルドのダウンロード（推奨）

1. [Releases](https://github.com/VRCWizard/TTS-Voice-Wizard/releases)ページから最新版をダウンロード
2. ダウンロードしたZIPファイルを任意のフォルダに展開
3. `TTSVoiceWizard.exe`を実行

### 方法2: ソースからのビルド

#### 前提条件

- Visual Studio 2022 または .NET 6.0 SDK
- Git

#### ビルド手順

```bash
# リポジトリのクローン
git clone https://github.com/VRCWizard/TTS-Voice-Wizard.git
cd TTS-Voice-Wizard

# NuGetパッケージの復元
dotnet restore

# ビルド
dotnet build --configuration Release

# 実行
dotnet run --project OSCVRCWiz
```

### 依存関係のインストール

#### 必須

- [.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)

#### オプション（使用するエンジンに応じて）

- **VOICEVOX**: [VOICEVOX](https://voicevox.hiroshiba.jp/)をインストール・起動
- **Whisper**: Python環境とWhisperモデル
- **Vosk**: Voskモデルファイル
- **仮想オーディオ**: VB-Audio Virtual Cable または類似ソフト

---

## ⚡ クイックスタート

### 基本的な使用手順

#### Step 1: アプリケーションの起動

1. `TTSVoiceWizard.exe`をダブルクリックして起動
2. 初回起動時に設定ウィザードが表示されます

#### Step 2: マイクの選択

1. **Settings** タブを開く
2. **Audio Input** セクションで使用するマイクを選択
3. PICO4の場合は `PICO Virtual Audio Device` または `Streaming Microphone` を選択

#### Step 3: 音声認識エンジンの設定

1. **STT** タブを開く
2. 使用するエンジンを選択（初心者にはWhisperを推奨）
3. 必要に応じてAPIキーを入力

#### Step 4: 音声合成エンジンの設定

1. **TTS** タブを開く
2. 使用するエンジンを選択（日本語にはVOICEVOXを推奨）
3. 音声出力デバイスを設定

#### Step 5: VRChatへの接続

1. VRChatを起動し、OSCを有効化（Action Menu → Options → OSC → Enabled）
2. TTS Voice Wizardの **OSC** タブで接続状態を確認
3. Chatboxへのテキスト送信をテスト

### 最小構成での動作確認

以下の設定で最もシンプルな構成を実現できます：

```
STT: Whisper (ローカル)
TTS: VOICEVOX
出力: VB-Audio Virtual Cable → VRChatマイク入力
```

---

## 🔧 詳細設定ガイド

### オーディオ設定

#### 入力デバイス設定

TTS Voice Wizardは、システムに接続されているすべてのオーディオ入力デバイスを自動検出します。

**設定項目:**

| 項目 | 説明 |
|-----|------|
| **Input Device** | 音声入力に使用するマイクデバイス |
| **Sample Rate** | サンプリングレート（通常16000Hz） |
| **Buffer Size** | オーディオバッファサイズ（低遅延化に影響） |
| **Voice Activity Detection** | 音声活動検出の有効/無効 |

#### 出力デバイス設定

TTSで生成された音声の出力先を設定します。

**設定項目:**

| 項目 | 説明 |
|-----|------|
| **Output Device** | TTS音声の出力デバイス |
| **Virtual Cable** | 仮想オーディオデバイスの使用 |
| **Volume** | 出力音量（0-100%） |
| **Output to Speakers** | スピーカーへの同時出力 |

### ネットワーク設定

#### OSC設定

VRChatとの通信に使用するOSC設定です。

```
デフォルト設定:
- VRChat IPアドレス: 127.0.0.1
- 送信ポート: 9000
- 受信ポート: 9001
```

**注意**: ファイアウォールでこれらのポートが許可されていることを確認してください。

#### API接続設定

各種クラウドサービスのAPIエンドポイント設定です。

| サービス | エンドポイント | 認証方式 |
|---------|--------------|---------|
| Azure Speech | カスタムリージョン指定可能 | APIキー |
| ElevenLabs | api.elevenlabs.io | APIキー |
| DeepL | api.deepl.com / api-free.deepl.com | APIキー |
| Amazon Polly | AWS リージョン指定 | IAM認証 |

### プロファイル管理

複数の設定プロファイルを保存し、シーンに応じて切り替えることができます。

**プロファイルに保存される設定:**

- 音声認識エンジン設定
- 音声合成エンジン設定
- 音声設定（ボイス、ピッチ、スピード）
- OSC設定
- 翻訳設定
- UI設定

```
使用例:
プロファイル1: 日常会話用（高速・低品質TTS）
プロファイル2: ロールプレイ用（キャラクターボイス）
プロファイル3: 国際交流用（翻訳有効）
```

---

## 🎤 音声認識エンジン

### OpenAI Whisper

Whisperは、OpenAIが開発した最先端の音声認識モデルです。多言語対応と高い認識精度が特徴です。

#### 特徴

- **多言語対応**: 99以上の言語を自動検出・認識
- **高精度**: 特に日本語の認識精度が非常に高い
- **オフライン動作**: ローカル実行可能
- **GPUサポート**: CUDA対応で高速処理

#### モデルサイズと精度

| モデル | サイズ | 精度 | 処理速度 | 推奨用途 |
|-------|-------|------|---------|---------|
| tiny | 75MB | ⭐⭐ | 非常に高速 | リアルタイム・低スペックPC |
| base | 142MB | ⭐⭐⭐ | 高速 | バランス重視 |
| small | 466MB | ⭐⭐⭐⭐ | 中速 | 高精度・日常使用 |
| medium | 1.5GB | ⭐⭐⭐⭐⭐ | 低速 | 最高精度・収録用 |
| large | 2.9GB | ⭐⭐⭐⭐⭐ | 非常に低速 | 最高精度 |

#### 設定方法

1. Whisperの実行環境をセットアップ
2. モデルファイルをダウンロード
3. TTS Voice WizardでWhisperパスを指定
4. 使用するモデルサイズを選択

```python
# Whisperインストール（参考）
pip install openai-whisper
```

### Vosk

Voskは、軽量でオフライン動作に特化した音声認識エンジンです。

#### 特徴

- **完全オフライン**: インターネット接続不要
- **軽量**: リソース消費が少ない
- **低遅延**: リアルタイム処理に適している
- **多言語モデル**: 各言語専用のモデルを提供

#### 日本語モデル

```
推奨モデル:
- vosk-model-ja-0.22 (1GB) - 高精度
- vosk-model-small-ja-0.22 (50MB) - 軽量版
```

#### 設定方法

1. [Vosk Models](https://alphacephei.com/vosk/models)から日本語モデルをダウンロード
2. モデルを任意のフォルダに展開
3. TTS Voice Wizardでモデルパスを指定

### Azure Speech Services

Microsoftが提供するクラウドベースの音声認識サービスです。

#### 特徴

- **高精度**: 企業レベルの認識精度
- **リアルタイム**: ストリーミング認識対応
- **カスタム語彙**: ドメイン固有の単語を登録可能
- **話者識別**: 複数話者の識別が可能

#### 料金

| 項目 | 無料枠 | 従量課金 |
|-----|-------|---------|
| 音声認識 | 5時間/月 | $1/時間 |
| 神経音声TTS | 50万文字/月 | $16/100万文字 |

#### 設定方法

1. [Azure Portal](https://portal.azure.com/)でSpeech Servicesリソースを作成
2. APIキーとリージョンを取得
3. TTS Voice Wizardに設定を入力

```
設定例:
API Key: your-azure-api-key
Region: japaneast
Language: ja-JP
```

### ElevenLabs Speech Recognition

ElevenLabsは、高品質な音声処理を提供するサービスです。

#### 特徴

- **高品質処理**: ノイズ除去と明瞭化
- **感情検出**: 音声から感情を検出
- **TTS統合**: ElevenLabs TTSとの連携が容易

---

## 🔊 音声合成エンジン

### VOICEVOX

VOICEVOXは、無料で利用できる高品質な日本語音声合成エンジンです。**日本語でVRChatを楽しむユーザーに最も推奨されるエンジンです。**

#### 特徴

- **完全無料**: 商用利用も可能（利用規約に従う）
- **高品質**: 自然な日本語音声
- **多彩なキャラクター**: 複数のボイスキャラクター
- **パラメータ調整**: 速度・ピッチ・抑揚の細かい調整
- **ローカル動作**: インターネット接続不要

#### キャラクター一覧（一部）

| キャラクター | 特徴 | 用途 |
|------------|------|-----|
| 四国めたん | 元気な女性 | 日常会話 |
| ずんだもん | かわいい声 | キャラクターRP |
| 春日部つむぎ | 落ち着いた女性 | ナレーション |
| 雨晴はう | 少年声 | 男性キャラ |
| 波音リツ | クール系 | 大人女性 |
| 玄野武宏 | 男性声 | 男性ボイス |

#### 設定方法

1. [VOICEVOX公式サイト](https://voicevox.hiroshiba.jp/)からダウンロード
2. VOICEVOXを起動（バックグラウンドでエンジンが起動）
3. TTS Voice WizardでVOICEVOXエンジンを選択
4. 使用するキャラクターを選択

```
VOICEVOXデフォルト設定:
- ホスト: http://localhost:50021
- ポート: 50021
```

#### 高度な設定

```json
{
  "speedScale": 1.0,      // 話速（0.5-2.0）
  "pitchScale": 0.0,      // 音高（-0.15-0.15）
  "intonationScale": 1.0, // 抑揚（0.0-2.0）
  "volumeScale": 1.0,     // 音量（0.0-2.0）
  "prePhonemeLength": 0.1,
  "postPhonemeLength": 0.1
}
```

### Azure Text-to-Speech

Microsoftの神経音声技術を使用した高品質TTS。

#### 特徴

- **Neural Voice**: AIベースの自然な音声
- **多言語対応**: 100以上の言語・方言
- **カスタム音声**: 独自の音声モデル作成可能
- **SSML対応**: 詳細な音声制御

#### 日本語音声一覧

| 音声名 | 性別 | スタイル |
|-------|------|---------|
| ja-JP-NanamiNeural | 女性 | ニュース、カスタマーサービス |
| ja-JP-KeitaNeural | 男性 | ナレーション |
| ja-JP-AoiNeural | 女性 | チャット |
| ja-JP-DaichiNeural | 男性 | カジュアル |
| ja-JP-MayuNeural | 女性 | カスタマーサービス |
| ja-JP-NaokiNeural | 男性 | アシスタント |
| ja-JP-ShioriNeural | 女性 | チャット |

### ElevenLabs

最先端のAI音声合成技術を提供するサービス。

#### 特徴

- **Voice Cloning**: 音声クローン作成
- **多言語生成**: 一つの音声で多言語生成
- **感情表現**: 豊かな感情表現
- **リアルタイム対応**: 低遅延ストリーミング

#### 料金プラン

| プラン | 文字数/月 | 価格 |
|-------|---------|------|
| Free | 10,000 | 無料 |
| Starter | 30,000 | $5 |
| Creator | 100,000 | $22 |
| Pro | 500,000 | $99 |

### Amazon Polly

AWSの音声合成サービス。

#### 日本語音声

- **Takumi** (男性) - Neural
- **Kazuha** (女性) - Neural
- **Tomoko** (女性) - Neural

### System Speech

Windows標準の音声合成機能を使用。

#### 特徴

- **追加インストール不要**: Windowsに標準搭載
- **低リソース**: 軽量動作
- **オフライン**: インターネット不要

#### 日本語音声の追加

Windows設定 → 時刻と言語 → 言語 → 日本語 → 音声パック追加

---

## 🎮 VRChat OSC連携

### OSCの概要

OSC（Open Sound Control）は、ネットワーク経由でアプリケーション間でデータを送受信するプロトコルです。VRChatは、OSCを通じて外部アプリケーションからアバターやチャットボックスを制御できます。

### 基本設定

#### VRChatでのOSC有効化

1. VRChatを起動
2. アクションメニューを開く（Rキー または コントローラーメニューボタン）
3. Options → OSC → Enabled をオン

#### TTS Voice WizardでのOSC設定

```
送信先: 127.0.0.1:9000 (VRChat受信ポート)
受信元: 127.0.0.1:9001 (VRChat送信ポート)
```

### Chatbox機能

Chatboxは、VRChat内で頭上にテキストを表示する機能です。

#### 送信パラメータ

| パラメータ | 型 | 説明 |
|-----------|---|------|
| message | String | 表示するテキスト（144文字以内） |
| send | Boolean | 送信フラグ |
| notify | Boolean | 通知サウンド |

#### OSCアドレス

```
/chatbox/input [string message, bool send, bool notify]
/chatbox/typing [bool isTyping]
```

#### 文字数制限と対策

VRChat Chatboxには144バイトの文字数制限があります。日本語は1文字3バイトのため、実質約48文字が上限となります。

**対策:**
- 長文は自動分割して複数回に分けて送信
- 不要な文字を省略
- 要約機能の活用

### アバターパラメータ制御

OSCを通じてアバターのパラメータを直接制御できます。

#### 使用例

```
音声認識状態の表示:
/avatar/parameters/SpeechRecognizing [bool]

TTS再生状態:
/avatar/parameters/TTSPlaying [bool]

音量レベル:
/avatar/parameters/VoiceLevel [float 0.0-1.0]
```

### Viseme（リップシンク）連動

TTSの音声出力に連動してアバターの口を動かすことができます。

#### 対応Viseme

| Visemeパラメータ | 発音 |
|-----------------|------|
| sil | 無音 |
| PP | p, b, m |
| FF | f, v |
| TH | th |
| DD | t, d |
| kk | k, g |
| CH | ch, j |
| SS | s, z |
| nn | n |
| RR | r |
| aa | a |
| E | e |
| ih | i |
| oh | o |
| ou | u |

---

## 🌐 外部サービス連携

### 翻訳サービス連携

#### DeepL

高品質な機械翻訳サービス。特に日本語⇔英語の翻訳品質が高い。

**設定方法:**
1. [DeepL API](https://www.deepl.com/pro-api)でアカウント作成
2. APIキーを取得
3. TTS Voice Wizardに設定

```
無料プラン: 50万文字/月
Pro プラン: 従量課金
```

#### Google翻訳

幅広い言語対応の翻訳サービス。

#### Azure Translator

Microsoft Azure の翻訳サービス。Azure Speechと統合利用可能。

### 心拍数連携

#### Pulsoid

スマートウォッチやフィットネストラッカーから心拍数を取得し、アバターに反映。

**対応デバイス:**
- Apple Watch
- Garmin
- Fitbit
- Samsung Galaxy Watch
- その他Bluetooth心拍計

**使用例:**
- 心拍数をアバターのパラメータに反映
- 心拍数をChatboxに表示
- 心拍数に応じた表情変化

#### HypeRate

ゲーマー向け心拍数共有サービス。

### メディア連携

#### Spotify

現在再生中の曲情報をChatboxに表示。

**表示例:**
```
🎵 Now Playing: Song Title - Artist Name
```

#### Windows Media

システムで再生中のメディア情報を取得・表示。

**対応アプリ:**
- Spotify
- YouTube Music
- Amazon Music
- その他システムメディア連携対応アプリ

---

## 📱 PICO4/Quest向け設定

### PICO4でのセットアップ

PICO4をPCVRモードで使用する場合の設定ガイドです。

#### 接続方式の選択

| 方式 | 遅延 | 品質 | 安定性 |
|-----|------|------|-------|
| USB接続 | 低 | 高 | 高 |
| Wi-Fi (5GHz) | 中 | 中-高 | 中 |
| Wi-Fi (6GHz/Wi-Fi 6E) | 低 | 高 | 高 |

#### PICO Streaming Assistant設定

1. PCにPICO Streaming Assistantをインストール
2. PICO4とPCを同じネットワークに接続
3. ストリーミング設定でマイクを有効化

```
設定項目:
- マイク: 有効
- マイクサンプルレート: 48000Hz
- オーディオ品質: 高
```

#### 音声入力デバイスの選択

TTS Voice Wizardで以下のデバイスを選択：

```
PICO4 USB接続時:
- "PICO Virtual Audio Device"

PICO4 ワイヤレス接続時:
- "Streaming Audio" または類似名称
```

### Quest 2/3/Pro向け設定

#### Virtual Desktop使用時

1. Virtual DesktopのStreamer設定でマイク転送を有効化
2. TTS Voice Wizardで "Virtual Desktop Audio" を選択

#### Air Link使用時

1. Oculus ソフトウェアでマイク設定を確認
2. システムのデフォルトマイクとして設定
3. TTS Voice Wizardでマイクを選択

#### Steam Link使用時

1. SteamVR設定でマイクを有効化
2. オーディオ設定でマイク入力を確認
3. TTS Voice Wizardで対応デバイスを選択

### 遅延最適化

PCVRでの音声処理遅延を最小化するための設定：

1. **有線接続の使用**: USB 3.0以上のケーブル推奨
2. **Wi-Fi環境の最適化**: 5GHz/6GHz帯の使用、ルーターとの距離短縮
3. **バッファサイズの調整**: 小さいバッファで低遅延化（安定性とトレードオフ）
4. **ストリーミング品質の調整**: ビットレートを下げて遅延を削減

---

## 🗣️ VOICEVOX連携

### VOICEVOXとは

VOICEVOXは、ヒホ氏（Hiroshiba Kazuyuki）が開発した**無料で使える高品質な日本語テキスト読み上げソフトウェア**です。

### 特徴と利点

- **完全無料**: 個人・商用利用ともに無料
- **高品質な日本語音声**: 自然なイントネーション
- **オフライン動作**: インターネット接続不要
- **CPU/GPU対応**: NVIDIA GPU使用で高速化
- **パラメータ調整**: 細かい音声調整が可能

### インストール手順

#### Step 1: ダウンロード

1. [VOICEVOX公式サイト](https://voicevox.hiroshiba.jp/)にアクセス
2. OSに対応したインストーラーをダウンロード
3. インストーラーを実行

#### Step 2: 初回起動

1. VOICEVOXを起動
2. 利用規約に同意
3. 初期設定を完了

#### Step 3: エンジンの確認

VOICEVOXエンジンがバックグラウンドで起動していることを確認：

```bash
# エンジンの動作確認
curl http://localhost:50021/version
```

応答例: `"0.14.7"`

### TTS Voice Wizardとの連携

#### 設定方法

1. VOICEVOXを起動（エンジンがポート50021で待機）
2. TTS Voice Wizardを起動
3. TTS設定でVOICEVOXを選択
4. キャラクターを選択

#### パラメータ設定

| パラメータ | 範囲 | 説明 |
|-----------|------|------|
| 話速 | 0.5-2.0 | 読み上げ速度 |
| 音高 | -0.15-0.15 | 声の高さ |
| 抑揚 | 0.0-2.0 | イントネーションの強さ |
| 音量 | 0.0-2.0 | 出力音量 |

#### キャラクター別推奨設定

**日常会話向け（四国めたん）:**
```json
{
  "speedScale": 1.1,
  "pitchScale": 0.0,
  "intonationScale": 1.0
}
```

**落ち着いたナレーション（春日部つむぎ）:**
```json
{
  "speedScale": 0.9,
  "pitchScale": -0.05,
  "intonationScale": 0.8
}
```

**元気なキャラクター（ずんだもん）:**
```json
{
  "speedScale": 1.2,
  "pitchScale": 0.1,
  "intonationScale": 1.3
}
```

### VOICEVOX APIリファレンス

TTS Voice WizardはVOICEVOX REST APIを使用しています。

#### 音声合成フロー

```
1. 音声クエリ生成
   POST /audio_query?text={text}&speaker={speaker_id}

2. 音声合成
   POST /synthesis?speaker={speaker_id}
   Body: 音声クエリJSON

3. 音声データ取得
   Response: audio/wav
```

#### スピーカーID一覧

```json
{
  "四国めたん（ノーマル）": 2,
  "四国めたん（あまあま）": 0,
  "四国めたん（セクシー）": 4,
  "四国めたん（ツンツン）": 6,
  "ずんだもん（ノーマル）": 3,
  "ずんだもん（あまあま）": 1,
  "ずんだもん（セクシー）": 5,
  "ずんだもん（ツンツン）": 7,
  "春日部つむぎ": 8,
  "雨晴はう": 10,
  "波音リツ": 9,
  "玄野武宏": 11
}
```

---

## ⚡ 低遅延最適化

### 遅延の要因と対策

VRChatでリアルタイム音声通話を実現するには、「発話 → 認識 → 合成 → 出力」の全体遅延を最小化する必要があります。

#### 遅延の内訳

| 処理段階 | 一般的な遅延 | 最適化後 |
|---------|------------|---------|
| マイク入力 | 20-50ms | 10-20ms |
| 音声認識（STT） | 500-2000ms | 200-500ms |
| テキスト処理 | 10-50ms | 5-20ms |
| 音声合成（TTS） | 200-1000ms | 100-300ms |
| オーディオ出力 | 20-50ms | 10-20ms |
| **合計** | **750-3150ms** | **325-860ms** |

### 音声認識の最適化

#### Whisper最適化

1. **適切なモデルサイズ選択**
   - リアルタイム用途: `tiny` または `base`
   - GPU使用時: `small` まで許容

2. **GPU活用**
   ```bash
   # CUDA有効化確認
   python -c "import torch; print(torch.cuda.is_available())"
   ```

3. **ストリーミング認識**
   - 音声を小さなチャンクで処理
   - 部分結果を逐次表示

#### Vosk最適化

1. **軽量モデル使用**: `vosk-model-small-ja-0.22`
2. **サンプルレート調整**: 16000Hzが最適
3. **バッファサイズ**: 4000サンプル以下

### 音声合成の最適化

#### VOICEVOX最適化

1. **GPUモード使用**
   ```
   VOICEVOX設定 → GPUモードを有効化
   ```

2. **事前キャッシュ**
   - よく使うフレーズを事前合成
   - キャッシュヒット率向上

3. **並列処理**
   - 認識中に前の文の合成を実行
   - パイプライン処理

#### 話速設定

```
speedScale: 1.2-1.5
```
話速を上げることで発話時間を短縮し、会話のテンポを維持。

### オーディオ設定の最適化

#### バッファサイズ

小さいバッファは低遅延だが、音切れリスク増加。

```
推奨設定:
- 安定性重視: 4096サンプル
- 低遅延重視: 1024-2048サンプル
```

#### 仮想オーディオデバイス

VB-Audio Virtual Cable設定:

1. 内部サンプルレート: 48000Hz
2. バッファサイズ: 最小（32-64サンプル）

### システム全体の最適化

1. **不要なアプリを終了**: CPU/メモリリソース確保
2. **電源設定**: 高パフォーマンスモード
3. **ウイルス対策ソフト**: 除外リストに追加
4. **Windowsオーディオ設定**: 排他モード有効化

### ベンチマーク例

最適化前後の比較（Whisper tiny + VOICEVOX、RTX 3070環境）:

| 項目 | 最適化前 | 最適化後 |
|-----|---------|---------|
| 認識遅延 | 800ms | 250ms |
| 合成遅延 | 400ms | 150ms |
| 総遅延 | 1200ms | 400ms |

---

## 🔧 トラブルシューティング

### よくある問題と解決策

#### 音声認識が機能しない

**症状:** マイク入力があるのにテキストが表示されない

**確認事項:**
1. マイクデバイスが正しく選択されているか
2. マイクがミュートになっていないか
3. マイクのアクセス許可が有効か（Windowsプライバシー設定）
4. 音声認識エンジンが正常に起動しているか

**解決策:**
```
1. Windowsの設定 → プライバシー → マイク → アプリのアクセス許可を確認
2. デバイスマネージャーでマイクドライバーを更新
3. TTS Voice Wizardを管理者権限で実行
```

#### VOICEVOXに接続できない

**症状:** "VOICEVOX Engine not found" エラー

**確認事項:**
1. VOICEVOXが起動しているか
2. ポート50021が使用可能か
3. ファイアウォールでブロックされていないか

**解決策:**
```bash
# ポート確認
netstat -an | findstr 50021

# VOICEVOXを再起動
# ファイアウォール例外を追加
```

#### VRChatに接続できない

**症状:** Chatboxにテキストが表示されない

**確認事項:**
1. VRChatでOSCが有効か
2. ポート9000/9001が開いているか
3. VRChatとTTS Voice Wizardが同じPCで動作しているか

**解決策:**
```
1. VRChat Action Menu → Options → OSC → Enable
2. ファイアウォールでポート9000/9001を許可
3. OSCファイルのリセット:
   %AppData%\..\LocalLow\VRChat\VRChat\OSC
   フォルダを削除してVRChatを再起動
```

#### 音声出力がない

**症状:** TTSは動作しているが音が聞こえない

**確認事項:**
1. 出力デバイスが正しく選択されているか
2. 音量がゼロになっていないか
3. 仮想オーディオデバイスが正常に動作しているか

**解決策:**
```
1. Windowsサウンド設定で出力デバイスを確認
2. VB-Audio Virtual Cableを再インストール
3. Windows Audio サービスを再起動
```

### デバッグモード

詳細なログを有効にしてデバッグ情報を取得：

```
TTS Voice Wizard → Settings → Debug → Enable Debug Logging
ログファイル: %AppData%\TTSVoiceWizard\logs\
```

### ログファイルの分析

```
[ERROR] マイクデバイスが見つかりません
→ オーディオドライバーを確認

[ERROR] VOICEVOX API timeout
→ VOICEVOXを再起動、またはタイムアウト値を増加

[ERROR] OSC send failed
→ ネットワーク設定を確認
```

### パフォーマンス問題

**高CPU使用率:**
- Whisperモデルサイズを小さくする
- GPUモードを有効化
- 不要なエンジンを無効化

**高メモリ使用量:**
- 言語モデルをアンロード
- キャッシュサイズを制限
- アプリケーションを再起動

---

## 👨‍💻 開発者向け情報

### プロジェクト構造

```
TTS-Voice-Wizard/
├── OSCVRCWiz/                    # メインアプリケーション
│   ├── Services/                 # サービスレイヤー
│   │   ├── Speech/               # 音声処理
│   │   │   ├── TextToSpeech/     # TTS エンジン
│   │   │   │   ├── TTSEngines/   # 各種TTSエンジン実装
│   │   │   │   └── ...
│   │   │   └── Speech Recognition/ # STT エンジン
│   │   │       └── ...
│   │   ├── Integrations/         # 外部サービス連携
│   │   │   ├── Heartrate/        # 心拍数連携
│   │   │   ├── Media/            # メディア連携
│   │   │   └── ...
│   │   ├── Text/                 # テキスト処理
│   │   └── ...
│   ├── Settings/                 # 設定管理
│   ├── Resources/                # リソースファイル
│   ├── RJControls/               # カスタムUIコントロール
│   └── VoiceWizardWindow.cs      # メインウィンドウ
├── MoonbaseVoices/               # 追加音声サービス
└── README.md                     # このファイル
```

### 技術スタック

| カテゴリ | 技術 |
|---------|------|
| 言語 | C# 10.0 |
| フレームワーク | .NET 6.0 |
| UI | Windows Forms |
| オーディオ | NAudio |
| ネットワーク | Rug.Osc (OSC) |
| データ | Newtonsoft.Json |

### 主要クラス

#### VoiceWizardWindow

メインウィンドウクラス。UIとサービスの統合を担当。

```csharp
public partial class VoiceWizardWindow : Form
{
    // UI初期化
    // イベントハンドリング
    // サービス連携
}
```

#### AudioService

オーディオ入出力の管理。

```csharp
public class AudioService
{
    // マイク入力
    // 音声出力
    // デバイス管理
}
```

#### TTSEngine（基底クラス）

音声合成エンジンの共通インターフェース。

```csharp
public abstract class TTSEngine
{
    public abstract Task<byte[]> SynthesizeAsync(string text);
    public abstract void SetVoice(string voiceId);
    public abstract void SetParameters(TTSParameters parameters);
}
```

### 新しいTTSエンジンの追加

1. `TTSEngines`フォルダに新しいクラスを作成
2. `TTSEngine`基底クラスを継承
3. 必要なメソッドを実装
4. UIに設定項目を追加
5. エンジン選択ロジックに追加

```csharp
public class CustomTTS : TTSEngine
{
    public override async Task<byte[]> SynthesizeAsync(string text)
    {
        // 音声合成実装
    }
    
    public override void SetVoice(string voiceId) { }
    public override void SetParameters(TTSParameters parameters) { }
}
```

### ビルドとテスト

#### ビルド

```bash
# Debug ビルド
dotnet build

# Release ビルド
dotnet build --configuration Release

# 公開ビルド
dotnet publish --configuration Release --self-contained true
```

#### テスト

```bash
dotnet test
```

### 貢献ガイドライン

1. Issueで議論
2. Forkしてブランチ作成
3. 変更を実装
4. テストを追加
5. Pull Requestを作成

```
ブランチ命名:
feature/xxx - 新機能
fix/xxx - バグ修正
docs/xxx - ドキュメント
```

---

## 📄 ライセンス

### TTS Voice Wizard

本ソフトウェアはMITライセンスの下で公開されています。

```
MIT License

Copyright (c) 2024 VRCWizard

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### サードパーティライセンス

本ソフトウェアは以下のオープンソースライブラリを使用しています：

| ライブラリ | ライセンス | 用途 |
|-----------|-----------|------|
| NAudio | MIT | オーディオ処理 |
| Newtonsoft.Json | MIT | JSON処理 |
| Rug.Osc | BSD | OSC通信 |
| Vosk | Apache 2.0 | 音声認識 |

### VOICEVOXキャラクター利用規約

VOICEVOX のキャラクターを使用する場合は、各キャラクターの利用規約に従ってください。

- [VOICEVOX利用規約](https://voicevox.hiroshiba.jp/term)

### 外部サービス利用規約

各外部サービス（Azure、AWS、ElevenLabs等）を使用する場合は、それぞれのサービスの利用規約に従ってください。

---

## 🙏 謝辞

### 開発者・コントリビューター

- VRCWizard（オリジナル開発者）
- すべてのコントリビューター

### 技術提供

- VOICEVOX / ヒホ様
- OpenAI Whisper
- Azure Cognitive Services
- ElevenLabs
- Vosk

### コミュニティ

- VRChat コミュニティ
- 日本VRChatユーザーコミュニティ
- すべてのユーザーとフィードバック提供者

---

## 📞 サポート・連絡先

### 問題報告

バグや機能リクエストは[GitHub Issues](https://github.com/VRCWizard/TTS-Voice-Wizard/issues)にて報告してください。

### コミュニティ

- [Discord サーバー](https://discord.gg/ttsvw)（公式コミュニティ）

### 更新情報

最新のリリース情報やアップデートについては、GitHubリポジトリのReleasesページをご確認ください。

---

## 📚 付録

### A. キーボードショートカット一覧

| ショートカット | 機能 |
|--------------|------|
| F1 | ヘルプ表示 |
| F5 | 音声認識開始/停止 |
| F6 | TTS トグル |
| F7 | ミュート切り替え |
| Ctrl+P | プロファイル切り替え |
| Ctrl+S | 設定保存 |
| Ctrl+Shift+R | 再接続 |

### B. OSCパラメータ一覧

```
/avatar/parameters/SpeechRecognizing (Boolean)
/avatar/parameters/TTSPlaying (Boolean)
/avatar/parameters/VoiceLevel (Float)
/avatar/parameters/HeartRate (Int)
/avatar/parameters/MusicPlaying (Boolean)
/chatbox/input (String, Boolean, Boolean)
/chatbox/typing (Boolean)
```

### C. 設定ファイル

設定ファイルの場所:
```
%AppData%\TTSVoiceWizard\settings.json
```

設定ファイル例:
```json
{
  "sttEngine": "whisper",
  "ttsEngine": "voicevox",
  "whisperModel": "small",
  "voicevoxSpeaker": 3,
  "voicevoxSpeed": 1.0,
  "oscEnabled": true,
  "oscSendPort": 9000,
  "oscReceivePort": 9001,
  "translationEnabled": false,
  "translationService": "deepl",
  "targetLanguage": "en"
}
```

### D. よく使うVOICEVOXコマンド

```bash
# エンジン状態確認
curl http://localhost:50021/version

# スピーカー一覧取得
curl http://localhost:50021/speakers

# 音声合成テスト
curl -X POST "http://localhost:50021/audio_query?text=こんにちは&speaker=3" \
  -H "Content-Type: application/json" \
  | curl -X POST "http://localhost:50021/synthesis?speaker=3" \
  -H "Content-Type: application/json" \
  -d @- \
  --output test.wav
```

### E. 対応言語コード

音声認識・翻訳で使用する言語コード:

| 言語 | コード |
|-----|-------|
| 日本語 | ja / ja-JP |
| 英語 | en / en-US |
| 中国語（簡体） | zh / zh-CN |
| 中国語（繁体） | zh-TW |
| 韓国語 | ko / ko-KR |
| フランス語 | fr / fr-FR |
| ドイツ語 | de / de-DE |
| スペイン語 | es / es-ES |
| イタリア語 | it / it-IT |
| ロシア語 | ru / ru-RU |

### F. GPU対応情報

#### NVIDIA CUDA対応

Whisper CUDA対応要件:
- NVIDIA GPU (Compute Capability 3.5以上)
- CUDA Toolkit 11.x 以上
- cuDNN 8.x 以上

```bash
# CUDA確認
nvidia-smi
```

VOICEVOX GPU対応:
- NVIDIA GPU (DirectML対応)
- VOICEVOX GPU版インストール

### G. ネットワーク設定

#### ファイアウォール設定

許可が必要なポート:

| ポート | プロトコル | 用途 |
|-------|----------|------|
| 9000 | UDP | VRChat OSC 送信 |
| 9001 | UDP | VRChat OSC 受信 |
| 50021 | TCP | VOICEVOX API |
| 443 | TCP | HTTPS (各種API) |

#### プロキシ設定

企業ネットワーク等でプロキシを使用する場合:

```
環境変数:
HTTP_PROXY=http://proxy.example.com:8080
HTTPS_PROXY=http://proxy.example.com:8080
```

---

<div align="center">

**TTS Voice Wizard** - VRChatでの音声コミュニケーションを革新する

Made with ❤️ for the VRChat Community

[⬆️ トップに戻る](#tts-voice-wizard)

</div>