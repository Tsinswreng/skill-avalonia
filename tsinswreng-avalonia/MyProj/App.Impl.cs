namespace MyProj;

using Avalonia.Styling;

public partial class App{

	public static partial T DiOrMk<T>() where T : IMk<T>{
		// 容器有註冊就用容器提供的實例（可注入依賴）；
		// 沒註冊才退回 Mk()（此時該實例沒有依賴，需要服務的操作會被 CheckInit() 擋下）。
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
