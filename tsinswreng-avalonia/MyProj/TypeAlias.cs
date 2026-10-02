#if false
<ItemGroup>
	<Compile Include="../TypeAlias.cs" />
</ItemGroup>
#endif

#pragma warning disable CS8981
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
global using str = System.String;
global using obj = System.Object;
global using nil = System.Object;
global using CT = System.Threading.CancellationToken;

// [Doc] 這個註解用的 Attr 由 Tsinswreng.CsCore 提供。
// 全域引入，讓各檔案不必逐一 using，符合「[Doc] 理應全局直接可用」的約定。
global using Tsinswreng.CsCore;

#pragma warning disable CS0436
global using static Tsinswreng.CsTypeAlias.Nil;
namespace Tsinswreng.CsTypeAlias {
	internal class Nil{
		public const nil NIL = null!;
	}
}


//使nil潙空object即可
//蔿防跨項目不一致、勿自定義public class Nil_{ public static Nil_ Nil = null!; }
//#pragma warning restore CS8981
