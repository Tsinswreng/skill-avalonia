namespace MyProj.Infra;

using Avalonia.Controls;
using System.Collections.Specialized;

public partial class StackGrid{

	public partial StackGrid(bool IsRow = true){
		this.IsRow = IsRow;
		((INotifyCollectionChanged)Children).CollectionChanged += OnChildrenChanged;
	}

	protected partial void OnChildrenChanged(obj? Sender, NotifyCollectionChangedEventArgs Args){
		switch(Args.Action){
			case NotifyCollectionChangedAction.Add:
				if(Args.NewStartingIndex < 0 || Args.NewItems is null){
					ReIndexFrom(0);
					return;
				}
				for(var i = 0; i < Args.NewItems.Count; i++){
					SetIndex((Control)Args.NewItems[i]!, Args.NewStartingIndex + i);
				}
				return;
			case NotifyCollectionChangedAction.Remove:
				ReIndexFrom(Args.OldStartingIndex < 0 ? 0 : Args.OldStartingIndex);
				return;
			case NotifyCollectionChangedAction.Move:
			case NotifyCollectionChangedAction.Replace:
				var From = System.Math.Min(
					Args.OldStartingIndex < 0 ? 0 : Args.OldStartingIndex,
					Args.NewStartingIndex < 0 ? 0 : Args.NewStartingIndex
				);
				ReIndexFrom(From);
				return;
			default:
				return;
		}
	}

	protected partial void ReIndexFrom(i32 Start){
		for(var i = Start; i < Children.Count; i++){
			SetIndex(Children[i], i);
		}
	}

	protected partial void SetIndex(Control One, i32 Index){
		if(IsRow){
			if(GetRow(One) != Index){
				SetRow(One, Index);
			}
		}else{
			if(GetColumn(One) != Index){
				SetColumn(One, Index);
			}
		}
	}
}
