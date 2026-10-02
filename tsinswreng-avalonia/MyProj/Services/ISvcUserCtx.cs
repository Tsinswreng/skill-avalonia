namespace MyProj.Services;

[Doc(@"用戶上下文的端口。
調用方只依賴這個接口，實現由入口層注入，故 Ui 層不認識具體來源。")]
public partial interface ISvcUserCtx{

	[Doc("取當前用戶的上下文。")]
	UserCtx GetUserCtx();
}
