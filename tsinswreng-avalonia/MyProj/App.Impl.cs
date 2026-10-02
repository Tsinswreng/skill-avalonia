namespace MyProj;

using Avalonia.Styling;

public partial class App{

	public static partial T DiOrMk<T>() where T : IMk<T>{
		if(SvcProvider?.GetService(typeof(T)) is T FromContainer){
			return FromContainer;
		}
		return T.Mk();
	}

	public override partial void Initialize(){
		Styles.Add(new FluentTheme());
		// 本規範的項目一律深色主題；要換主題只改這一處。
		RequestedThemeVariant = ThemeVariant.Dark;
	}
}
