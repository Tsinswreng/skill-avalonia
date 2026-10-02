namespace MyProj.Services;

/// <summary>
/// 示例用的資料來源。
///
/// 真實項目中，這一層應該是「端口（interface）＋ 各平臺／各來源實現」的形式；
/// 本示例為了精簡只用一個具體類，而且它沒有方法（故不需要聲明與實現分離）。
/// </summary>
public class SvcNames{

	/// <summary>初始名字清單。</summary>
	public IReadOnlyList<str> DefaultNames{get;} = ["Alice", "Bob", "Carol"];
}
