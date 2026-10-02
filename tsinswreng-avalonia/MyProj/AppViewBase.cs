namespace MyProj.Infra;

using Avalonia.Markup.Declarative;

public abstract partial class AppViewBase<TVm> : ViewBase<TVm> where TVm : class{

	
	/// 不能用public get; set; 否則源生成器生成之代碼會出錯
	protected TVm vm{
		get{return ViewModel!;}
		set{ViewModel = value;}
	}
}
