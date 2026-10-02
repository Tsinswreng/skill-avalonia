namespace MyProj.Infra;

/// <summary>
/// 「已知要向後補、但當前版本先佔位」的基礎設施。
///
/// 目前只有多語言本地化一項。之所以現在就建這個入口而不是先硬編碼字串：
/// 一旦字串散落在各視圖裡，日後接入 I18n 就得回頭改幾十處；
/// 全部走這裡之後，屆時只改本類的實現即可。
/// </summary>
public static partial class Todo{

	/// <summary>
	/// 本地化的臨時替身：**當前原樣返回輸入文字**（即尚未真正翻譯）。
	/// 待接入正式 I18n 後，把實現改為查表；調用方（所有視圖）不需要任何改動。
	/// </summary>
	/// <param name="Text">原文。</param>
	public static partial str I18n(str Text);

	/// <summary>
	/// 帶參數的本地化佔位：用 <c>{0}</c>、<c>{1}</c> 作佔位符。
	/// </summary>
	/// <param name="Format">含佔位符的原文。</param>
	/// <param name="Args">替換參數。</param>
	public static partial str I18n(str Format, params obj[] Args);
}
