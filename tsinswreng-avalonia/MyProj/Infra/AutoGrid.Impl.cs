namespace MyProj.Infra;

using Avalonia.Controls;
using System.Collections.Specialized;

public partial class AutoGrid{

	public partial AutoGrid(bool IsRow = true){
		this.IsRow = IsRow;
		((INotifyCollectionChanged)Children).CollectionChanged += (Sender, Args) => {
			ReIndex();
		};
	}

	protected partial void ReIndex(){
		for(var i = 0; i < Children.Count; i++){
			var one = Children[i];
			if(IsRow){
				SetRow(one, i);
			}else{
				SetColumn(one, i);
			}
		}
	}
}
