# OSCParser-CSharp
OSC（Open Sound Control）バイナリデータ用のC#パーサー. OSCメッセージを解析し, アドレスパターン, タイプタグ, 引数などを抽出するユーティリティ.

## 事前知識
OSCの詳しい仕様については，公式の[OpenSoundControl Specification 1.0](https://opensoundcontrol.stanford.edu/spec-1_0.html)を確認すること．

## 実行環境
C#8以降（それ以前のバージョンを使う場合は，switch式を書き直すこと）

## 対応タイプフォーマット
- 標準フォーマット
    - int32
    - float32
    - OSC-string
    - OSC-blob
- 非標準フォーマット
    - True
    - False

## 使い方
- 受信したバイト配列を`Packet.TryParseOSCPacket()`の引数に入れて呼び出すと，パースできた場合`true`が返り第2引数にパース後の`Packet`データが格納される．
    - Packetのaddressにはアドレスパターンが格納される．
    - Packetのargumentsには，OSCの引数が格納される．
        - 各argumentのvalueは`object`にアップキャストされているので，`type`フィールドを確認したり`is`キーワードを使うことでダウンキャストして利用すること．
        - 対応する型については，[Packet.Argument](./OSCPacket.cs)の`ToString()`メソッドを参照すること.
```c#
using OSCPacket;

// ...

byte[] bytes = // Assign the received byte array.
if (Packet.TryParseOSCPacket(bytes, out Packet oscPacket))
{
    string address = oscPacket.address;

    Argument firstArgument = oscPacket.arguments[0];
    if (firstArgument.value is int i)
    {
        // ...
    }

    Argument secondArgument = oscPacket.arguments[1];
    if (secondArgument.type == ArgumentType.Float32)
    {
        float value = (float) secondArgument.value;
        // ...
    }
}
```

## ライセンス
[MITライセンス](./LICENSE)
