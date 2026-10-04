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
