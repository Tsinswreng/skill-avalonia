namespace MyProj.Infra;

public partial class AppVmBase{

	public partial void Init(){
		IsInited = true;
	}

	public partial void CheckInit(){
		if(!IsInited){
			throw new InvalidOperationException(
				$"{GetType().Name} 尚未初始化：請用依賴注入建立它，而不是用 Mk()。");
		}
	}

	protected async partial void Fire(Task<nil> Op){
		try{
			await Op;
		}catch(Exception Ex){
			// HandleErr 自己不得拋例外，否則例外會從這個 async void 逸出。
			HandleErr(Ex);
		}
	}

	protected partial void HandleErr(Exception Ex){
		// 實作待定。
	}
}
