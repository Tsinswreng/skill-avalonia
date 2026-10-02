namespace MyProj.Di;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 本層的依賴注入註冊入口。
///
/// 為何註冊集中在這裡、而不是散在入口層各寫一遍：
/// 註冊內容會隨業務成長（日後還有快取、索引、資料庫），
/// 抄成多份就會出現「新增服務時漏改一處」的執行期故障。
/// 入口層只需呼叫 <c>services.SetupSample()</c>。
/// </summary>
public static partial class DiSample{

	/// <summary>
	/// 註冊本示例用到的服務與 ViewModel。
	/// </summary>
	/// <param name="Svc">要註冊到的服務集合。</param>
	/// <returns>同一個 <paramref name="Svc"/>，方便串接其他 <c>Setup*</c>。</returns>
	public static partial IServiceCollection SetupSample(this IServiceCollection Svc);
}
