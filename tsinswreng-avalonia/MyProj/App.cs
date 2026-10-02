namespace MyProj;

using Avalonia.Styling;
using MyProj.Infra;

/// <summary>
/// 應用實例。
///
/// 這裡放兩樣東西：
/// 一是 Avalonia 的生命週期入口（<see cref="Initialize"/>），
/// 二是服務解析入口（<see cref="Services"/> ＋ <see cref="DiOrMk{T}"/>）——
/// 視圖在建立自己的 ViewModel 時會用到後者。
/// </summary>
public partial class App : Application{

	/// <summary>
	/// 服務容器。由入口層（<c>Program.cs</c>）在啟動時設置。
	/// 為 null 時 <see cref="DiOrMk{T}"/> 會退回用該型別自己的 <c>Mk()</c>。
	/// </summary>
	public static IServiceProvider? Services{get;set;}

	/// <summary>
	/// 服務解析入口：先問容器，容器沒有則用該型別自己的 <c>Mk()</c>。
	///
	/// 為什麼不用 <c>ActivatorUtilities</c> 之類的反射式建立：
	/// 本規範要求 AOT 相容，而基於反射的激活需要運行期保留建構器資訊，
	/// 既是裁剪風險、也讓依賴關係變得隱晦。
	/// 這裡的兩個來源都是編譯期可見的，AOT 安全。
	/// </summary>
	/// <typeparam name="T">要解析的型別，須實現 <see cref="IMk{T}"/>。</typeparam>
	public static partial T DiOrMk<T>() where T : IMk<T>;

	/// <summary>
	/// 初始化全局樣式與主題。取代 XAML 模板中的 <c>App.axaml</c>。
	/// </summary>
	public override partial void Initialize();
}
