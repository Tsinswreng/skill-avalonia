namespace MyProj.Services;

public partial class SvcUserCtx{

	public partial SvcUserCtx(){
	}

	public partial UserCtx GetUserCtx(){
		return new UserCtx("示例用戶");
	}
}
