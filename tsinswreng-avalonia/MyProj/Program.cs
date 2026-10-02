// 入口層：建立服務容器、建立生命週期、掛上唯一的 Window。
//
// 為什麼不用模板預設的 UseServiceProvider ＋ UseComponentControlFactory：
// 那一套靠 ActivatorUtilities（反射）建立視圖，與本規範的 AOT 要求相衝；
// 本規範的視圖自己用 App.DiOrMk<T>() 解析 Vm，直接 new 即可。
using MyProj;
using MyProj.Di;
using MyProj.Infra;
using MyProj.Views.Sample;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.SetupSample();
App.Services = services.BuildServiceProvider();

var lifetime = new ClassicDesktopStyleApplicationLifetime{
	Args = args,
	ShutdownMode = ShutdownMode.OnLastWindowClose,
};

AppBuilder.Configure<App>()
	.UsePlatformDetect()
	.SetupWithLifetime(lifetime);

lifetime.MainWindow = new Window{
	Title = Todo.I18n("Avalonia 規範示例"),
	Width = 900,
	Height = 620,
	Content = new ViewSample(),
};

lifetime.Start(args);
