namespace MyProj.Views.Sample;

using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using MyProj;
using MyProj.Infra;
using MyProj.Services;
using Tsinswreng.Avln.Dsl;
using Tsinswreng.Avln.Grid;

// 巢狀 Vm 在基底子句裡還不可見，故用別名引入，讓下面能直接寫 AppViewBase<Vm>。
using Vm = ViewSample.Vm;

[Doc(@"記得在這裏寫該寫的註釋")]
public partial class ViewSample : AppViewBase<Vm>{

	// 轉換器也要在宣告檔聲明、但不實現；獨立出來方便測試。
	// 初始化寫在實現檔的靜態構造器裡。
	[Doc("把輸入框的內容轉成「加入」按鈕是否可用：空輸入時不可用。")]
	public static IValConvtrWithErr ConvInputToBtnEnabled = default!;

	[Doc("唯一的無參構造器。View 只能有無參構造器，Declarative 的視圖工廠與依賴注入都依賴它。")]
	public partial ViewSample();

	// 佈局容器。大多數場景用 StackGrid 作視圖的根節點：它是 Grid 的子類，
	// 子項一加進去就依順序自動落號，故不必自己算 Grid_Row、Grid_Column。
	// 一個 StackGrid 只能全為行或全為列，不要同時設置行和列；要兩維就嵌套。
	// 需要手動指定行號列號時，改用原生 Grid。
	// IsRow: true 表示全為行的佈局。
	public StackGrid Root = new(IsRow: true);

	// 關鍵控件都要獨立作為類的 public 成員，以方便設計規劃與測試。
	// 關鍵控件包括：涉及輸入操作的（按鈕、輸入框）、信息展示的（文本框）、
	// 以及子模塊（其他的 ViewXxx）。
	// 聲明為可空、不需要寫 get;set;。
	// 控件在何處建立與賦值，見實現檔對應的 MkXxx 方法。

	[Doc("把輸入框的內容加入清單。")]
	public Button? _BtnAdd;

	[Doc("重新載入清單。")]
	public Button? _BtnReload;

	[Doc("移除清單中選中的名字。")]
	public Button? _BtnRemove;

	[Doc("名字輸入框。雙向綁定到 Vm 的 InputName；回車即加入。")]
	public TextBox? _InputName;

	[Doc("名字清單。綁定 Vm 的 Names 與 SelectedName。")]
	public ListBox? _ListNames;

	[Doc("狀態文字。綁定 Vm 的 StatusText。")]
	public TextBlock? _StatusText;

	[Doc(@"建立控件樹。根控件由回傳值描述，父子關係用 Children(...)。
Vm 由泛型基類傳入，型別已確定，故不必判空。
#Prm[本視圖的 ViewModel]
#Rtn[控件樹的根控件]
")]
	protected override partial object Build(Vm vm);

	[Doc(@"生命週期回調。控件樹與樣式都建好之後由庫觸發。
耗時初始化放這裡，不能放構造器或 Build，否則會阻塞界面建立。
它回傳 void，不能 await，故只呼叫 Vm 的同步命令。
")]
	protected override partial void OnAfterInitialized();

	[Doc(@"建立本視圖的樣式。
樣式簡單時直接和控件一起初始化即可；有重複或需要批量設定時才抽到這裡。
")]
	protected override partial StyleGroup? BuildStyles();

	// 當嵌套層次過多、或子控件是一個相對獨立的邏輯單元，
	// 可將 Build 中的部分代碼抽到單獨的函數、把控件返回出去、再在 Build 中調用。
	// 不要為了拆函數而拆函數。
	// 代碼塊的嵌套層級要和實際控件樹結構保持一致。

	[Doc(@"工具列區塊：重新載入、移除選中。
#Prm[本視圖的 ViewModel]
#Rtn[工具列的控件]
")]
	public partial Control MkToolbar(Vm vm);

	[Doc(@"名字清單區塊。
#Prm[本視圖的 ViewModel]
#Rtn[清單的控件]
")]
	public partial Control MkList(Vm vm);

	[Doc(@"輸入列區塊：輸入框與加入按鈕。
#Prm[本視圖的 ViewModel]
#Rtn[輸入列的控件]
")]
	public partial Control MkInputRow(Vm vm);

	[Doc("樣式類名常量。禁止用字串硬編碼類名。")]
	public static partial class Cls{

		[Doc("工具列按鈕的類名。")]
		public const str ToolBtn = nameof(ToolBtn);
	}

	[Doc(@"本視圖的 ViewModel。
巢狀寫法用於「View 與 Vm 一一對應」的常態：配對關係成為語法事實，
外部引用寫 ViewSample.Vm，不必為兩邊各取一個名字。
少數情況（一個 Vm 給多個 View 共用、或被非 UI 層使用）才拆成獨立的 VmXxx 型別。
Vm 只曝露狀態與命令：不做視圖跳轉、不操作控件、不耦合 View 層細節。
")]
	[Doc(@$"記得在這裏寫該寫的註釋")]
	public partial class Vm : AppVmBase, IMk<Vm>{

		[Doc(@$"所有 Vm 都要有 protected 的無參構造器,
		供{nameof(Mk)}用。")]
		protected Vm(){}

		[Doc(@"用于從外部直接創建對象、不注入依賴，單元測試也用它。
不能定義多個 public 構造器，否則依賴注入無法確定該用哪一個。")]
		public static partial Vm Mk();

		[Doc("依賴聲明。不需要加任何修飾符、不需要 {get;set;}、都初始化為 default!。")]
		ISvcUserCtx SvcUserCtx = default!;
		SvcNames SvcNames = default!;
		

		[Doc(@"唯一的 public 構造器，供依賴注入")]
		public partial Vm(
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
}
