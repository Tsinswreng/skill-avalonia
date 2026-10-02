namespace MyProj.Infra;

public abstract partial class AppViewBase<TVm>{

	protected override partial void OnAfterInitialized(){
		OnLoaded();
	}

	/// <summary>
	/// 默認空實現：子類沒有「加載後初始化」需求時不必覆寫。
	/// </summary>
	protected virtual partial void OnLoaded(){
	}
}
