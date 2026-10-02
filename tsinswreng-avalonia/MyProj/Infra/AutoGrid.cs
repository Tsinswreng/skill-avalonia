namespace MyProj.Infra;

using Avalonia.Controls;
using System.Collections.Specialized;

[Doc(@"自動落號的 Grid。
子項加進來時依加入順序把行號或列號寫上，故不必自己算 Grid_Row、Grid_Column。
加入只看新子項，位置取自集合事件帶的索引，故加入 n 個子項的總成本是 n 次寫入。
一個 AutoGrid 只能全為行或全為列，不要同時設置行和列；需要兩維時就嵌套。
需要手動指定行號列號時，改用原生 Grid。
IsRow: true 表示全為行的佈局。")]
public partial class AutoGrid : Grid{

	[Doc("全為行時為 true，全為列時為 false。")]
	public bool IsRow{get;set;} = true;

	[Doc("依 IsRow 決定這個容器管的是行還是列。")]
	public partial AutoGrid(bool IsRow = true);

	[Doc(@"子項集合變動時落號。
加入只寫新子項；移除、移動、替換從受影響的位置起把尾段重排；清空不做事。")]
	protected partial void OnChildrenChanged(obj? Sender, NotifyCollectionChangedEventArgs Args);

	[Doc("從 Start 起把尾段重排一遍。")]
	protected partial void ReIndexFrom(i32 Start);

	[Doc("設定單個子項的號；值沒變就不寫。")]
	protected partial void SetIndex(Control One, i32 Index);
}
