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



#H[View 與 Vm 規範][
	示例代碼：

	`VmSample.cs`（Vm 聲明）:
	#raw(
		read("MyProj/Views/Sample/VmSample.cs"),
		block:true,
		lang: "cs",
	)

	`VmSample.Impl.cs`（Vm 實現）:
	#raw(
		read("MyProj/Views/Sample/VmSample.Impl.cs"),
		block:true,
		lang: "cs",
	)

	`ViewSample.cs`（View 聲明）:
	#raw(
		read("MyProj/Views/Sample/ViewSample.cs"),
		block:true,
		lang: "cs",
		
	)

	`ViewSample.Impl.cs`（View 實現）:
	#raw(
		read("MyProj/Views/Sample/ViewSample.Impl.cs"),
		block:true,
		lang: "cs",
	)
	// 注:
	// - `AppViewBase<TVm>`、`ViewModelBase`、`IMk<T>`、`App.DiOrMk<T>()`、`Todo.I18n()`、
	// 	`Init`、`CheckInit`、`Fire`、`HandleErr` 定義在 `ViewModelBase` 上。
	// 	若項目未定義則應請示用戶。
	若有缺少的 沒定義的符號(如`AppViewBase<>`, `Todo.I18n()`等)則應停下來請示用戶,
	禁止擅自行動
]

#H[再次強調][
	#H[注意事項][
		- 所有代碼要兼容AOT
		- 耗時/異步初始化不得阻塞UI創建
		- 臨時的UI文本用`Todo.I18n`
		- 字體大小等共用樣式優先從項目統一配置調用, 不要到處硬編碼
		- 如果項目缺少本 skill 依賴的基礎設施，如 `IMk<>`、`Todo.I18n()`、綁定輔助器、View/Vm 基類，立即請示用戶
		- 在ViewXxx中把關鍵控件提到public成員。
		- 聲明中的註釋要非常詳細、把具體功能樣式交互流程等都寫清楚。讓人看到Decl就像看到了文檔一樣。
		- 多用`StackGrid`代替`Grid`
	]
	
	#H[禁止事項][
		- 不要用 `new Binding("...")` 或其他字符串路徑綁定。因 這是不兼容AOT的
		- 不要在 Vm 層做視圖跳轉、操作 View 控件、耦合 View 層細節
		- 不要把耗時操作寫進 View 構造器、同步按鈕事件、或直接阻塞 UI 線程
		- 不要硬編碼 UI 文本、字體大小、樣式類名
		- 不要爲了“拆函數而拆函數”；只有在 Build 嵌套過深或子區塊相對獨立時才抽 `MkXxx()`
		- 不要手動給`StackGrid`設置行號和列號, 應讓他自動分配
	]
]
