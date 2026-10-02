namespace MyProj.Services;

[Doc(@"用戶上下文來源的示例實現。
真實項目中由入口層按平臺注入不同實現，本示例固定回一個用戶。")]
public partial class SvcUserCtx : ISvcUserCtx{

	[Doc("無參構造器。本示例沒有依賴。")]
	public partial SvcUserCtx();

	[Doc("取當前用戶的上下文。")]
	public partial UserCtx GetUserCtx();
}
