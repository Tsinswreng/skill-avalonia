namespace MyProj.Views.Sample;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Styling;
using Avalonia.Threading;
using MyProj.Services;
using Tsinswreng.Avln.Grid;

// 本檔只放函數實現。聲明與 [Doc] 註釋位於 ViewSample.cs。
// 一屏一組 Impl：View 與它的巢狀 Vm 的實現都在本檔，不另外開 ViewSample.Vm.Impl.cs。
// 控件樹與版面都用 Declarative 的擴展方法：.Rows()、.Cols()、.Children()、.With()。
// 佈局容器用 StackGrid（Tsinswreng.Avln.Grid），子項一加進去就自動落號，故不必自己算 Grid_Row、Grid_Column。
public partial class ViewSample{

	// View 只能有無參構造器。這裡解析 Vm 並交給基類的 vm，也就是設置 DataContext；
	// 庫的延遲初始化會在 DataContext 變成相容型別時立刻建立控件樹。
	public partial ViewSample(){
		// 容器有註冊就用容器提供的單例（見 DiSample.SetupSample）；
		// 沒註冊則退回 Mk()（此時沒有依賴，需要服務的操作會被 CheckInit() 擋下）。
		// 一律用 vm 這一個名字，不要有時寫 vm、有時寫 ViewModel。
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
	// 代碼塊的嵌套層級要和實際控件樹結構保持一致：根節點縮進最少、越深的子控件縮進越多。
	// Vm 由泛型基類傳入，型別已確定，故不判空。
	protected override partial object Build(Vm vm){
		// 四列單欄：工具列 / 清單 / 輸入列 / 狀態列。
		Root.Rows("Auto,*,Auto,Auto");

		Root.Children(
			MkToolbar(vm),
			MkList(vm),
			MkInputRow(vm),
			new TextBlock()
				.Text(vm, x=>x.StatusText)
				.Margin(new Thickness(12, 0, 12, 10))
				.With(t => {
					_StatusText = t;
				})
		);

		return Root;
	}

	// 生命週期回調：控件樹與樣式都建好之後由庫觸發，耗時初始化放這裡。
	// 它回傳 void，不能 await，故呼叫 Vm 的同步命令；非同步工作由 Vm 內部的 Fire 啟動。
	protected override partial void OnAfterInitialized(){
		vm.Reload();
	}

	// 抽出的子區塊：只在嵌套過深、或該區塊相對獨立時才抽，不要為了拆函數而拆函數。
	// 關鍵控件在 With(...) 裡提出來，供測試與後續操作使用。
	public partial Control MkToolbar(Vm vm){
		return new StackPanel()
			.Orientation(Orientation.Horizontal)
			.Margin(new Thickness(12))
			.Children(
				new Button()
					// UI 文本走 Todo.I18n，禁止硬編碼。
					.Content(Todo.I18n("重新載入"))
					.With(b => {
						_BtnReload = b;
						b.Classes.Add(Cls.ToolBtn);
						// 事件處理器是 void，不能 await，故呼叫 Vm 的同步命令。
						b.OnClick(_ => vm.Reload());
					}),

				new Button()
					.Content(Todo.I18n("移除選中"))
					.With(b => {
						_BtnRemove = b;
						b.Classes.Add(Cls.ToolBtn);
						b.OnClick(_ => vm.RemoveSelected());
					})
			);
	}

	// 清單：來源與選中項都綁到 Vm。
	public partial Control MkList(Vm vm){
		return new ListBox()
			// 綁定的第二個參數是 Vm 實例、第三個是成員選擇器；這是編譯期綁定，AOT 安全。
			.ItemsSource(vm, x=>x.Names)
			.SelectedItem(vm, x=>x.SelectedName, BindingMode.TwoWay)
			.Margin(new Thickness(12, 0))
			// 項目模板的參數必須可空：虛擬化回收容器時，模板會被以 null 呼叫。
			.ItemTemplate(new FuncDataTemplate<str>((Item, _) =>
				Item is null ? null : new TextBlock().Text(Item)))
			.With(l => {
				_ListNames = l;
			});
	}

	// 輸入列：輸入框與加入按鈕。不需要指定 BindingMode 的綁定就不要寫。
	public partial Control MkInputRow(Vm vm){
		return new StackGrid(IsRow: false)
			.Cols("*,Auto")
			.Margin(new Thickness(12, 10))
			.Children(
				new TextBox()
					// 雙向綁定：輸入框寫回 Vm。
					.Text(vm, x=>x.InputName, BindingMode.TwoWay)
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
					}),

				new Button()
					.Content(Todo.I18n("加入"))
					.Margin(new Thickness(8, 0, 0, 0))
					.With(b => {
						_BtnAdd = b;
						b.Classes.Add(Cls.ToolBtn);
						// 同步、非耗時的事件直接在這裡處理。
						b.OnClick(_ => vm.AddName());
					})
			);
	}

	// ── 巢狀 Vm 的函數實現 ──

	public partial class Vm{

		// 依賴注入用的構造器：設置完依賴後才標記初始化完成。
		public partial Vm(SvcNames SvcNames){
			this.SvcNames = SvcNames;
			base.Init();
		}

		// 沿著 protected 無參構造器建立；此實例沒有依賴，
		// 需要服務的操作會被 CheckInit() 擋下。
		public static partial Vm Mk(){
			return new Vm();
		}

		// 同步、無耗時操作，故直接綁到按鈕與回車。
		public partial void AddName(){
			// step 1: 空輸入時用「未命名」，免得清單出現看不出是什麼的空白項。
			var name = InputName.Trim();
			if(name.Length == 0){
				name = Todo.I18n("未命名");
			}

			// step 2: 改集合與狀態。兩者都用 SetProperty 發通知，故界面會跟著更新。
			Names.Add(name);
			InputName = "";
			StatusText = Todo.I18n($"已加入：{name}（共 {Names.Count} 個）");
		}

		// 同步、無耗時操作。沒有選中項時只提示，不改清單。
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

		// 非同步工作一律由 Fire 啟動：它在失敗時交給 HandleErr。
		// 不可在視圖或生命週期回調裡裸呼叫 Load，那樣例外會成為未被觀察的例外。
		public partial void Reload(){
			Fire(Load(default));
		}

		// 耗時工作放線程池；改動被綁定的集合與屬性前，切回 UI 線程。
		public partial async Task<nil> Load(CT Ct){
			var names = await Task.Run(() => SvcNames.DefaultNames, Ct);

			await Dispatcher.UIThread.InvokeAsync(() => {
				Names.Clear();
				foreach(var one in names){
					Names.Add(one);
				}
				StatusText = Todo.I18n($"已載入 {Names.Count} 個名字");
			});

			return NIL;
		}
	}
}
