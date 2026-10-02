global using static MyProj.GlobalStatic;
using Avalonia.Threading;

namespace MyProj;

public class GlobalStatic{
	public void RunOnUi(Action A){
		Dispatcher.UIThread.Post(A);
	}
}
