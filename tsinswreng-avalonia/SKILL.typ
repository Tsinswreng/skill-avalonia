#import "@preview/tsinswreng-auto-heading:0.1.0": auto-heading
#let H = auto-heading;
\-\-\-

name: tsinswreng-avalonia

description: Avalonia 項目開發規範。UI 一律用 Avalonia.Markup.Declarative 以純 C\# 聲明式寫，不用 XAML。

\-\-\-


//自製段落
#let P(C) = {C}
//批註標記佔位
#let Todo(Title, Body) = {}
#let Wip(Title, Body) = {}


#H[總則][
	#P[
		UI 一律用 `Avalonia.Markup.Declarative` 以*純 C\# 聲明式*寫。
		不建立 `.axaml`、不寫 XAML、不用字串路徑綁定。
	]
	#P[
		本 skill 假設項目已具備下列基礎設施。
		若缺少任何一項，立即請示用戶，
		不要自造替代品，也不要用別的 UI 寫法繞過去：
	]
	- #[`AppViewBase`：視圖基類（*非泛型*，理由見下文）。
	]
	- #[`ViewModelBase`：`ObservableObject` 派生，
		提供 `Init()`／`IsInited`／`CheckInit()`。
	]
	- #[`IMk<T>`：`public static abstract T Mk()`，
		配合 `App.DiOrMk<T>()` 做「容器有註冊就用容器、沒有就用 `Mk()`」。
	]
	- #[`Todo.I18n()`：本地化佔位（當前原樣返回輸入文字）。
	]
	- #[全域別名：至少 `str`／`obj`／`nil`／`CT`；
		常見還有 `VAlign`／`HAlign`／`BindingMode`（`Avalonia.Layout.VerticalAlignment` 等的別名）。
		通常集中放在 `GlobalUsing.cs`。
	]
	#P[
		跨層 API 慣例（與 Ngan 各前端一致）：
		所有對外 API 的第一個參數是 `IFnCtx? Ctx`（可空，當前實現多數不使用）；
		非同步 API 的最後一個參數是 `CT Ct`。
		若項目沒有 `IFnCtx`，依該項目既有寫法，並在動工前確認。
	]
]


#H[命名與文件位置][
	#H[命名規範][
		- 視圖(View) 用 `View` 前綴，例如 `ViewLogin`
		- 視圖模型(ViewModel) 用 `Vm` 前綴，例如 `VmLogin`
		- 關鍵控件成員以下劃線開頭，例如 `_BtnAdd`
		- 樣式類名一律走 `Cls` 常數（用 `nameof`），禁止字串硬編碼
		- 抽出的子區塊方法用 `Mk` 前綴並回傳控件，例如 `MkList()`
	]

	#H[文件位置][
		例：
		````
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
		````
		- #[`ViewXxx` 與 `VmXxx` 必須同放在名為 `Xxx` 的文件夾下。
		]
		- #[遵守《聲明與實現分離》：
			類型與所有函數都宣告為 `partial`；
			`Xxx.cs` 只放宣告（欄位、屬性、事件、常數的宣告也在這一半），
			函數實現全部放 `Xxx.Impl.cs`。
		]
		- #[`.Impl.cs` 裏只寫函數實現，別的一概不寫。
		]
	]
]


#H[項目配置（csproj）][
	這些都是實測踩到才加的，缺一項就會在建置期出現與真因無關的錯誤：

	- #[`Avalonia.Markup.Declarative`：聲明式寫法的本體。
		它靠*源生成器*為 Avalonia 控件生成鏈式擴展方法，
		所以控件樹才能寫成 `.Text(...)`、`.Margin(...)` 這樣。
	]
	- #[SDK band `10.0.1xx` 的使用者必須補
		`<PackageReference Include="MarkupDeclarative.Roslyn4" Version="12.1.0" PrivateAssets="all" />`。
		官方生成器以 Roslyn 5.3 構建，而該 band 的編譯器是 5.0，
		不補會得到 `CS9057`（分析器無法載入）
		外加十幾條 `CS1061`／`CS1955`（所有鏈式方法都找不到），極易誤判成「API 改名了」。
	]
	- #[`<UsedAvaloniaProducts></UsedAvaloniaProducts>`：關掉 Avalonia 遙測目標。
		該目標掛在 `BeforeTargets="CoreCompile"`，
		要寫 `%LOCALAPPDATA%\AvaloniaUI\BuildServices\buildtasks.log`；
		被拒時整個建置以 `MSB4018` 中止，而且錯誤訊息與編譯毫無關係。
	]
	- #[`PublishAot=true`（至少 Release）：本系列項目要求 AOT 相容。
		因此*禁用反射式激活*（`ActivatorUtilities`、`Activator.CreateInstance`），
		DI 一律用工廠 lambda：`Svc.AddSingleton<VmXxx>(sp => new VmXxx(...))`。
	]
	- #[建置請帶 `-nodeReuse:false -m:1`。
		不帶時會偶發 `MSB4276`（找不到 Workload 相關的 Sdk），
		表現為「0 個錯誤」但生成失敗。
	]
]


#H[Vm 規範][
	Vm 即 ViewModel。示例：

	`VmUserProfile.cs`：
	````cs
	namespace MyProj.Views.UserProfile;

	using System.Collections.ObjectModel;
	using MyProj.Infra;

	/// 記得在這裏寫註釋
	public partial class VmUserProfile: ViewModelBase, IMk<VmUserProfile>{

		// 所有 Vm 都要有 protected 的無參構造器。
		protected VmUserProfile(){}

		// 用於從外部直接建立對象、不注入依賴。單元測試也用它。
		// 不能定義多個 public 構造器，否則依賴注入無法確定該用哪一個。
		public static partial VmUserProfile Mk();

		// 聲明依賴。不需要加任何修飾符、不需要 {get;set;}、
		// 都初始化為 = default!。
		ISvcUser SvcUser = default!;

		// 唯一的 public 構造器、用於依賴注入。
		// 參數的預設值只能寫在這一半（寫在 Impl 半會得到 CS1066）。
		public partial VmUserProfile(ISvcUser SvcUser);

		// 用於綁定的屬性、getter/setter 必須定義成這樣。
		// 無特殊情況（如轉發其他屬性）必須使用 field 關鍵字。
		// 裸 {get;set;} 不發通知，會讓 Vm→View 的方向靜默失效。
		public i32 Cnt1{
			get;
			set{ SetProperty(ref field, value); }
		} = 0;

		public str Input{
			get;
			set{ SetProperty(ref field, value); }
		} = "";

		public ObservableCollection<str> List{
			get;
			set{ SetProperty(ref field, value); }
		} = ["a", "b", "c"];

		/// 記得在這裏寫註釋
		/// 所有函數（含構造器）都宣告成 partial、實現寫在 .Impl.cs 中
		public partial void Click1();

		/// 記得在這裏寫註釋
		public partial Task<nil> CallService(IFnCtx? Ctx, CT Ct);
	}
	````

	`VmUserProfile.Impl.cs`：
	````cs
	namespace MyProj.Views.UserProfile;

	using Avalonia.Threading;
	using MyProj.Infra;

	/// .Impl.cs 專門用來寫函數實現
	public partial class VmUserProfile{

		public partial VmUserProfile(ISvcUser SvcUser){
			this.SvcUser = SvcUser;
			// 依賴設置完畢後才標記初始化完成。
			base.Init();
		}

		public static partial VmUserProfile Mk(){
			// 沿著 protected 無參構造器建立；此實例沒有依賴，
			// 後續任何需要服務的操作都會被 CheckInit() 擋下。
			return new VmUserProfile();
		}

		// 無參非同步函數、用於給普通按鈕綁定、只涉及 Vm 內部狀態的修改、無耗時操作。
		public partial void Click1(){
			Cnt1++;
		}

		// 呼叫後端服務：聲明為非同步函數、函數名不需特殊後綴、最後一個參數設為 CT Ct。
		public partial async Task<nil> CallService(IFnCtx? Ctx, CT Ct){
			CheckInit();

			// step 1: 耗時工作切到線程池，防止 UI 卡頓。
			//         若項目提供了封裝（例如 RunTask）就優先用它；沒有才用 Task.Run。
			var R = await Task.Run(() => SvcUser.ServeApi1(Ctx, Ct), Ct);

			// step 2: 背景線程要改動被綁定的屬性時，切回 UI 線程再改。
			await Dispatcher.UIThread.InvokeAsync(() => {
				Input += R + "";
			});

			return NIL;
		}
	}
	````

	要點：

	- #[`base.Init()`／`IsInited`／`CheckInit()` 定義在 Vm 基類中；
		`IMk<>` 定義在項目中。若項目未定義則應請示用戶。
	]
	- #[*不在 Vm 層做視圖跳轉、不操作控件、不耦合 View 層細節*。
		Vm 只暴露狀態與命令。
	]
	- #[綁定用的屬性一律 `SetProperty(ref field, value)`；
		只有真的是「不需要通知」的純常數才用自動屬性。
	]
	- #[需要對外通知的欄位型成員（例如集合以外的自定義狀態）用 `OnPropertyChanged(nameof(...))` 手動補通知。
	]
]


#H[View 規範][
	使用 `Avalonia.Markup.Declarative`（`ViewBase` 派生）。
	視圖分為兩半：宣告（`ViewXxx.cs`）與實現（`ViewXxx.Impl.cs`）。

	`ViewUserProfile.cs`：
	````cs
	namespace MyProj.Views.UserProfile;

	using Avalonia.Controls;
	using Avalonia.Markup.Declarative;
	using MyProj.Infra;

	/// 記得在這裏寫註釋
	public partial class ViewUserProfile: AppViewBase{

		// 本視圖的 ViewModel（即 Avalonia 的 DataContext）。
		// 基類刻意保持非泛型：源生成器不對泛型基類做型別參數代換，
		// 會生成引用 TCtx 的非法代碼（CS0246）。
		// 故改為各視圖自己聲明強型別的 Ctx。
		public VmUserProfile? Ctx{
			get{ return DataContext as VmUserProfile; }
			set{ DataContext = value; }
		}

		// 頁面中的關鍵控件都要獨立作為類的 public 成員、以方便設計規劃與測試。
		// 關鍵控件包括：涉及輸入操作的（按鈕、輸入框）、信息展示的（文本框）、
		// 以及子模塊/子 UserControl（其他的 ViewXxx）。
		// 聲明為可空、不需要寫 get;set;，並用註釋說明這個控件。
		/// 點擊後把 Cnt1 加一
		public Button? _BtnAdd;

		/// 輸入框：雙向綁定到 Vm 的 Input
		public TextBox? _Input;

		/// 清單
		public ListBox? _ListBox;

		// View 必須有且只有一個無參構造器。
		public partial ViewUserProfile();

		// 聲明控件樹與樣式的建立。
		// 兩者都是基類的 override；實現寫在 .Impl.cs。
		protected override partial obj Build();
		protected override partial StyleGroup? BuildStyles();

		// 控件樹已就緒之後的回調。
		// 耗時初始化必須放這裏，不能放構造器或 Build()，否則會阻塞界面建立。
		protected override partial void OnLoaded();

		// 當嵌套層次過多、或子控件是一個相對獨立的邏輯單元、
		// 可將 Build 中的部分代碼抽到單獨的函數中、把控件返回出去、再在 Build 中調用。
		public partial Control MkList(VmUserProfile Vm);

		// 樣式類名常量。禁止用字串硬編碼類名。
		public static partial class Cls{
			public const str MenuBtn = nameof(MenuBtn);
		}
	}
	````

	`ViewUserProfile.Impl.cs`：
	````cs
	namespace MyProj.Views.UserProfile;

	using Avalonia;
	using Avalonia.Controls;
	using Avalonia.Controls.Templates;
	using Avalonia.Layout;
	using Avalonia.Markup.Declarative;
	using Avalonia.Styling;
	using MyProj.Infra;

	public partial class ViewUserProfile{

		public partial ViewUserProfile(){
			// View 的構造器一定會設好 Ctx；
			// Build() 不接受 null 綁定來源，故缺 Vm 時直接失敗比產生空界面好定位。
			Ctx = App.DiOrMk<VmUserProfile>();
		}

		protected override partial void OnLoaded(){
			var vm = Ctx;
			if(vm is null){
				return;
			}
			// 耗時初始化放這裏（例如向後端要資料）。
		}

		protected override partial obj Build(){
			var vm = Ctx ?? throw new InvalidOperationException(
				$"{nameof(ViewUserProfile)} 缺少 ViewModel（Ctx 為 null），無法建立控件樹。");

			// 控件樹由回傳值描述，父子關係用 Children(...)。
			// 代碼塊的嵌套層級要和實際控件樹結構保持一致：
			// 根節點縮進最少、越深的子控件縮進越多。
			return new Grid()
				.Cols("*,Auto")
				.Rows("Auto,*,Auto")
				.Children(
					// 抽出的子區塊也是普通控件，照樣排在 Children 裏。
					MkList(vm)
						.Grid_Column(0).Grid_ColumnSpan(2).Grid_Row(1),

					new TextBox()
						// 綁定：第二個參數是 Vm 實例、第三個是成員選擇器。
						// 這是編譯期綁定（AOT 安全）。
						.Text(vm, x=>x.Input, BindingMode.TwoWay)
						.Grid_Column(0).Grid_Row(2)
						.With(t=>{
							// 關鍵控件在這裏提出來
							_Input = t;

							// 直接屬性（direct property）沒有生成鏈式方法，
							// 只能在 With 裏直接賦值。
							t.PlaceholderText = Todo.I18n("請輸入");
						}),

					new Button()
						// UI 文本走 I18n，禁止硬編碼
						.Content(Todo.I18n("點我加一"))
						.Grid_Column(1).Grid_Row(2)
						.With(b=>{
							_BtnAdd = b;

							// 樣式類名走 Cls 常量
							b.Classes.Add(Cls.MenuBtn);

							// 同步、非耗時的事件直接在這裏處理
							b.OnClick(_ => vm.Click1());
						})
				);
		}

		protected override partial StyleGroup? BuildStyles(){
			// 樣式有重複時才抽到這裡；並且用 Cls 常數管理類名。
			return [
				new Style<Button>(sel => sel.Class(Cls.MenuBtn))
					.HorizontalAlignment(HAlign.Center)
					.VerticalAlignment(VAlign.Stretch),
			];
		}

		// 列表寫法示例
		public partial Control MkList(VmUserProfile Vm){
			return new ListBox()
				.ItemsSource(Vm.List)
				// 項目模板的參數必須可空：虛擬化回收容器時，
				// 模板會被以 null 呼叫（見「實測踩過的坑」）。
				.ItemTemplate(new FuncDataTemplate<str>((Item, _) =>
					Item is null ? null : new TextBlock().Text(Item)))
				.With(l => {
					_ListBox = l;
				});
		}
	}
	````

	#H[控件樹怎麼寫][
		- #[根節點是 `Build()` 的回傳值；父子關係用 `.Children(...)`（Panel 系）。
			`ContentControl` 用 `.Content(...)`、`Border` 等裝飾器用 `.Child(...)`。
		]
		- #[佈局以原生 `Grid` 為主：`.Cols("230,4,*,4,420")`、`.Rows("Auto,*,Auto")`，
			用星號欄吸收剩餘寬度。
			（不要自造佈局容器；Avalonia 原生的 Panel 系已經足夠）
		]
		- #[Grid 的附加屬性鏈式方法名是
			`Grid_Column`／`Grid_Row`／`Grid_ColumnSpan`／`Grid_RowSpan`。
		]
		- #[鏈式方法只對 `StyledProperty` 生成（含繼承來的）。
			直接屬性沒有鏈式方法（例：`Window.Title`、`TextBox.PlaceholderText`、`ScrollViewer` 在 Avalonia 12 的內容屬性），
			改用物件初始化器 `new Window{ Title = ... }` 或在 `.With(o=>{ o.Xxx = ...; })` 裏賦值。
		]
		- #[第三方控件若沒有生成鏈式方法，同樣退回 `With`；
			要讓生成器認得它得另外跑上游的 `AvaloniaExtensionGenerator` 工具。
		]
		- #[`.With(o => {...})` 用於「鏈式方法表達不了」的初始化（事件、直接屬性、把控件提出到成員變數）。
		]
		- #[能抽 `MkXxx()` 就抽：
			條件是嵌套過深、或該區塊是相對獨立的邏輯單元。
			不要為了拆函數而拆函數。
		]
	]

	#H[綁定怎麼寫][
		- #[優先用生成的強型別鏈式方法：`o.Text(vm, x=>x.Input)`。
			第二個參數傳綁定來源（通常是本視圖的 Vm），第三個是成員選擇器。
		]
		- #[*禁止* `new Binding("字符串路徑")` 與其他字串路徑綁定：
			非 AOT 安全，也不會在改名時報錯。
		]
		- #[*不需要指定 `BindingMode` 的就不要寫*；
			雙向綁定（輸入框寫回 Vm）必須顯式寫 `BindingMode.TwoWay`。
		]
		- #[綁定源不是本視圖 Vm 時（例如項目模板內），
			把來源對象當第二個參數傳進去即可，不要改用字串路徑。
		]
		- #[編譯期綁定的 setter 支援常見的數值與可空轉換
			（`int`→`double`、`bool`→`bool?` 等），故不要手寫 `(double)` 這種轉型；
			轉到衍生型別的成員（`x => ((Derived)x).Prop`）是允許的。
		]
	]

	#H[事件怎麼寫][
		- #[同步、非耗時的事件處理器直接在 lambda 裏做：
			`b.OnClick(_ => vm.Click1())`；鍵盤用 `t.OnKeyDown(e => {...})`。
		]
		- #[*事件處理器不要寫成 `async void`*。
			需要非同步時呼叫 Vm 的非同步函數，並在 View 內用一個 `Fire(...)`
			這類「啟動但不 await、把例外收斂到狀態列」的輔助方法。
		]
		- #[需要在事件裏吃掉按鍵時，設 `e.Handled = true`。
		]
	]

	#H[樣式怎麼寫][
		- #[樣式放 `BuildStyles()`，回傳 `StyleGroup`（可為 null 表示無樣式）。
		]
		- #[寫法：`new Style<Button>(sel => sel.Class(Cls.MenuBtn)).Property(...)`；
			選取器與 `Style` 相關擴展需要 `using Avalonia.Styling;`，
			`Thickness` 等型別需要 `using Avalonia;`。
		]
		- #[類名一律用 `Cls` 常量（`nameof`），禁止字串硬編碼。
		]
		- #[樣式簡單時直接和控件一起初始化即可；
			只有在需要批量設置或跨控件重複時才抽到 `BuildStyles()`。
		]
	]
]


#H[實測踩過的坑][
	以下都是在真實項目中*實際踩到並修掉*的，不是猜測。

	#H[泛型視圖基類會生成非法代碼（CS0246）][
		寫 `AppViewBase<TCtx> : ViewBase<TCtx>` 後編譯報
		`CS0246: 未能找到類型或命名空間名 TCtx`，
		出錯位置在 `obj/.../AvaloniaPropertyExtensionsGenerator/AppViewBase1Extensions.g.cs`。
		根因：生成器的謂詞是「任何實現 `IDeclarativeViewBase` 的型別（含繼承）」，
		但它*不做型別參數代換*。
		對策：視圖基類保持非泛型，各視圖自己聲明強型別 `Ctx`。
	]

	#H[聲明與實現分離會讓 ViewFactory 註冊生成器撞名（CS8785）][
		症狀：`warning CS8785: 生成器 ViewFactoryRegistrationGenerator 未能生成源 ...
		hintName "...ViewFactoryRegistration.g.cs" 在生成器中必須唯一`。
		根因：該生成器以「類別識別碼」組 hintName，
		而同一類別在兩個檔案各有一個 `class` 宣告，於是同一個 hintName 被加兩次。
		影響：該類別的 `ViewFactory` 註冊產物缺失。
		若項目不經 `ViewFactory` 建立視圖（直接 `new ViewXxx()`），這只是噪音；
		要用 `ViewFactory.Create<T>()` 時需重新評估此寫法。
	]

	#H[partial 兩半的參數特性會被合併（CS0579）][
		例：`[EnumeratorCancellation]` 同時寫在宣告與實現的參數上，
		編譯報 `CS0579: 特性重複`。
		對策：只寫在有方法體的那一半（`.Impl.cs`）。
	]

	#H[partial 方法的參數預設值只認宣告那半（CS1066）][
		`= null` 這種預設值寫在 `.Impl.cs` 會得到
		`CS1066: 為形參指定的預設值將不起任何作用`。
		對策：預設值只寫在 `Xxx.cs`。
	]

	#H[Vm 屬性用裸 `{get;set;}` 會讓綁定靜默失效][
		`ObservableObject` 只在呼叫 `SetProperty` 時才發 `PropertyChanged`。
		裸自動屬性不發通知，於是 Vm→View 的方向永遠不更新。
		更糟的是手動 `OnPropertyChanged` 的屬性仍然正常，
		極易誤判成「綁定全都沒問題」。
		對策：所有會被綁定的屬性都用 `set{ SetProperty(ref field, value); }`。
	]

	#H[`FuncDataTemplate` 會被以 null 呼叫][
		清單有虛擬化時，容器被回收會讓 `ContentPresenter` 的內容清成 null，
		但模板仍會被呼叫，於是 `(T)null` 傳進模板、解參考即崩。
		對策：模板方法（或 lambda）接受可空參數並在 null 時回傳 null。
	]

	#H[常量字串插值只接受「常量字串」][
		`public const str Cols = $"*,{Width}";` 若要成立，`Width` 必須是 `const str`；
		用 `const int` 會報 `CS0133: 指派給 ... 的表達式必須是常量`。
		故欄寬這類常量要宣告為 `const str`。
	]

	#H[Avalonia 12 的 API 變動][
		- `ScrollViewer` 是 `ContentControl`（不再是 `Decorator`）：
			內容用 `.Content(...)`，用 `.Child(...)` 會得到
			`CS0311`（無法轉換成 `Decorator`）。
		- `TextBox.Watermark` 已過時，改用 `PlaceholderText`
			（且它是直接屬性，沒有鏈式方法）。
	]
]


#H[再次強調所該做與不該做][

	#H[所該做][
		- 用 `.Children(...)`／`.Content(...)`／`.Child(...)` 組織控件樹
		- 加入控件樹時、用 `.With(o => ...)` 初始化鏈式方法表達不了的東西
		- 代碼塊的嵌套層級要與實際控件樹結構保持一致
		- 佈局用原生 `Grid` ＋ 星號欄自適應，必要時加 `GridSplitter`（記得設 `ResizeDirection`）
		- 綁定一律用生成的強型別鏈式方法；雙向才補 `BindingMode.TwoWay`
		- View 中的耗時初始化放到 `OnLoaded()`，不要阻塞 UI 創建
		- 樣式有重複時抽到 `BuildStyles()`，並用 `Cls` 常量管理類名
		- UI 文本走 I18n；字體大小、間距等優先走項目配置或統一常量
		- 在 `ViewXxx` 中把關鍵控件提到 public 成員
		- 宣告中的註釋要非常詳細，把具體功能、樣式、交互流程等都寫清楚；
			讓人看到 Decl 就像看到了文檔一樣
	]

	#H[所不該做][
		- 不要建立 `.axaml`、不要在 C\# 裏混寫 XAML
		- 不要用 `new Binding("...")` 或其他字串路徑綁定
		- 不要為了用鏈式方法硬湊：Direct 屬性就用物件初始化器或 `With`
		- 不要在 ViewModel 層做視圖跳轉、操作 View 控件、耦合 View 層細節
		- 不要把耗時操作寫進 View 構造器、同步按鈕事件、或直接阻塞 UI 線程
		- 不要把事件處理器寫成 `async void`
		- 不要硬編碼 UI 文本、字體大小、樣式類名
		- 不要為了「拆函數而拆函數」；只有嵌套過深或子區塊相對獨立時才抽 `MkXxx()`
		- 不要在 `Xxx.cs` 中寫函數實現；聲明與實現必須分離到 `Xxx.Impl.cs`
		- 不要用 `ActivatorUtilities`／`Activator.CreateInstance` 這類反射式激活（AOT 不相容）
		- 不要在基礎設施是否存在、命名是否一致、規範是否適配當前項目這些問題上自行猜測；
			有疑問立即停下來問用戶
	]
]
