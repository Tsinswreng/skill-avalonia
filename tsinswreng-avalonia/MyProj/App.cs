namespace MyProj;

public partial class App : Application{

	public static IServiceProvider? SvcProvider{get;set;}
	public static partial T DiOrMk<T>() where T : IMk<T>;
	
	/// 初始化全局樣式與主題。取代 XAML 模板中的 <c>App.axaml</c>。
	public override partial void Initialize();
}
