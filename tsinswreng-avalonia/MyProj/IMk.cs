namespace MyProj;

/// <summary>
/// 「可由專案自身直接建立」的型別約定。
///
/// 用途有二：
/// <list type="bullet">
/// <item>依賴注入容器沒有註冊該型別時的兜底建立方式（見 <c>App.DiOrMk&lt;T&gt;()</c>）。</item>
/// <item>單元測試可以不搭容器、直接 <c>T.Mk()</c> 造出物件。</item>
/// </list>
/// 實現者必須提供 <c>Mk()</c>，且該方法建立的實例**不得**依賴注入的服務；
/// 若某個型別離開服務就無法工作，就不要實現本介面——這樣容器缺註冊時會直接報錯，
/// 而不是造出半殘的物件在運行期才炸。
/// </summary>
public interface IMk<T>{
	/// <summary>建立一個不注入任何依賴的實例。</summary>
	public static abstract T Mk();
}
