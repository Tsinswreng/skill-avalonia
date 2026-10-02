namespace MyProj.Infra;

public partial class ViewModelBase{

	public partial void Init(){
		IsInited = true;
	}

	public partial void CheckInit(){
		if(!IsInited){
			throw new InvalidOperationException(
				$"{GetType().Name} 尚未初始化：請用依賴注入建立它，而不是用 Mk()。");
		}
	}
}
