namespace MyProj.Views.Sample;

using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using MyProj;
using MyProj.Infra;
using MyProj.Services;

/// 綜合示例視圖：把本規範的 View／Vm 寫法一次展示完，包含
/// <list type="bullet">
/// <item>巢狀 Vm（常態：View 與 Vm 一一對應）與 <see cref="AppViewBase{TVm}"/> 泛型基類；</item>
/// <item><c>Build</c> 由基類傳入強型別 Vm、<c>OnLoaded</c> 用基類的 <c>vm</c> 訪問器；</item>
/// <item>關鍵控件提為 public 成員；<c>MkXxx()</c> 抽子區塊；</item>
/// <item>雙向／單向綁定、樣式類名走 <c>Cls</c> 常量、UI 文本走 <c>Todo.I18n</c>；</item>
/// <item>項目模板 <c>FuncDataTemplate</c> 的可空處理；</item>
/// <item>非同步操作經 <c>Fire</c> 收斂例外（事件處理器不寫 <c>async void</c>）。</item>
/// </list>
public partial class ViewSample : AppViewBase<ViewSample.Vm>{

	/// 唯一的無參構造器。View 只能有無參構造器：
	/// Declarative 的視圖工廠與依賴注入都依賴它。
	public partial ViewSample();

	// ── 關鍵控件：獨立為 public 成員，方便設計規劃與測試 ──

	public Button? _BtnAdd;

	public Button? _BtnReload;

	public Button? _BtnRemove;

	public TextBox? _InputName;

	public ListBox? _ListNames;

	public TextBlock? _StatusText;

	/// <param name="vm">本視圖的 ViewModel。</param>
	protected override partial object Build(Vm vm);
	protected override partial void OnAfterInitialized();
	protected override partial StyleGroup? BuildStyles();

	/// <param name="vm">本視圖的 ViewModel。</param>
	public partial Control MkToolbar(Vm vm);

	/// <param name="vm">本視圖的 ViewModel。</param>
	public partial Control MkList(Vm vm);

	/// <param name="vm">本視圖的 ViewModel。</param>
	public partial Control MkInputRow(Vm vm);

	/// 啟動不可 await 的非同步操作，並把例外收斂到狀態列。
	/// 事件處理器不能寫成 <c>async void</c>，故統一走這裡。
	/// <param name="vm">本視圖的 ViewModel。</param>
	/// <param name="Op">已啟動的操作。</param>
	private partial void Fire(Vm vm, Task<nil> Op);

	/// <param name="vm">本視圖的 ViewModel。</param>
	/// <param name="Op">已啟動的操作。</param>
	private partial Task<nil> FireCore(Vm vm, Task<nil> Op);

	public static partial class Cls{
		public const str ToolBtn = nameof(ToolBtn);
	}

	/// 本視圖的 ViewModel。
	///
	/// 巢狀寫法用於「View 與 Vm 一一對應」的常態：配對關係成為語法事實，
	/// 外部引用寫 <c>ViewSample.Vm</c>，不必為兩邊各取一個名字。
	/// 少數情況（一個 Vm 給多個 View 共用、或被非 UI 層使用）才拆成獨立的 <c>VmXxx</c> 型別。
	public partial class Vm : ViewModelBase, IMk<Vm>{

		protected Vm(){
		}

		public static partial Vm Mk();

		SvcNames SvcNames = default!;

		public partial Vm(SvcNames SvcNames);

		public ObservableCollection<str> Names{
			get;
			set{ SetProperty(ref field, value); }
		} = [];

		public str InputName{
			get;
			set{ SetProperty(ref field, value); }
		} = "";

		public str? SelectedName{
			get;
			set{ SetProperty(ref field, value); }
		}

		public str StatusText{
			get;
			set{ SetProperty(ref field, value); }
		} = Todo.I18n("就緒");

		public partial void AddName();

		public partial void RemoveSelected();

		public partial Task<nil> Load(CT Ct);
	}
}
