namespace MyProj.Views.Sample;

using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using MyProj.Infra;
using MyProj.Services;

/// <summary>
/// 綜合示例視圖：把本規範的 View／Vm 寫法一次展示完，包含
/// <list type="bullet">
/// <item>巢狀 Vm（常態：View 與 Vm 一一對應）與 <see cref="AppViewBase{TVm}"/> 泛型基類；</item>
/// <item><c>Build</c> 由基類傳入強型別 Vm、<c>OnLoaded</c> 用基類的 <c>vm</c> 訪問器；</item>
/// <item>關鍵控件提為 public 成員；<c>MkXxx()</c> 抽子區塊；</item>
/// <item>雙向／單向綁定、樣式類名走 <c>Cls</c> 常量、UI 文本走 <c>Todo.I18n</c>；</item>
/// <item>項目模板 <c>FuncDataTemplate</c> 的可空處理；</item>
/// <item>非同步操作經 <c>Fire</c> 收斂例外（事件處理器不寫 <c>async void</c>）。</item>
/// </list>
/// </summary>
public partial class ViewSample : AppViewBase<ViewSample.Vm>{

	/// <summary>
	/// 唯一的無參構造器。View 只能有無參構造器：
	/// Declarative 的視圖工廠與依賴注入都依賴它。
	/// </summary>
	public partial ViewSample();

	// ── 關鍵控件：獨立為 public 成員，方便設計規劃與測試 ──

	/// <summary>「加入」按鈕：把輸入框內容加進清單。</summary>
	public Button? _BtnAdd;

	/// <summary>「重新載入」按鈕：重新向服務要資料（非同步）。</summary>
	public Button? _BtnReload;

	/// <summary>「移除選中」按鈕。</summary>
	public Button? _BtnRemove;

	/// <summary>名字輸入框：雙向綁定到 <c>Vm.InputName</c>，回車即加入。</summary>
	public TextBox? _InputName;

	/// <summary>名字清單：綁定 <c>Vm.Names</c> 與 <c>Vm.SelectedName</c>。</summary>
	public ListBox? _ListNames;

	/// <summary>狀態文字：綁定 <c>Vm.StatusText</c>。</summary>
	public TextBlock? _StatusText;

	/// <summary>建立控件樹。Vm 由泛型基類傳入，型別已確定，故不判空。</summary>
	/// <param name="vm">本視圖的 ViewModel。</param>
	protected override partial object Build(Vm vm);

	/// <summary>建立本視圖的樣式。</summary>
	protected override partial StyleGroup? BuildStyles();

	/// <summary>控件樹就緒後的初始化；耗時操作放這裡，不要阻塞界面建立。</summary>
	protected override partial void OnLoaded();

	/// <summary>工具列區塊。</summary>
	/// <param name="vm">本視圖的 ViewModel。</param>
	public partial Control MkToolbar(Vm vm);

	/// <summary>名字清單區塊。</summary>
	/// <param name="vm">本視圖的 ViewModel。</param>
	public partial Control MkList(Vm vm);

	/// <summary>輸入列區塊。</summary>
	/// <param name="vm">本視圖的 ViewModel。</param>
	public partial Control MkInputRow(Vm vm);

	/// <summary>
	/// 啟動不可 await 的非同步操作，並把例外收斂到狀態列。
	/// 事件處理器不能寫成 <c>async void</c>，故統一走這裡。
	/// </summary>
	/// <param name="vm">本視圖的 ViewModel。</param>
	/// <param name="Op">已啟動的操作。</param>
	private partial void Fire(Vm vm, Task<nil> Op);

	/// <summary><see cref="Fire"/> 的執行體。</summary>
	/// <param name="vm">本視圖的 ViewModel。</param>
	/// <param name="Op">已啟動的操作。</param>
	private partial Task<nil> FireCore(Vm vm, Task<nil> Op);

	/// <summary>樣式類名常量。禁止用字串硬編碼類名。</summary>
	public static partial class Cls{
		/// <summary>工具列按鈕。</summary>
		public const str ToolBtn = nameof(ToolBtn);
	}

	/// <summary>
	/// 本視圖的 ViewModel。
	///
	/// 巢狀寫法用於「View 與 Vm 一一對應」的常態：配對關係成為語法事實，
	/// 外部引用寫 <c>ViewSample.Vm</c>，不必為兩邊各取一個名字。
	/// 少數情況（一個 Vm 給多個 View 共用、或被非 UI 層使用）才拆成獨立的 <c>VmXxx</c> 型別。
	/// </summary>
	public partial class Vm : ViewModelBase, IMk<Vm>{

		/// <summary>供 <see cref="Mk"/> 使用的無參構造器。</summary>
		protected Vm(){
		}

		/// <summary>建立不注入任何依賴的實例（單元測試用）。</summary>
		public static partial Vm Mk();

		/// <summary>依賴：示例資料來源。</summary>
		SvcNames SvcNames = default!;

		/// <summary>唯一的 public 構造器，供依賴注入。</summary>
		/// <param name="SvcNames">示例資料來源。</param>
		public partial Vm(SvcNames SvcNames);

		/// <summary>名字清單。</summary>
		public ObservableCollection<str> Names{
			get;
			set{ SetProperty(ref field, value); }
		} = [];

		/// <summary>輸入框內容（雙向綁定）。</summary>
		public str InputName{
			get;
			set{ SetProperty(ref field, value); }
		} = "";

		/// <summary>清單中選中的名字。</summary>
		public str? SelectedName{
			get;
			set{ SetProperty(ref field, value); }
		}

		/// <summary>狀態文字。</summary>
		public str StatusText{
			get;
			set{ SetProperty(ref field, value); }
		} = Todo.I18n("就緒");

		/// <summary>把輸入框的名字加入清單；輸入為空時用「未命名」。</summary>
		public partial void AddName();

		/// <summary>移除清單中選中的名字。</summary>
		public partial void RemoveSelected();

		/// <summary>向服務載入名字清單（耗時）。</summary>
		/// <param name="Ct">取消令牌。</param>
		public partial Task<nil> LoadAsync(CT Ct);
	}
}
