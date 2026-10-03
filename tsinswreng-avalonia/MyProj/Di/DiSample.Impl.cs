namespace MyProj.Di;

using MyProj.Services;
using MyProj.Views.Sample;
using Microsoft.Extensions.DependencyInjection;

public static partial class DiSample{

	public static partial IServiceCollection SetupSample(this IServiceCollection Svc){
		// step 1: 本示例的資料來源服務。
		//         真實項目中它應該是一個端口（interface）＋ 各平臺實現；
		//         本示例為了精簡只有一個具體類。
		Svc.AddSingleton<SvcNames>();

		// step 2: 用戶上下文的端口與實現。
		//         調用方只依賴端口，換來源時不必動 Ui 層。
		Svc.AddSingleton<ISvcUserCtx, SvcUserCtx>();

		// step 3: ViewModel 一律註冊為單例：視圖可能被重建，而狀態應在重建後保留。
		//         這裡用工廠 lambda 而不是 ActivatorUtilities：後者是反射式的，
		//         需要運行期保留建構器資訊，與 AOT 要求相衝，且依賴關係會變得隱晦。
		Svc.AddSingleton<ViewSample.Vm>(sp => new ViewSample.Vm(
			sp.GetRequiredService<ISvcUserCtx>(),
			sp.GetRequiredService<SvcNames>()));

		return Svc;
	}
}
