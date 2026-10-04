namespace MyProj.Views.Sample;

using System.Collections.ObjectModel;
using MyProj;
using MyProj.Infra;
using MyProj.Services;

[Doc(@"示例視圖 ViewSample 的 ViewModel。
View 與 Vm 一律分開定義：兩者各自是頂層的 partial 類別，
外部引用直接寫 VmSample；
配對關係由「同在 Views/Sample/ 目錄下」表達。
故本模組固定四個檔案：ViewSample.cs、ViewSample.Impl.cs、VmSample.cs、VmSample.Impl.cs。
Vm 只曝露狀態與命令：不做視圖跳轉、不操作控件、不耦合 View 層細節。
")]
public partial class VmSample : AppVmBase, IMk<VmSample>{

	[Doc(@$"所有 Vm 都要有 protected 的無參構造器,
	供{nameof(Mk)}用。")]
	protected VmSample(){}

	[Doc(@"用于從外部直接創建對象、不注入依賴，單元測試也用它。
不能定義多個 public 構造器，否則依賴注入無法確定該用哪一個。")]
	public static partial VmSample Mk();

	[Doc("依賴聲明。不需要加任何修飾符、不需要 {get;set;}、都初始化為 default!。")]
	ISvcUserCtx SvcUserCtx = default!;
	SvcNames SvcNames = default!;

	[Doc(@"唯一的 public 構造器，供依賴注入")]
	public partial VmSample(
		ISvcUserCtx SvcUserCtx
		,SvcNames SvcNames
	);

	[Doc("用于綁定的屬性的getter和setter必須定義成這樣、無特殊情況(如轉發其他屬性)則必須使用field關鍵字。")]
	public str InputName{
		get;
		set{ SetProperty(ref field, value); }
	} = "";

	[Doc($"記得在成員上寫該寫的註釋")]
	public ObservableCollection<str> Names{
		get;
		set{ SetProperty(ref field, value); }
	} = [];

	public str? SelectedName{
		get;
		set{ SetProperty(ref field, value); }
	}

	public str StatusText{
		get;
		set{ SetProperty(ref field, value); }
	} = Todo.I18n("就緒");

	[Doc($"記得在函數上寫該寫的註釋")]
	public partial void AddName();
	public partial void RemoveSelected();
	public partial void Reload();
	public partial Task<nil> Load(CT Ct);
}
