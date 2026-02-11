やりたいことは、端的に言うと次の **1文** に集約できます。

---

**PICO4（PCVR／SteamVR）で取得した自分のマイク音声を、できるだけ低遅延かつ高精度に音声認識し、その結果をVOICEVOXで日本語音声として即座に生成し、VRChat内で“自分の声の代わり”として他人に聞こえる形で出力する、最小構成・最短経路の実用アプリを作りたい。**

---

これを **機能分解せずに完全に漏れなく書く**と、以下になります。

* 入力は **PICO4のマイク（PCVR経由）**
* 処理は **PC上**
* 音声は

  * 無駄なUIや装飾を挟まず
  * **リアルタイム性を最優先**し
  * **日本語で高精度に文字起こし（STT）**
* 文字起こし結果は

  * 翻訳や要約を必須とせず
  * **VOICEVOXで即TTS化**
* 出力は

  * VRChatのChatbox制約（ASCII問題）を回避し
  * **仮想マイクとしてVRChatに入力**
* 目的は

  * デモでも研究でもなく
  * **日常的にVRChatで使える実用品**
* 重視する優先順位は

  1. **実用遅延の低さ**
  2. **日本語としての認識精度・自然さ**
  3. **構成・実装の最小性（ブラックボックス可、ただし削れるなら削る）**

---

さらに一言で“思想”まで含めるなら：

> **「文字を見せたいのではなく、日本語で“喋っている体験”をVRChat上に最短で成立させたい」**


## コーディングスタイル
- **Delete and minimize**: Comments, `if`, `try`, temporaries, old files, and redundant code.
- **Architecture**: Simple Clean Architecture.
- **Focus**: Low-latency, Japanese-optimized pipeline (Whisper -> VOICEVOX -> VRChat).
