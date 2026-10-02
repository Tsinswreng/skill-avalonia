namespace MyProj.Views.Sample;

using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using MyProj;
using MyProj.Infra;
using MyProj.Services;

using Vm = ViewSample.Vm;
public partial class ViewSample : AppViewBase<Vm>{

	[Doc(@"唯一的無參構造器。View 只能有無參構造器：
Declarative 的視圖工廠與依賴注入都依賴它。")]
	public partial ViewSample();

	// ── 關鍵控件：獨立為 public 成員，方便設計規劃與測試 ──

	public Button? _BtnAdd;

	public Button? _BtnReload;

	public Button? _BtnRemove;

	public TextBox? _InputName;

	public ListBox? _ListNames;

	public TextBlock? _StatusText;

	[Doc("#Prm[本視圖的 ViewModel]")]
	protected override partial object Build(Vm vm);
	protected override partial void OnAfterInitialized();
	protected override partial StyleGroup? BuildStyles();

	public partial Control MkToolbar(Vm vm);

	public partial Control MkList(Vm vm);

	public partial Control MkInputRow(Vm vm);

	public static partial class Cls{
		public const str ToolBtn = nameof(ToolBtn);
	}

	[Doc(@"本視圖的 ViewModel。
巢狀寫法用於「View 與 Vm 一一對應」的常態：配對關係成為語法事實，
外部引用寫 ViewSample.Vm，不必為兩邊各取一個名字。
少數情況（一個 Vm 給多個 View 共用、或被非 UI 層使用）才拆成獨立的 VmXxx 型別。")]
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

		[Doc("重新載入清單。同步命令，內部用 Fire 啟動非同步工作。")]
		public partial void Reload();

		public partial Task<nil> Load(CT Ct);
	}
}
