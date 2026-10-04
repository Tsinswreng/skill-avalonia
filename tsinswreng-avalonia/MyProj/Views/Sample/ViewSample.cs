namespace MyProj.Views.Sample;

using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using MyProj.Infra;
using Tsinswreng.Avln.Dsl;
using Tsinswreng.Avln.Grid;

// 內部類 Vm 用別名引入，讓下面能直接寫 AppViewBase<Vm>。
using Vm = VmSample;
[Doc(@"記得在這裏寫該寫的註釋")]
public partial class ViewSample : AppViewBase<Vm>{

	// 轉換器也要在宣告檔聲明、但不實現；獨立出來方便測試。
	// 初始化寫在實現檔的靜態構造器裡。
	[Doc("把輸入框的內容轉成「加入」按鈕是否可用：空輸入時不可用。")]
	public static IValConvtrWithErr ConvInputToBtnEnabled = default!;

	[Doc("唯一的無參構造器。View 只能有無參構造器，Declarative 的視圖工廠與依賴注入都依賴它。")]
	public partial ViewSample();

	// 佈局容器。大多數場景用 StackGrid 作視圖的根節點：它是 Grid 的子類，
	// 來自Tsinswreng.Avln.Grid
	// 子項一加進去就依順序自動落號，故不必自己算 Grid_Row、Grid_Column。
	// IsRow: true 表示全為行的佈局。
	// 一個 StackGrid 只能全為行或全為列，不要同時設置行和列；若需兩維則嵌套。
	// 在大多數情況 你都應該用 StackGrid 代替Grid,
	// 只有少數需要手動指定行號或列號的情況 纔改用原生 Grid。
	public StackGrid Root = new(IsRow: true);

	//頁面中的關鍵控件都要獨立作爲類的public成員、以方便設計規劃與測試。
	//關鍵控件包括:
	//涉及輸入操作交互(如按鈕,輸入框),信息展示的(如文本框);
	//子模塊/子UserControl(其他的`ViewXxx`)。
	//聲明關鍵控件時、可使用具體類型(如 `public TextBox? _CtrlCnt1`;)
	//若暫時未確定類型也可以寫`Control?`或`object?`。
	//聲明爲可空、不需要寫get;set;
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
	protected override partial object Build(VmSample vm);

	[Doc(@"生命週期回調。控件樹與樣式都建好之後由庫觸發。
耗時初始化放這裡，不能放構造器或 Build，否則會阻塞界面建立。
它回傳 void，不能 await，故只呼叫 Vm 的同步命令。
")]
	protected override partial void OnAfterInitialized();

	[Doc(@"建立本視圖的樣式。
樣式簡單時直接和控件一起初始化即可；有重複或需要批量設定時才抽到這裡。
")]
	protected override partial StyleGroup? BuildStyles();

	//當ViewSample中縮進層次過多時、或子控件是一個相對獨立的邏輯單元時、
	//可將Build中的部分代碼抽到一個單獨的函數中、把控件返回出去、再在Build中調用

	[Doc(@"工具列區塊：重新載入、移除選中。
#Prm[本視圖的 ViewModel]
#Rtn[工具列的控件]
")]
	public partial Control MkToolbar(VmSample vm);

	[Doc(@"名字清單區塊。
#Prm[本視圖的 ViewModel]
#Rtn[清單的控件]
")]
	public partial Control MkList(VmSample vm);

	[Doc(@"輸入列區塊：輸入框與加入按鈕。
#Prm[本視圖的 ViewModel]
#Rtn[輸入列的控件]
")]
	public partial Control MkInputRow(VmSample vm);

	[Doc("樣式類名常量。禁止用字串硬編碼類名。")]
	public static partial class Cls{

		[Doc("工具列按鈕的類名。")]
		public const str ToolBtn = nameof(ToolBtn);
	}
}
