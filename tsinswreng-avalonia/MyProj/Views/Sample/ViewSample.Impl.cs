namespace MyProj.Views.Sample;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Styling;
using MyProj.Infra;

public partial class ViewSample{

	public partial ViewSample(){
		// 容器有註冊就用容器提供的單例（見 DiSample.SetupSample）；
		// 沒註冊則退回 Mk()（此時沒有依賴，需要服務的操作會被 CheckInit() 擋下）。
		// 設置 DataContext（= 基類的 ViewModel）會觸發庫的延遲初始化 → 立刻呼叫 Build(vm)。
		ViewModel = App.DiOrMk<Vm>();
	}

	protected override partial StyleGroup? BuildStyles(){
		return [
			// 工具列按鈕：統一樣式，故抽到這裡、並用 Cls 常量管理類名。
			new Style<Button>(sel => sel.Class(Cls.ToolBtn))
				.Margin(new Thickness(0, 0, 8, 0))
				.MinWidth(96),
		];
	}

	protected override partial object Build(Vm vm){
		// 四列單欄：工具列 / 清單 / 輸入列 / 狀態列。
		// 代碼塊的嵌套層級要和實際控件樹結構保持一致。
		return new Grid()
			.Rows("Auto,*,Auto,Auto")
			.Children(
				MkToolbar(vm).Grid_Row(0),
				MkList(vm).Grid_Row(1),
				MkInputRow(vm).Grid_Row(2),

				new TextBlock()
					.Text(vm, x=>x.StatusText)
					.Margin(new Thickness(12, 0, 12, 10))
					.Grid_Row(3)
					.With(t => {
						_StatusText = t;
					})
			);
	}

	protected override partial void OnAfterInitialized(){
		Fire(vm, vm.Load(CancellationToken.None));
	}

	public partial Control MkToolbar(Vm vm){
		return new StackPanel()
			.Orientation(Orientation.Horizontal)
			.Margin(new Thickness(12))
			.Children(
				new Button()
					.Content(Todo.I18n("重新載入"))
					.With(b => {
						_BtnReload = b;
						b.Classes.Add(Cls.ToolBtn);
						// 非同步操作用 Fire 收斂例外；不要寫成 async void。
						b.OnClick(_ => Fire(vm, vm.Load(CancellationToken.None)));
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

	public partial Control MkList(Vm vm){
		return new ListBox()
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

	public partial Control MkInputRow(Vm vm){
		return new Grid()
			.Cols("*,Auto")
			.Margin(new Thickness(12, 10))
			.Children(
				new TextBox()
					// 雙向綁定：輸入框寫回 Vm
					.Text(vm, x=>x.InputName, BindingMode.TwoWay)
					.Grid_Column(0)
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
					.Grid_Column(1)
					.Margin(new Thickness(8, 0, 0, 0))
					.With(b => {
						_BtnAdd = b;
						b.Classes.Add(Cls.ToolBtn);
						b.OnClick(_ => vm.AddName());
					})
			);
	}

	private partial void Fire(Vm vm, Task<nil> Op){
		// 刻意不 await：生命週期回調與事件處理器不能阻塞。
		_ = FireCore(vm, Op);
	}

	private partial async Task<nil> FireCore(Vm vm, Task<nil> Op){
		try{
			await Op;
		}catch(Exception ex){
			vm.StatusText = Todo.I18n("操作失敗： "+ex.Message);
		}
		return NIL;
	}
}
