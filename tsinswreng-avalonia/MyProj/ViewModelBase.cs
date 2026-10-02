namespace MyProj.Infra;

using CommunityToolkit.Mvvm.ComponentModel;

[Doc(@"所有 ViewModel 的基類。
生命週期約定：
由依賴注入建立的 Vm，其唯一 public 構造器最後必須調用 Init。
任何需要依賴的 public 方法開頭應調用 CheckInit，及早暴露「用 Mk() 造出來卻當成注入實例使用」的錯誤。
屬性通知用 SetProperty(ref field, value)（來自 ObservableObject），搭配 C# 的 field 關鍵字。
")]
public partial class ViewModelBase : ObservableObject{

	[Doc(@"供子類以「不注入依賴」方式建立時使用（見 Mk()）。
設為 protected：禁止外部直接 new 出一個沒有依賴的 Vm。")]
	protected ViewModelBase(){
	}

	[Doc("是否已完成初始化。由 Init 設為 true。")]
	public bool IsInited{get;set;} = false;

	[Doc("標記初始化完成。由依賴注入構造器在設置完所有依賴之後調用。")]
	public partial void Init();

	[Doc("檢查是否已初始化；未初始化則拋出異常。用於需要依賴的 public 方法開頭，讓「漏調 Init」在第一次調用時就暴露。")]
	public partial void CheckInit();

	[Doc(@$"啟動一個非同步操作，失敗時交給 {nameof(HandleErr)}。
事件處理器與生命週期回調都回傳 void，不能 await，所以 Vm 啟動非同步工作一律走這裡，不要直接呼叫會拋例外的方法。
#Prm[已啟動的操作]
")]
	protected partial void Fire(Task<nil> Op);

	[Doc(@$"失敗的處理入口。由 {nameof(Fire)} 在 catch 裡呼叫，也可以由 Vm 自己在 catch 之後呼叫。
實作待定。
#Prm[要處理的例外]
")]
	protected partial void HandleErr(Exception Ex);
}
