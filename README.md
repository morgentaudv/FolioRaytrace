# FolioRaytrace

C#の勉強で作ったCPUでRaytraceを行い、画像を描画するアプリケーションです。

## 使い方

ビルドしたファイルに各種コマンドをつけて実行します。
現在はデフォルトのシーン（レベル）だけ描画ができます。

``` csharp
.\FolioRaytrace.exe --output_default_path --parallel --sample_level 3 --use_default_world --image_width 1280 --image_height 720
```

![Default_20240722_144501](https://github.com/user-attachments/assets/2d3f36b6-2138-4d42-965d-1949897c8309)

## コマンド

```
--output_default_path : 描画画像をデフォルトパスに出力します。
-o, --output [Path] : 指定[Path]に画像を出力します。
--use_default_world : デフォルトシーン（レベル）を描画対象とします。
-d, --debug : デバッグ情報を画面左上に描画します。
--parallel : CPUのすべてのスレッドを並列で動かし描画速度を速めます。
--sample_level : カメラから飛ばすRayのサンプリング制度を指定します。
--image_width : 出力画像の横サイズを指定します。
--image_height : 出力画像の縦サイズを指定します。
```

