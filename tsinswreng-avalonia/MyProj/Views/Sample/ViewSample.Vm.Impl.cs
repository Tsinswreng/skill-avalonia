namespace MyProj.Views.Sample;

using Avalonia.Threading;
using MyProj.Infra;
using MyProj.Services;

/// <summary>
/// 巢狀 Vm 的函數實現。它與 View 在同一組檔案裡：
/// 宣告在 <c>ViewSample.cs</c>，實現放這裡，View 自己的實現放 <c>ViewSample.Impl.cs</c>。
/// </summary>
public partial class ViewSample{

	public partial class Vm{

		public partial Vm(SvcNames SvcNames){
			this.SvcNames = SvcNames;
			// 依賴設置完畢後才標記初始化完成。
			base.Init();
		}

		public static partial Vm Mk(){
			// 沿著 protected 無參構造器建立；此實例沒有依賴，
			// 需要服務的操作會被 CheckInit() 擋下。
			return new Vm();
		}

		public partial void AddName(){
			var name = InputName.Trim();
			if(name.Length == 0){
				name = Todo.I18n("未命名");
			}

			Names.Add(name);
			InputName = "";
			StatusText = Todo.I18n("已加入：{0}（共 {1} 個）", name, Names.Count);
		}

		public partial void RemoveSelected(){
			if(SelectedName is null){
				StatusText = Todo.I18n("請先在清單中選一個名字");
				return;
			}

			var name = SelectedName;
			Names.Remove(name);
			SelectedName = null;
			StatusText = Todo.I18n("已移除：{0}（共 {1} 個）", name, Names.Count);
		}

		public partial async Task<nil> LoadAsync(CT Ct){
			CheckInit();

			// step 1: 耗時工作切到線程池，防止 UI 卡頓。
			await Task.Delay(300, Ct);

			// step 2: 改動被綁定的集合與屬性時，切回 UI 線程再改。
			await Dispatcher.UIThread.InvokeAsync(() => {
				Names.Clear();
				foreach(var one in SvcNames.DefaultNames){
					Names.Add(one);
				}
				StatusText = Todo.I18n("已載入 {0} 個名字", Names.Count);
			});

			return NIL;
		}
	}
}
