// 全域別名與常用 using；與本規範各項目保持同一套習慣，降低跨項目閱讀成本。
global using str = System.String;
global using obj = System.Object;
global using nil = System.Object;
global using CT = System.Threading.CancellationToken;
global using u8 = System.Byte;
global using i8 = System.SByte;
global using u16 = System.UInt16;
global using i16 = System.Int16;
global using u32 = System.UInt32;
global using i32 = System.Int32;
global using u64 = System.UInt64;
global using i64 = System.Int64;
global using f32 = System.Single;
global using f64 = System.Double;
global using static MyProj.Infra.Nil;

namespace MyProj.Infra;

/// <summary>
/// <c>NIL</c> 的來源。
///
/// 為什麼需要它：本規範的非同步方法一律回傳 <c>Task&lt;nil&gt;</c>
/// （<c>nil</c> 是 <c>object</c> 的別名，代表「沒有回傳值」），
/// 方法結尾統一寫 <c>return NIL;</c>。
/// </summary>
public static class Nil{

	/// <summary>「沒有值」的單例。刻意為非 null：非同步方法宣告為 <c>Task&lt;nil&gt;</c>，
	/// 回傳可空值會得到 CS8603。</summary>
	public static readonly obj NIL = new();
}
