namespace MyProj.Infra;

public static partial class Todo{

	public static partial str I18n(str Text){
		return Text;
	}

	public static partial str I18n(str Format, params obj[] Args){
		return str.Format(Format, Args);
	}
}
