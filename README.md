# 2024_TeamB

**コーディング規約**
```
・Unityのバージョンは2021.3.34f1

・クラス名、メソッド名
　PascalCase

・メンバー名
　頭に"_"をつけたうえでのcamelCase
  public変数は必ずプロパティにしてください。
　例）private int _number
　　　public int Number => _number;
　boolの変数または関数の先頭にはis,can,has等を付けてください。
　例）isFlag
　Action,Funcなどのイベントの先頭にはonを付けてください。
　例）onEvent
　定数名は全て大文字にしてください
　例）const int CONST_NUMBER
　SerializeField属性の場合でも_を付けてください
　例）[SerializeField] private int _number = 1;
　アクセス修飾子は必ず付けてください
　例）private int _number;

・ブランチ名
　大枠/機能名
　例）Player/PlayerRecord

・コミット文
　[add] : 何かスクリプト、ファイル等を追加した場合
　[update] : スクリプト、ファイル等に変更をした場合
　[remove] : スクリプト、ファイル等を削除した場合
　[clean] : スクリプト、ファイル等を移動した場合
　[fix] : スクリプト等に発生していたバグを解消した場合
　2行目以降は必ず詳しい説明を書き加えてください。

・プルリクエスト
  最低誰か一人のレビューを受けてからマージしてください。

・Unityインスペクタ
　インスペクタに公開する変数Header属性をつけてください。
　公開する必要のない変数はprivateにしてください。

・文字コード
　UTF-8でお願いします。
```
