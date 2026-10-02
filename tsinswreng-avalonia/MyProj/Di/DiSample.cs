namespace MyProj.Di;

using Microsoft.Extensions.DependencyInjection;

[Doc(@"本層的依賴注入註冊入口。
為何註冊集中在這裡、而不是散在入口層各寫一遍：
註冊內容會隨業務成長（日後還有快取、索引、資料庫），
抄成多份就會出現「新增服務時漏改一處」的執行期故障。
入口層只需呼叫 services.SetupSample()。
")]
public static partial class DiSample{

	[Doc(@"註冊本示例用到的服務與 ViewModel。
#Prm[要註冊到的服務集合]
#Rtn[同一個服務集合，方便串接其他 Setup 方法]
")]
	public static partial IServiceCollection SetupSample(this IServiceCollection Svc);
}
