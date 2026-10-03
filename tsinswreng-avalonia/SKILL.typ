#import "@preview/tsinswreng-auto-heading:0.1.0": auto-heading
#let H = auto-heading;
\-\-\-

name: tsinswreng-avalonia

description: Avalonia 項目開發規範。

\-\-\-


#H[總則][
	- UI 一律用 `Avalonia.Markup.Declarative` 以*純 C\# 聲明式*寫：
		不建立 `.axaml`、不寫 XAML。
	- 綁定一律用生成的強型別鏈式方法，禁止 `new Binding("字串路徑")` 這類寫法。
]


#H[命名規範][
	- 視圖(View)用 View前綴
	- 視圖模型(ViewModel) 用Vm前綴

]


#H[文件位置][
例:
````
MyProj/Views/
	Login/
		ViewLogin.cs
		ViewLogin.Impl.cs
		VmLogin.cs
		VmLogin.Impl.cs
	UserProfile/
		ViewUserProfile.cs
		ViewUserProfile.Impl.cs
		VmUserProfile.cs
		VmUserProfile.Impl.cs
````

- ViewXxx和VmXxx須同時放在名为Xxx的文件夾下
- 遵守 《聲明與實現分離》的規範、類型和所有函數都聲明爲partial、`Xxx.cs`中不寫函數實現、函數實現都寫在`Xxx.Impl.cs`中

]

#H[作爲內部類的Vm][
	大多數時候View和Vm都是一一對應的。
	在這時候就沒必要把Vm單獨抽出一個文件來了,
	而是把Vm作爲內部類定義在View類的裏面,
	類名直接叫`Vm`,
	不會衝突。
	合起來寫旹就只有一個`ViewXxx.cs`和一個`ViewXxx.Impl.cs`
	
	如果不是View和Vm一一對應的情況,
	就ViewXxx和VmXxx分開寫。

]


#H[View 與 Vm 規範][
	示例代碼：

	`ViewSample.cs`（聲明）:
	#raw(
		read("MyProj/Views/Sample/ViewSample.cs"),
		block:true,
		lang: "cs",
		
	)

	`ViewSample.Impl.cs`（實現）:
	#raw(
		read("MyProj/Views/Sample/ViewSample.Impl.cs"),
		block:true,
		lang: "cs",
	)

	注:
	- `AppViewBase<TVm>`、`ViewModelBase`、`IMk<T>`、`App.DiOrMk<T>()`、`Todo.I18n()`、
		`StackGrid`（`Tsinswreng.Avln.Grid`，本工作區的本地專案）、
		`IValConvtrWithErr` 與 `FnConvtr`（`Tsinswreng.Avln.Dsl`，只借轉值器）、
		`[Doc]` 與全域別名
		（`str`、`obj`、`nil`、`NIL`、`CT`）由項目提供，
		`Init`、`CheckInit`、`Fire`、`HandleErr` 定義在 `ViewModelBase` 上。
		若項目未定義則應請示用戶。
]
