namespace MyProj.Services;

[Doc("用戶上下文。跨層傳遞的資料，故用不可變的 record。")]
public record UserCtx(str Name);
