namespace MyProj.Infra;

using CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// 所有 ViewModel 的基類。
///
/// 生命週期約定：
/// <list type="bullet">
/// <item>由依賴注入建立的 Vm，其唯一 public 構造器最後必須調用 <see cref="Init"/>。</item>
/// <item>任何需要依賴的 public 方法開頭應調用 <see cref="CheckInit"/>，
/// 及早暴露「用 <c>Mk()</c> 造出來卻當成注入實例使用」的錯誤。</item>
/// <item>屬性通知用 <c>SetProperty(ref field, value)</c>（來自 <see cref="ObservableObject"/>），
/// 搭配 C# 的 <c>field</c> 關鍵字。</item>
/// </list>
/// </summary>
public partial class ViewModelBase : ObservableObject{

	/// <summary>
	/// 供子類以「不注入依賴」方式建立時使用（見 <c>Mk()</c>）。
	/// 設為 protected：禁止外部直接 new 出一個沒有依賴的 Vm。
	/// </summary>
	protected ViewModelBase(){
	}

	/// <summary>
	/// 是否已完成初始化。由 <see cref="Init"/> 設為 true。
	/// </summary>
	public bool IsInited{get;set;} = false;

	/// <summary>
	/// 標記初始化完成。由依賴注入構造器在設置完所有依賴之後調用。
	/// </summary>
	public partial void Init();

	/// <summary>
	/// 檢查是否已初始化；未初始化則拋出異常。
	/// 用於需要依賴的 public 方法開頭，讓「漏調 Init」在第一次調用時就暴露。
	/// </summary>
	/// <exception cref="InvalidOperationException">尚未初始化時拋出。</exception>
	public partial void CheckInit();
}
