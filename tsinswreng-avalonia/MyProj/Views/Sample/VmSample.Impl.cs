namespace MyProj.Views.Sample;

using Avalonia.Threading;
using MyProj.Services;

public partial class VmSample{

	public partial VmSample(
		ISvcUserCtx SvcUserCtx
		,SvcNames SvcNames
	){
		this.SvcUserCtx = SvcUserCtx;
		this.SvcNames = SvcNames;
		base.Init();//標記初始化完成。
	}

	// 沿著 protected 無參構造器建立；此實例沒有依賴，
	public static partial VmSample Mk(){
		return new VmSample();
	}

	//無參非異步函數、用于給普通按鈕綁定、只涉及ViewModel內部狀態的修改 無耗時操作
	public partial void AddName(){
		// step 1: 空輸入時用「未命名」，免得清單出現看不出是什麼的空白項。
		var name = InputName.Trim();
		if(name.Length == 0){
			//UI顯示的字符串及異常信息字符串都禁止硬編碼。
			//可臨時用Todo.I18n。
			name = Todo.I18n("未命名");
		}

		// step 2: 改集合與狀態。兩者都用 SetProperty 發通知，故界面會跟著更新。
		Names.Add(name);
		InputName = "";
		StatusText = Todo.I18n($"已加入：{name}（共 {Names.Count} 個）");
	}

	public partial void RemoveSelected(){
		if(SelectedName is null){
			StatusText = Todo.I18n("請先在清單中選一個名字");
			return;
		}

		var name = SelectedName;
		Names.Remove(name);
		SelectedName = null;
		StatusText = Todo.I18n($"已移除：{name}（共 {Names.Count} 個）");
	}

	// 非同步工作一律由 Fire 啟動
	// 不可在視圖或生命週期回調裡裸呼叫 Load，那樣例外會成為未被觀察的例外。
	public partial void Reload(){
		Fire(Load(default));
	}

	// 服務本身是非同步的，故這裡直接 await；改動被綁定的集合與屬性前，切回 UI 線程。
	public partial async Task<nil> Load(CT Ct){
		var userCtx = SvcUserCtx.GetUserCtx();
		var names = await SvcNames.GetNames(userCtx, Ct);

		await Dispatcher.UIThread.InvokeAsync(() => {
			Names.Clear();
			foreach(var one in names){
				Names.Add(one);
			}
			StatusText = Todo.I18n($"已載入 {Names.Count} 個名字（{userCtx.Name}）");
		});

		return NIL;
	}
}
