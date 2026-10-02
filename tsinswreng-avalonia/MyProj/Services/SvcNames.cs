namespace MyProj.Services;

[Doc("名字清單的端口。調用方只依賴這個接口，實現由入口層注入。")]
public interface ISvcNames{
	[Doc(@$"取該用戶的名字清單。
	#Prm[用戶上下文]
	#Prm[]
	#Rtn[名字清單]
	")]
	public Task<IReadOnlyList<str>> GetNames(UserCtx UserCtx, CT Ct);
}

public class SvcNames:ISvcNames{
	[Impl]
	public Task<IReadOnlyList<str>> GetNames(UserCtx UserCtx, CT Ct){
		return Task.FromResult<IReadOnlyList<str>>(["Alice", "Bob", "Carol"]);
	}
}
