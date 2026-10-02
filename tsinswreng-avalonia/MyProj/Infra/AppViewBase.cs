namespace MyProj.Infra;

using Avalonia.Markup.Declarative;

/// <summary>
/// 所有視圖的基類：在 Declarative 的 <see cref="ViewBase{TViewModel}"/> 之上，
/// 補一個「控件樹與樣式都建好之後」的回調，並提供強型別的 Vm 訪問器。
///
/// 用了庫的泛型基類之後，子類的 <c>Build</c> 直接拿到強型別的 Vm
/// （<c>protected override object Build(Vm vm)</c>），
/// 因此*不必*自己宣告 <c>Ctx</c> 屬性、也不必在 <c>Build</c> 裡判空或 throw：
/// 真的缺 Vm 時，庫的 <c>ViewBase&lt;TVm&gt;</c> 會在建立控件樹之前拋出
/// 「Cannot build view Xxx without a valid ViewModel of type VmXxx」。
///
/// 命名例外：本訪問器叫 <c>vm</c>（小駝峰）。因為常態寫法是「Vm 是 View 的巢狀型別」，
/// <c>Vm</c> 這個識別符已被型別佔用；同名會讓成員查找指向型別而不是實例。
/// 這是本規範唯一的小駝峰成員。
///
/// *為何這個訪問器是 protected 且只讀* —— 這是本類別能被源生成器放過的關鍵：
/// Declarative 的 <c>AvaloniaPropertyExtensionsGenerator</c> 會為「實作 IDeclarativeViewBase 的類別」
/// 生成鏈式擴展方法；對「public 且有 public setter 的普通屬性」，它生成的簽名是
/// <c>public static {含型別參數的類別全名} Xxx(this ...)</c>，
/// 但那個方法自己不宣告類別的型別參數 → 型別參數在生成檔裡無處可尋 → CS0246。
/// 只讀（或非 public）的屬性不在它的處理範圍內，本類別因此不會產生任何生成檔。
/// 已實測：在同一個類別上加一個 <c>public TVm ProbeCtx{ get; set; }</c> 就會重現 CS0246。
/// </summary>
/// <typeparam name="TVm">本視圖使用的 ViewModel 型別。</typeparam>
public abstract partial class AppViewBase<TVm> : ViewBase<TVm> where TVm : class{

	/// <summary>
	/// 本視圖的 ViewModel。由子類的建構器設好（見 <c>ViewModel</c>）後就不會再變，故不判空。
	/// 要從外部指派時用庫提供的 <c>ViewModel</c>。
	/// </summary>
	protected TVm vm{
		get{
			return ViewModel!;
		}
	}

	/// <summary>
	/// Declarative 在「樣式與控件樹都建好」之後的回調；這裡轉發給 <see cref="OnLoaded"/>。
	/// 子類不應覆寫本方法，以免繞過約定。
	/// </summary>
	protected override partial void OnAfterInitialized();

	/// <summary>
	/// 子類可選實現：控件樹已就緒之後的初始化。
	/// **耗時操作必須放這裡**，不能放建構器或 <c>Build</c>，否則會阻塞界面建立。
	/// </summary>
	protected virtual partial void OnLoaded();
}
