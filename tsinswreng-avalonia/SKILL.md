---

name: tsinswreng-avalonia

description: Avalonia 項目開發規範。

---

## 總則

- UI 一律用 `Avalonia.Markup.Declarative` 以**純 C# 聲明式**寫： 不建立 `.axaml`、不寫 XAML。
- 綁定一律用生成的強型別鏈式方法，禁止 `new Binding("字串路徑")` 這類寫法。

## 命名規範

- 視圖(View)用 View前綴
- 視圖模型(ViewModel) 用Vm前綴

## 文件位置

例:

```
MyProj/Views/
	Login/
		ViewLogin.cs
		ViewLogin.Impl.cs
		VmLogin.cs
		VmLogin.Impl.cs
	UserProfile/
		ViewUserProfile.cs
		ViewUserProfile.Impl.cs
		VmUserProfile.cs
		VmUserProfile.Impl.cs
```

- ViewXxx和VmXxx須同時放在名为Xxx的文件夾下
- 遵守 《聲明與實現分離》的規範、類型和所有函數都聲明爲partial、`Xxx.cs`中不寫函數實現、函數實現都寫在`Xxx.Impl.cs`中

## View 與 Vm 規範

示例代碼：

`VmSample.cs`（Vm 聲明）:

```cs
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
```

`VmSample.Impl.cs`（Vm 實現）:

```cs
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
```

`ViewSample.cs`（View 聲明）:

```cs
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
```

`ViewSample.Impl.cs`（View 實現）:

```cs
namespace MyProj.Views.Sample;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Styling;
using Tsinswreng.Avln.Dsl;
using Tsinswreng.Avln.Grid;

// 內部類 Vm 用別名引入，讓下面能直接寫 AppViewBase<Vm>。
using Vm = VmSample;
public partial class ViewSample{

	// 靜態構造器用來初始化轉值器這類靜態欄位。靜態構造器不能寫 partial。
	static ViewSample(){
		ConvInputToBtnEnabled = new FnConvtr<str, bool>(
			Input => !string.IsNullOrWhiteSpace(Input));
	}

	// View 只能有無參構造器。這裡解析 Vm 並交給基類的 vm，也就是設置 DataContext；
	// 庫的延遲初始化會在 DataContext 變成相容型別時立刻建立控件樹。
	public partial ViewSample(){
		// 一律用 vm 這一個名字，不要有時寫 vm、有時寫 ViewModel。
		// 若`App.DiOrMk`不存在則應停下來請示用戶, 勿擅自行動
		vm = App.DiOrMk<Vm>();
	}

	// 樣式有重複時才抽到這裡；類名一律用 Cls 常量，禁止硬編碼字串。
	protected override partial StyleGroup? BuildStyles(){
		return [
			// 工具列按鈕：統一樣式，故抽到這裡。
			new Style<Button>(sel => sel.Class(Cls.ToolBtn))
				.Margin(new Thickness(0, 0, 8, 0))
				.MinWidth(96),
		];
	}

	// 控件樹由回傳值描述。佈局用成員 Root：它是 StackGrid，子項一加進去就依順序自動落號，
	// 故不必自己算 Grid_Row、Grid_Column。
	// Vm 由泛型基類傳入，型別已確定，故不判空。
	protected override partial object Build(Vm vm){
		// 四列單欄：工具列 / 清單 / 輸入列 / 狀態列。
		Root.Rows("Auto,*,Auto,Auto")
		.Children(
			MkToolbar(vm),
			MkList(vm),
			MkInputRow(vm),
			new TextBlock()
				.With(t => {
					_StatusText = t;//在With中初始化關鍵控件這樣寫
				})
				.Text(vm, x=>x.StatusText)//綁定寫法
				.Margin(new Thickness(12, 0, 12, 10))
		);

		//組織子控件並加入控件樹時、代碼塊的嵌套 要和 樹的邏輯結構 保持一致
		//如上方的示例的包裹層級是StackGrid>MkToolbar,MkList,MkInputRow,狀態列
		//則在代碼中嵌套關係也要保持一致、即StackGrid縮進層級最少、往後依次增多
		return Root;
	}

	// 生命週期回調：控件樹與樣式都建好之後由庫觸發，耗時初始化放這裡。
	// 它回傳 void，不能 await，故呼叫 Vm 的同步命令；非同步工作由 Vm 內部的 Fire 啟動。
	protected override partial void OnAfterInitialized(){
		vm.Reload();
	}

	// 抽出的子區塊：只在嵌套過深、或該區塊相對獨立時才抽，不要為了拆函數而拆函數。
	public partial Control MkToolbar(Vm vm){
		return new StackPanel()
			.Orientation(Orientation.Horizontal)
			.Margin(new Thickness(12))
			.Children(
				new Button()
					.With(b => {
						_BtnReload = b;
						b.Classes.Add(Cls.ToolBtn);
						// 事件處理器是 void，不能 await，故呼叫 Vm 的同步命令。
						b.OnClick(_ => vm.Reload());
					})
					// 臨時的 未定的 UI 文本走 Todo.I18n，禁止硬編碼。
					.Content(Todo.I18n("重新載入")),

				new Button()
					.With(b => {
						_BtnRemove = b;
						b.Classes.Add(Cls.ToolBtn);
						b.OnClick(_ => vm.RemoveSelected());
					})
					.Content(Todo.I18n("移除選中"))
			);
	}

	// 清單：來源與選中項都綁到 Vm。
	public partial Control MkList(Vm vm){
		return new ListBox()
			.With(l => {
				_ListNames = l;
			})
			// 綁定的第二個參數是 Vm 實例、第三個是成員選擇器；這是編譯期綁定，AOT 安全。
			.ItemsSource(vm, x=>x.Names)
			.SelectedItem(vm, x=>x.SelectedName, BindingMode.TwoWay)
			.Margin(new Thickness(12, 0))
			// 項目模板：每一項是一個 TextBlock，文字就是該項的字串。
			// 虛擬化回收容器時，模板會被以 null 呼叫，故這裡判空之後回 null。
			.ItemTemplate(new FuncDataTemplate<str>((Item, _) => {
				if(Item is null){
					return null;
				}
				return new TextBlock().Text(Item);
			}));
	}

	// 輸入列：輸入框與加入按鈕。不需要指定 BindingMode 的綁定就不要寫。
	public partial Control MkInputRow(Vm vm){
		return new StackGrid(IsRow: false)
			.Cols("*,Auto")
			.Margin(new Thickness(12, 10))
			.Children(
				new TextBox()
					.With(t => {
						_InputName = t;
						// 直接屬性（direct property）沒有生成鏈式方法，只能在 With 裡賦值。
						t.PlaceholderText = Todo.I18n("輸入名字後按 Enter 或「加入」");
						t.OnKeyDown(e => {
							if(e.Key == Key.Enter){
								// 吃掉按鍵，免得輸入框自己再處理一次。
								e.Handled = true;
								vm.AddName();
							}
						});
					})
					// 雙向綁定寫法
					.Text(vm, x=>x.InputName, BindingMode.TwoWay),

				new Button()
					.With(b => {
						_BtnAdd = b;
						//添加樣式類名
						b.Classes.Add(Cls.ToolBtn);
						// 同步、非耗時的事件直接在這裡處理。
						b.OnClick(_ => vm.AddName());
					})
					// 轉值器把輸入框內容轉成可用狀態：空輸入時按鈕不可用。
					.IsEnabled(vm, x=>x.InputName, converter: ConvInputToBtnEnabled)
					.Content(Todo.I18n("加入"))
					.Margin(new Thickness(8, 0, 0, 0))
			);
	}
}
```

若有缺少的 沒定義的符號(如`AppViewBase<>`, `Todo.I18n()`等)則應停下來請示用戶, 禁止擅自行動

## 再次強調

### 注意事項

- 所有代碼要兼容AOT
- 耗時/異步初始化不得阻塞UI創建
- 臨時的UI文本用`Todo.I18n`
- 字體大小等共用樣式優先從項目統一配置調用, 不要到處硬編碼
- 如果項目缺少本 skill 依賴的基礎設施，如 `IMk<>`、`Todo.I18n()`、綁定輔助器、View/Vm 基類，立即請示用戶
- 在ViewXxx中把關鍵控件提到public成員。
- 聲明中的註釋要非常詳細、把具體功能樣式交互流程等都寫清楚。讓人看到Decl就像看到了文檔一樣。
- 多用`StackGrid`代替`Grid`

### 禁止事項

- 不要用 `new Binding("...")` 或其他字符串路徑綁定。因 這是不兼容AOT的
- 不要在 Vm 層做視圖跳轉、操作 View 控件、耦合 View 層細節
- 不要把耗時操作寫進 View 構造器、同步按鈕事件、或直接阻塞 UI 線程
- 不要硬編碼 UI 文本、字體大小、樣式類名
- 不要爲了“拆函數而拆函數”；只有在 Build 嵌套過深或子區塊相對獨立時才抽 `MkXxx()`
- 不要手動給`StackGrid`設置行號和列號, 應讓他自動分配
