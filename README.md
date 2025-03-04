# MultiLevelCascadeFilterSort
  
  
## === Japanese Version ===  
  
MultiLevelCascadeFilterSort は、多段階カスケードビューを管理するための C# ライブラリです。  
各アイテムに一意の整数 ID を割り当て、関連する子ビューとの連携をサポートします。  
主なコンポーネントは以下の通りです。  
  
#### 【MultiLevelCascadeFilterSort 名前空間】  
・CascadeCollectionBase：  
　　アイテムに一意の ID を割り当て、子ビューへの変更伝播を行う基本コレクション。  
・CascadeViewBase：  
　　基本コレクションと連携して動作するビューの基底クラス。フィルタリング、ソート、移動などの操作をサポートします。  
・CascadeViews.Helper：  
　　- CascadeComparer：基本コレクション内のアイテムを、カスタムコンパレータを使用して比較する IComparer<int> の実装。  
　　- CascadeEnumerator：内部の ID リストに基づいて、ビュー内のアイテムを列挙するためのカスタム列挙子。  
  
#### 【主な機能】  
・一意の ID 割り当て（UniqueNumberGenerator の利用）  
・DualKeyDictionary による効率的なアイテム管理  
・基本コレクションでの追加、削除、並び替え操作が子ビューに自動伝播  
・カスタムソート機能（RedoLastSort 等によるソートの再適用）  
・カスタム列挙子によるビュー内のアイテムの列挙  
  
#### 【使用例】  
// 例: 基本コレクションにアイテムを追加する  
var baseCollection = new YourConcreteCascadeCollection(); // CascadeCollectionBase を継承した具象クラスを使用  
int newItemId = baseCollection.Add(new YourItem());  
if(newItemId != -1)  
{  
    Console.WriteLine("アイテムが追加されました。ID: " + newItemId);  
}  
  
※ YourConcreteCascadeCollection および YourItem は、実際の実装に合わせて置き換えてください。  
  
  
## === English Version ===  

MultiLevelCascadeFilterSort is a C# library for managing multi-level cascade views.
It assigns a unique integer ID to each item and supports linking with associated child views.
The main components are as follows:

#### 【MultiLevelCascadeFilterSort Namespace】  
• CascadeCollectionBase:  
    A base collection that assigns unique IDs to items and propagates changes to child views.  
• CascadeViewBase:  
    The base class for views that work in conjunction with the base collection, supporting operations such as filtering, sorting, and moving items.  
• CascadeViews.Helper:  
    - CascadeComparer: An implementation of IComparer<int> that compares items from the base collection using a custom comparer.  
    - CascadeEnumerator: A custom enumerator that iterates through items in a view based on an internal ID list.  
  
#### 【Key Features】  
• Unique ID assignment (using a UniqueNumberGenerator)  
• Efficient item management using a DualKeyDictionary  
• Automatic propagation of add, remove, and sort operations from the base collection to child views  
• Custom sorting functionality (including re-applying the last sort with RedoLastSort)  
• Item enumeration using a custom enumerator  
  
#### 【Usage Example】  
// Example: Adding an item to the base collection  
var baseCollection = new YourConcreteCascadeCollection(); // Use a concrete class inheriting from CascadeCollectionBase  
int newItemId = baseCollection.Add(new YourItem());  
if(newItemId != -1)  
{  
    Console.WriteLine("Item added. ID: " + newItemId);  
}  
  
※ Replace YourConcreteCascadeCollection and YourItem with the appropriate class names and types based on your implementation.  
  
  
### Dependencies  
[MikouTools](https://github.com/Mikou2761210/MikouTools)
