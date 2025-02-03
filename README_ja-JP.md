<img height="128" align="left" src="assets/logo.png" alt="Logo">

# レノボ・レギオン・ツールキット

[![Build](https://github.com/BartoszCichecki/LenovoLegionToolkit/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/BartoszCichecki/LenovoLegionToolkit/actions/workflows/build.yml)
[![Crowdin](https://badges.crowdin.net/llt/localized.svg)](https://crowdin.com/project/llt)
[![Join Discord](https://img.shields.io/discord/761178912230473768?label=Legion%20Series%20Discord)](https://discord.com/invite/legionseries)
<a href="https://hellogithub.com/repository/dd55be3ac0c146208259f17b29d2162f" target="_blank"><img src="https://abroad.hellogithub.com/v1/widgets/recommend.svg?rid=dd55be3ac0c146208259f17b29d2162f&claim_uid=LBbuUlZqTIm1JAP&theme=small" alt="Featured｜HelloGitHub" /></a>

---

#### 他の言語バージョンのこのREADMEファイル：
* [英語版のREADME](README.md)
* [簡体字中国語版のREADME](README_zh-hans.md)

---

![Ukrainian Flag](assets/ukraine_flag_bar.png)

ロシアの侵略によって被害を受けたウクライナの武装勢力と人々を支援するために、ウクライナの公式募金プラットフォームであるUNITED24で募金を行ってください：https://u24.gov.ua.

**Слава Україні!**

![Ukrainian Flag](assets/ukraine_flag_bar.png)

<br />

レノボ・レギオン・ツールキット（LLT）は、レノボ・レギオン（および類似の）シリーズのノートパソコン向けに作成されたユーティリティで、Lenovo VantageやLegion Zoneでのみ利用可能な機能を変更することができます。

バックグラウンドサービスを実行せず、メモリを少なく使用し、CPUをほとんど使用せず、テレメトリを含みません。Lenovo Vantageと同様に、このアプリケーションはWindows専用です。

_レギオンシリーズのDiscordに参加してください：https://discord.com/invite/legionseries!_

<img src="assets/screenshot_main.png" width="700" />

&nbsp;

# 目次
  - [免責事項](#免責事項)
  - [ダウンロード](#ダウンロード)
  - [互換性](#互換性)
  - [機能](#機能)
  - [寄付](#寄付)
  - [クレジット](#クレジット)
  - [FAQ](#faq)
  - [引数](#引数)
  - [ログの収集方法](#ログの収集方法)
  - [貢献](#貢献)

## 免責事項

**このツールには保証がありません。自己責任で使用してください。**

このREADMEを注意深く読んでください。重要な情報が含まれています。

## ダウンロード

次の方法でプログラムをダウンロードできます：

- [リリースページ](https://github.com/BartoszCichecki/LenovoLegionToolkit/releases/latest)から手動でダウンロード
- [winget](https://github.com/microsoft/winget-cli)を使用：

  ```sh
  winget install BartoszCichecki.LenovoLegionToolkit
  ```

- [Scoop](https://scoop.sh)を使用：

  ```sh
  scoop bucket add versions
  ```

  ```sh
  scoop bucket add extras
  ```
  
  ```sh
  scoop install extras/lenovolegiontoolkit
  ```
  
> [!TIP]
> Linux用のVantageの代替品を探している場合は、[LenovoLegionLinux](https://github.com/johnfanv2/LenovoLegionLinux)プロジェクトをチェックしてください。

#### 次のステップ

LLTはバックグラウンドで実行されているときに最適に動作するため、設定に移動して_自動起動_と_閉じると最小化_を有効にします。次に、VantageとHotkeysを無効にするか、アンインストールします。その後、LLTは常に起動時に実行され、VantageとHotkeysが処理していたすべての機能を引き継ぎます。

> [!WARNING]
> LLTを完全に閉じると、Windowsの電源モードや電源プランの同期、マクロ、アクションなどの一部の機能が動作しなくなります。これは、LLTがバックグラウンドサービスを実行せず、変更に応答できないためです。

#### 必要なドライバ

クリーンなWindowsインストールにLLTをインストールした場合、必要なドライバがインストールされていることを確認してください。ドライバが不足している場合、一部のオプションが利用できないことがあります。特に、次の2つがシステムにインストールされていることを確認してください：
1. Lenovo Energy Management
2. Lenovo Vantage Gaming Feature Driver

#### .NETの問題？

何らかの理由でLLTインストーラーが.NETを正しく設定しなかった場合：
1. https://dotnet.microsoft.com/en-us/download/dotnet/8.0にアクセスします
2. ".NET Desktop Runtime"セクションを見つけます
3. x64 Windowsインストーラーをダウンロードします
4. インストーラーを実行します

> [!NOTE]
> ScoopからLLTをインストールした場合、.NET 8は依存関係として自動的にインストールされるはずです。何かが失敗した場合、`scoop update`を使用してすべてのパッケージを更新し、`--force`引数を使用してLLTを再インストールしてみてください。

これらの手順を実行した後、ターミナルを開いて`dotnet --info`と入力できます。出力で`.NET runtimes installed`セクションを探し、このセクションに次のようなものが表示されるはずです：

`Microsoft.NETCore.App 8.0.0 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]`

および

`Microsoft.WindowsDesktop.App 8.0.0 [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]`

正確なバージョン番号は異なる場合がありますが、`8.x.x`であれば問題ありません。これらの手順を実行した後でも、LLTが起動時に.NETが見つからないなどのエラーを表示する場合、問題はLLTではなく、あなたのマシンにあります。

#### テストに協力したいですか？

[Legion Series Discord](https://discord.com/invite/legionseries)に参加し、`#legion-toolkit`チャンネルに移動してください。将来のリリースのベータ版が頻繁に投稿されます！

## 互換性

Lenovo Legion Toolkitは、Lenovo LegionノートパソコンおよびIdeapad Gaming、LOQなどの類似のノートパソコン向けに作成されています。

第6世代（MY2021）、第7世代（MY2022）、第8世代（MY2023）がサポートされていますが、一部の機能は第5世代（MY2020）でも動作します。第6世代以前のデバイスやLegion以外のデバイスに関連する問題は、このプロジェクトの範囲外です。

起動時に互換性のないメッセージが表示される場合は、下部の*貢献*セクションを確認して、どのように支援できるかを確認してください。すべてのハードウェアとすべてのオプションを互換性のあるものにすることはできない場合がありますので、ご了承ください。

**他のノートパソコンのサポートは計画されていません。**

### Lenovoのソフトウェア

全体的な推奨事項として、LLTを使用している間はVantage、Hotkeys、Legion Zoneを無効にするかアンインストールすることをお勧めします。LLTが他のLenovoアプリと一緒に動作する場合、一部の機能が競合したり、正常に動作しないことがあります。

> [!TIP]
> LLTの無効化オプションを使用するのが最も簡単な方法です。

### その他の注意事項

LLTは現在、複数のユーザーのインストールをサポートしていないため、ノートパソコンに複数のユーザーが必要な場合、問題が発生する可能性があります。管理者権限のないアカウントにも同様の問題が発生します。LLTは管理者権限のあるアカウントが必要です。管理者権限のないアカウントにLLTをインストールすると、LLTは正常に動作しません。

## 機能

このアプリは次のことができます：

- Vantageを通じてのみ利用可能な電源モード、バッテリーチャージモードなどの設定を変更します。
- Spectrum RGB、4ゾーンRGB、ホワイトバックライトキーボードのサポート。
- dGPUのアクティビティを監視します（NVIDIAのみ）。
- ノートパソコンがAC電源に接続されたときに実行されるアクションを定義します。
- バッテリースタティスティクスを表示します。
- コマンドラインからノートパソコンの機能を制御します。
- ドライバとソフトウェアの更新を確認します。
- 保証ステータスを確認します。
- Lenovo Vantage、Legion Zone、Lenovo Hotkeysサービスをアンインストールせずに無効にします。
- ...その他多数！

### カスタムモード

カスタムモードは、すべてのデバイスでサポートされています。電源モードのドロップダウンに表示され、基本的には4番目の電源モードであり、電力制限やファンの調整が可能です。カスタムモードはFn+Qショートカットでアクセスできません。すべてのデバイスがカスタムモードのすべての機能をサポートしているわけではありません。

次のBIOSを持っている場合：
* G9CN（24以上）
* GKCN（46以上）
* H1CN（39以上）
* HACN（31以上）
* HHCN（20以上）

カスタムモードが正常に機能するために、上記の最小バージョンに更新してください。

### RGBと照明

Spectrum per-key RGBと4ゾーンRGBバックライトの両方がサポートされています。競合を避けるために、Vantageとそのサービスを無効にする必要があります。他のRGBアプリを使用している場合は、[FAQ](#faq)の解決策を確認してください。

他の照明機能（1レベルおよび3レベルのホワイトキーボードバックライト、パネルロゴ、リアポートバックライトなど）もサポートされていますが、いくつかの制約があります：

* GKCN54WWおよびそれ以下のバージョン - これらのBIOSバージョンのバグにより、一部の照明機能が無効になっています
* 一部の（主に第6世代の）ノートパソコンモデルは、すべてのオプションを表示しないか、存在しないオプションを表示することがあります - これは、これらの機能の可用性を報告しない誤ったBIOS設定によるものです

Corsair iCueを必要とする照明は、LLTではサポートされていません。

> [!IMPORTANT]
> Riot Vanguard DRM（Valorantなどで使用される）は、RGBコントロールに問題を引き起こすことが知られています。RGB設定が表示されず、インストールされている場合は、起動時に実行されないようにするか、アンインストールしてください。

### ハイブリッドモードとGPU動作モード

> [!NOTE]
> ハイブリッドモード/GPU動作モードオプションはAdvanced Optimusではなく、それとは別に動作します。

dGPUを使用する主な方法は2つあります：

1. ハイブリッドモードオン - 内部ノートパソコンディスプレイが統合GPUに接続され、必要に応じてdGPUが動作し、使用されていないときに電源がオフになります。これにより、バッテリー寿命が向上します。
2. ハイブリッドモードオフ（dGPU） - 内部ノートパソコンディスプレイが直接dGPUに接続され、最高のパフォーマンスを提供しますが、バッテリー寿命が最悪になります。

これらの2つのモードを切り替えるには、再起動が必要です。

第7世代および第8世代のノートパソコンでは、ハイブリッドモードに追加の2つの設定があります：

1. ハイブリッドiGPUのみ - このモードでは、dGPUが切断されます（USBドライブの取り外しのように考えてください）。これにより、最高のバッテリー寿命を達成するために電力を使用するリスクがありません。
2. ハイブリッド自動 - 上記と同様ですが、バッテリー電源でdGPUを自動的に切断し、ACアダプタを接続すると再接続します。

dGPUが使用されている場合、dGPUは切断されないことがあり、ほとんどの場合、切断されません。これには、dGPUを使用するアプリ、外部モニターの接続、およびLenovoによって指定されていないその他のケースが含まれます。LLTの「Deactivate GPU」オプションを使用する場合、問題が発生する前に、dGPUがオフになっており、外部画面が接続されていないことを確認してください。

上記のすべての設定は、ECの組み込み機能を使用しており、これらの機能がどれだけうまく機能するかは、Lenovoのファームウェア実装に依存します。私の観察によると、頻繁に切り替えない限り、これらの機能は信頼性があります。これらの方法の変更は瞬時に行われないため、忍耐が必要です。LLTは、頻繁なハイブリッドモードの切り替えを禁止し、ECがdGPUを起動できなかった場合に追加の試行を行うことで、これらの問題を軽減しようとします。dGPUが再表示されるまでに最大10秒かかる場合があります。

問題が発生した場合は、GPU動作モードの実験的な方法を試してみてください。詳細については、[引数](#引数)セクションを参照してください。

> [!WARNING]
> デバイスマネージャーを使用してdGPUを無効にしても、デバイスは切断されず、高い電力消費を引き起こします！

### NVIDIA dGPUの無効化

dGPUがアクティブなままになることがあります。たとえば、外部画面を接続して切断した場合、一部のプロセスがdGPUで実行され続け、バッテリー寿命が短くなります。

GPUを無効にするための2つの方法があります：

1. dGPUで実行されているすべてのプロセスを強制終了します（これが最も効果的です）。
2. dGPUを短時間無効にし、すべてのプロセスを統合GPUに移動させます。

dGPUがアクティブで、ハイブリッドモードが有効で、dGPUに接続された画面がない場合、無効化ボタンが有効になります。ボタンにカーソルを合わせると、dGPUの現在のP状態とdGPUで実行されているプロセスのリストが表示されます。

> [!NOTE]
> 一部のアプリは、この機能を使用するとクラッシュすることがあります。

### NVIDIA dGPUのオーバークロック

オーバークロックオプションは、Vantageで利用可能なものと同様のシンプルなオーバークロックを目的としています。Afterburnerなどのツールの代わりにはなりません。次の点に注意してください：
* ノートパソコンにそのようなオプションがある場合、BIOSでGPUオーバークロックが有効になっていることを確認してください。
* VantageまたはLegionZoneがバックグラウンドで実行されている場合、オーバークロックは機能しません。
* 他のツール（Afterburnerなど）を使用している場合、このオプションを使用することはお勧めしません。
* ダッシュボードを編集した場合、コントロールを手動で追加する必要があるかもしれません。

### Windows電源プランとWindows電源モード

まず、LLTで表示される電源モード（またはFn+Qで切り替える）は、電源プラン（コントロールパネルからアクセスするもの）や電源モード（設定アプリから変更できるもの）とは異なります。

現代の（お勧めの）アプローチは、Windows電源モードを使用し、デフォルトの「バランス（推奨）」電源プランのみを使用することです。Windows設定アプリで選択できる3つの電源モードがあります：

* 最高の電力効率
* バランス
* 最高のパフォーマンス

これらをLLT設定で、Quiet、Balance、Performance、Customの各レギオン電源モードに割り当てることができます。これを行うと、レギオン電源モードを変更するたびに、対応するWindows電源モードが自動的に設定されます。

従来のアプローチは、複数の電源プランを使用することです。これらのプランは、一部のデバイスに工場出荷時にインストールされています。これらを使用するか、独自のプランを構成する場合、Windows設定アプリの設定をデフォルトの「バランス」に設定しておきます。LLT設定で、レギオン電源モードを変更するたびに電源プランを自動的に切り替えるように構成できます。

電源モードやプランの同期に問題がある場合、特に2つのアプローチを切り替える場合、`powercfg -restoredefaultschemes; shutdown /r /t `コマンドを使用してWindows電源設定をデフォルトにリセットできます。このコマンドは、すべての電源プランをデフォルトにリセットし、デバイスを再起動します。デフォルトの「バランス（推奨）」以外のすべてのプランが削除されるため、再度使用する予定がある場合は、コピーを作成してください。

### ブートロゴ

第6世代および第7世代のノートパソコンでは、ブートロゴ（起動時に表示されるデフォルトの「Legion」画像）を変更することができます。ブートロゴはUEFIに保存されておらず、ブートドライブのUEFIパーティションに保存されています。カスタムブートロゴを設定する際、LLTは解像度、画像形式などの基本的なチェックを行い、互換性を確保するためにチェックサムを計算します。ただし、実際の検証は次回の起動時に行われます。UEFIはUEFIパーティションから画像を読み込み、表示しようとします。何らかの理由で失敗した場合、デフォルトの画像が使用されます。解像度と画像形式以外の正確な基準は不明であり、一部の画像は表示されないことがあります。この場合、別の画像を試してみてください。

### アクションからプログラムやスクリプトを実行する

アクションの「実行」ステップを使用して、アクションから任意のプログラムやスクリプトを開始できます。構成するには、実行可能ファイル（`.exe`）またはスクリプト（`.bat`）へのパスを提供する必要があります。オプションで、スクリプトやプログラムがサポートする引数を提供することもできます。これは、コマンドラインから何かを実行するのと同じです。

<details>
<summary>例</summary>

_ノートパソコンをシャットダウン_
 - 実行可能ファイルのパス：`shutdown`
 - 引数：`/s /t 0`

_ノートパソコンを再起動_
 - 実行可能ファイルのパス：`shutdown`
 - 引数：`/r`

_プログラムを実行_
 - 実行可能ファイルのパス：`C:\path\to\the\program.exe`（プログラムがPATH変数にある場合、名前のみを使用できます）
 - 引数：` `（オプション、サポートされている引数のリストについては、プログラムのREADME、ウェブサイトなどを確認してください）

_スクリプトを実行_
 - 実行可能ファイルのパス：`C:\path\to\the\script.bat`（スクリプトがPATH変数にある場合、名前のみを使用できます）
 - 引数：` `（オプション、サポートされている引数のリストについては、スクリプトのREADME、ウェブサイトなどを確認してください）

_Pythonスクリプト_
 - 実行可能ファイルのパス：`C:\path\to\python.exe`（またはPATH変数にある場合は`python`）
 - 引数：`C:\path\to\script.py`

 </details>

#### 環境

LLTは、スクリプト内からアクセスできるいくつかの変数をプロセス環境に自動的に追加します。これらの変数は、コンテキストが必要な高度なスクリプトに役立ちます。トリガーに応じて、異なる変数が追加されます。

<details>
<summary>環境変数</summary>

- AC電源アダプタが接続されたとき
	- `LLT_IS_AC_ADAPTER_CONNECTED=TRUE`
- 低電力AC電源アダプタが接続されたとき
	- `LLT_IS_AC_ADAPTER_CONNECTED=TRUE`
	- `LLT_IS_AC_ADAPTER_LOW_POWER=TRUE`
- AC電源アダプタが切断されたとき
	- `LLT_IS_AC_ADAPTER_CONNECTED=FALSE`
- 電源モードが変更されたとき
	- `LLT_POWER_MODE=<value>`、ここで`value`は次のいずれか：`1` - Quiet、`2` - Balance、`3` - Performance、`255` - Custom
	- `LLT_POWER_MODE_NAME=<value>`、ここで`value`は次のいずれか：`QUIET`、`BALANCE`、`PERFORMANCE`、`CUSTOM`
- ゲームが実行されているとき
	- `LLT_IS_GAME_RUNNING=TRUE`
- ゲームが終了したとき
	- `LLT_IS_GAME_RUNNING=FALSE`
- アプリが開始されたとき
	- `LLT_PROCESSES_STARTED=TRUE`
	- `LLT_PROCESSES=<value>`、ここで`value`はカンマ区切りのプロセス名のリスト
- アプリが終了したとき
	- `LLT_PROCESSES_STARTED=FALSE`
	- `LLT_PROCESSES=<value>`、ここで`value`はカンマ区切りのプロセス名のリスト
- 蓋が開いたとき
	- `LLT_IS_LID_OPEN=TRUE`
- 蓋が閉じたとき
	- `LLT_IS_LID_OPEN=FALSE`
- ディスプレイがオンになったとき
	- `LLT_IS_DISPLAY_ON=TRUE`
- ディスプレイがオフになったとき
	- `LLT_IS_DISPLAY_ON=FALSE`
- 外部ディスプレイが接続されたとき
	- `LLT_IS_EXTERNAL_DISPLAY_CONNECTED=TRUE`
- 外部ディスプレイが切断されたとき
	- `LLT_IS_EXTERNAL_DISPLAY_CONNECTED=FALSE`
- HDRがオンになったとき
	- `LLT_IS_HDR_ON=TRUE`
- HDRがオフになったとき
	- `LLT_IS_HDR_ON=FALSE`
- WiFiが接続されたとき
	- `LLT_WIFI_CONNECTED=TRUE`
	- `LLT_WIFI_SSID=<value>`、ここで`value`はネットワークのSSID
- WiFiが切断されたとき
	- `LLT_WIFI_CONNECTED=FALSE`
- 指定された時間に
	- `LLT_IS_SUNSET=<value>`、ここで`value`はトリガーの設定に応じて`TRUE`または`FALSE`
	- `LLT_IS_SUNRISE=<value>`、ここで`value`はトリガーの設定に応じて`TRUE`または`FALSE`
	- `LLT_TIME"`、ここで`value`はトリガーの設定に応じて`HH:mm`
	- `LLT_DAYS"`, ここで`value`はトリガーの設定に応じて次のいずれかのカンマ区切りリスト：`MONDAY`、`TUESDAY`、`WEDNESDAY`、`THURSDAY`、`FRIDAY`、`SATURDAY`、`SUNDAY`
- 定期的なアクション
	- `LLT_PERIOD=<value>`、ここで`value`は間隔の秒数
- 起動時
	- `LLT_STARTUP=TRUE`
- 再開時
	- `LLT_RESUME=TRUE`

</details>

#### 出力

「終了を待つ」がチェックされている場合、LLTは起動されたプロセスの標準出力からの出力をキャプチャします。この出力は`$RUN_OUTPUT$`変数に保存され、「通知を表示」ステップで表示できます。

### CLI

LLTの一部の機能をコマンドラインから直接制御することができます。CLI実行可能ファイルは`llt.exe`と呼ばれ、インストールディレクトリにあります。

CLIが正常に動作するためには、LLTがバックグラウンドで実行されており、CLIオプションがLLT設定で有効になっている必要があります。`llt.exe`をPATH変数に追加して、より簡単にアクセスできるようにすることもできます。

CLIは管理者として実行する必要はありません。

<details>
<summary>機能</summary>

* `llt quickAction --list` - すべてのクイックアクションをリストします
* `llt quickAction <name>` - 指定された`<name>`のクイックアクションを実行します
* `llt feature --list` - サポートされているすべての機能をリストします
* `llt feature get <name>` - 指定された`<name>`の機能の値を取得します
* `llt feature set <name> --list` - 指定された`<name>`の機能のすべての値をリストします
* `llt feature set <name> <value>` - 指定された`<name>`の機能を指定された`<value>`に設定します
* `llt spectrum profile get` - 現在のSpectrum RGBプロファイルを取得します
* `llt spectrum profile set <profile>` - Spectrum RGBプロファイルを`<profile>`に設定します
* `llt spectrum brightness get` - 現在のSpectrum RGBの明るさを取得します
* `llt spectrum brightness set <brightness>` - Spectrum RGBの明るさを`<brightness>`に設定します
* `llt rgb get` - 現在の4ゾーンRGBプリセットを取得します
* `llt rgb set <profile>` - 4ゾーンRGBを`<preset>`に設定します

</details>

## 寄付

Lenovo Legion Toolkitを楽しんで使用している場合は、寄付を検討してください。

[PayPalで寄付](https://www.paypal.com/donate/?hosted_button_id=22AZE2NBP3HTL)

<img src="LenovoLegionToolkit.WPF/Assets/Donate/paypal_qr.png" width="200" alt="PayPal QR code" />

## クレジット

特別な感謝を：

* [ViRb3](https://github.com/ViRb3)、[Lenovo Controller](https://github.com/ViRb3/LenovoController)を作成し、このツールの基盤として使用されました
* [falahati](https://github.com/falahati)、[NvAPIWrapper](https://github.com/falahati/NvAPIWrapper)および[WindowsDisplayAPI](https://github.com/falahati/WindowsDisplayAPI)を作成しました
* [SmokelessCPU](https://github.com/SmokelessCPU)、4ゾーンRGBおよびSpectrumキーボードのサポートに協力しました
* [Mario Bălănică](https://github.com/mariobalanica)、すべての貢献に感謝します
* [Ace-Radom](https://github.com/Ace-Radom)、すべての貢献に感謝します

翻訳提供者：
* ブルガリア語 - [Ekscentricitet](https://github.com/Ekscentricitet)
* 簡体字中国語 - [凌卡Karl](https://github.com/KarlLee830)、[Ace-Radom](https://github.com/Ace-Radom)
* 繁体字中国語 - [flandretw](https://github.com/flandretw)
* チェコ語 - J0sef
* オランダ語 - Melm、[JarneStaalPXL](https://github.com/JarneStaalPXL)
* フランス語 - EliotAku、[Georges de Massol](https://github.com/jojo2massol)、Rigbone、ZeroDegree
* ドイツ語 - Sko-Inductor、Running_Dead89
* ギリシャ語 - GreatApo
* イタリア語 - [Lampadina17](https://github.com/Lampadina17)
* カラカルパク語 - KarLin、Gulnaz、Niyazbek Tolibaev、Shingis Joldasbaev
* ラトビア語 - RJSkudra
* ルーマニア語 - [Mario Bălănică](https://github.com/mariobalanica)
* スロバキア語 - Mitschud、Newbie414
* スペイン語 - M.A.G.
* ポルトガル語 - dvsilva
* ポルトガル語（ブラジル） - Vernon
* ロシア語 - [Edward Johan](https://github.com/younyokel)
* トルコ語 - Undervolt
* ウクライナ語 - [Vladyslav Prydatko](https://github.com/va1dee)、[Dmytro Zozulia](https://github.com/Nollasko)
* ベトナム語 - Not_Nhan、Kuri、Nagidrop

翻訳を監視および修正してくれたすべての人に感謝します！

## FAQ

- [Vantageをアンインストールしたのに、なぜまだ実行中のメッセージが表示されるのですか？](#Vantageをアンインストールしたのになぜまだ実行中のメッセージが表示されるのですか)
- [インストーラーにウイルス/トロイの木馬/マルウェアが含まれているとアンチウイルスが報告するのはなぜですか？](#インストーラーにウイルストロイの木馬マルウェアが含まれているとアンチウイルスが報告するのはなぜですか)
- [ホットキーをカスタマイズできますか？](#ホットキーをカスタマイズできますか)
- [コンザベーションモードのしきい値をカスタマイズできますか？](#コンザベーションモードのしきい値をカスタマイズできますか)
- [Quiet、Balance、Performanceモードでファンをカスタマイズできますか？](#QuietBalancePerformanceモードでファンをカスタマイズできますか)
- [バッテリーでPerformanceまたはCustom Power Modeに切り替えられないのはなぜですか？](#バッテリーでPerformanceまたはCustom-Power-Modeに切り替えられないのはなぜですか)
- [AIエンジンが有効な場合、Performanceモードへの切り替えがバグのように見えるのはなぜですか？](#AIエンジンが有効な場合Performanceモードへの切り替えがバグのように見えるのはなぜですか)
- [マザーボードの交換後に互換性のないメッセージが表示されるのはなぜですか？](#マザーボードの交換後に互換性のないメッセージが表示されるのはなぜですか)
- [アクションが正しく構成されているのに、ゲームが検出されないのはなぜですか？](#アクションが正しく構成されているのにゲームが検出されないのはなぜですか)
- [LLTを使用している間に他のRGBソフトウェアを使用できますか？](#LLTを使用している間に他のRGBソフトウェアを使用できますか)
- [iCue RGBキーボードはサポートされますか？](#iCue-RGBキーボードはサポートされますか)
- [RGBエフェクトを増やすことはできますか？](#RGBエフェクトを増やすことはできますか)
- [他のモデルにファンコントロールを追加できますか？](#他のモデルにファンコントロールを追加できますか)
- [LLTアイコンにカーソルを合わせたときにカスタムツールチップが表示されないのはなぜですか？](#LLTアイコンにカーソルを合わせたときにカスタムツールチップが表示されないのはなぜですか)
- [CPUをオーバークロック/アンダーボルトする方法は？](#CPUをオーバークロックアンダーボルトする方法は)
- [GPUをオーバークロックしすぎた場合はどうすればよいですか？](#GPUをオーバークロックしすぎた場合はどうすればよいですか)
- [ブートロゴが適用されないのはなぜですか？](#ブートロゴが適用されないのはなぜですか)
- [スマートFnロックを使用しているときにスタッタリングが発生するのはなぜですか？](#スマートFnロックを使用しているときにスタッタリングが発生するのはなぜですか)
- [私のノートパソコンはどの世代ですか？](#私のノートパソコンはどの世代ですか)

#### Vantageをアンインストールしたのに、なぜまだ実行中のメッセージが表示されるのですか？

バージョン2.14.0以降、LLTはVantageに関連する残りのプロセスの検出に関して非常に厳格です。Vantageは3つのコンポーネントをインストールします：

1. Lenovo Vantageアプリ
2. Lenovo Vantageサービス
3. System Interface Foundation V2デバイス

最も簡単な解決策は、LLT設定に移動し、Lenovo Vantage、LegionZone、Hotkeysを無効にするオプションを選択することです（インストールされているもののみが表示されます）。

それらを削除したい場合は、すべての3つをアンインストールすることを確認してください。そうしないと、LLTの一部のオプションが利用できなくなります。タスクマネージャーで`Vantage`または`ImController`を含むプロセスを確認できます。`ImController`プロセスを削除するのに問題がある場合は、[System Interface Foundation V2デバイスのアンインストール](https://support.lenovo.com/us/en/solutions/HT506070)ガイドを参照してください。

#### インストーラーにウイルス/トロイの木馬/マルウェアが含まれているとアンチウイルスが報告するのはなぜですか？

LLTは、多くの低レベルのWindows APIを使用しており、アンチウイルスによって疑わしいと誤ってフラグが立てられることがあります。LLTはオープンソースであり、このソフトウェアが何をしているのか疑問がある人は誰でも簡単に監査できます。すべてのインストーラーはGitHubで直接ビルドされており、GitHub Actionsを使用しているため、含まれているものに疑問の余地はありません。この問題は、すべてのコードに署名することで解決できますが、拡張検証証明書に年間数百ドルを費やす余裕はありません。

このプロジェクトのウェブサイトからインストーラーをダウンロードした場合、心配する必要はありません - 警告は誤検知です。問題の解決に協力できる場合は、連絡を取りましょう。

#### ホットキーをカスタマイズできますか？

LLT設定でFn+F9ホットキーをカスタマイズできます。他のホットキーはカスタマイズできません。

#### コンザベーションモードのしきい値をカスタマイズできますか？

いいえ。コンザベーションモードのしきい値は、ファームウェアで60％（2021年およびそれ以前）または80％（2022年およびそれ以降）
