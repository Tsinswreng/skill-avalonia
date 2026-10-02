namespace MyProj.Infra;

using Avalonia.Controls;

[Doc(@"自動落號的 Grid。
子項集合每次變動，就依加入順序把行號或列號重設一遍，故不必自己算 Grid_Row、Grid_Column。
一個 AutoGrid 只能全為行或全為列，不要同時設置行和列；需要兩維時就嵌套。
需要手動指定行號列號時，改用原生 Grid。
IsRow: true 表示全為行的佈局。")]
public partial class AutoGrid : Grid{

	[Doc("全為行時為 true，全為列時為 false。")]
	public bool IsRow{get;set;} = true;

	[Doc("依 IsRow 決定這個容器管的是行還是列。")]
	public partial AutoGrid(bool IsRow = true);

	[Doc("依當前子項順序重設行號或列號。")]
	protected partial void ReIndex();
}
