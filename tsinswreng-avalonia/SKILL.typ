#import "@preview/tsinswreng-auto-heading:0.1.0": auto-heading
#let H = auto-heading;
\-\-\-

name: tsinswreng-avalonia

description: Avalonia 項目開發規範。UI 一律用 Avalonia.Markup.Declarative 以純 C\# 聲明式寫，不用 XAML。

\-\-\-


#H[總則][
	- UI 一律用 `Avalonia.Markup.Declarative` 以*純 C\# 聲明式*寫：
		不建立 `.axaml`、不寫 XAML。
	- 綁定一律用生成的強型別鏈式方法，禁止 `new Binding("字串路徑")` 這類寫法。
]


#H[命名與文件位置][
	#H[命名規範][
		- 視圖(View) 用 `View` 前綴，例如 `ViewSample`。
		- 視圖模型(ViewModel) 與 View 一一對應時巢狀在該 View 裡，型別名就叫 `Vm`；
			一個 Vm 給多個 View 共用、或被非 UI 層使用時，才拆成頂層的 `VmXxx`。
		- 關鍵控件成員用下劃線開頭，例如 `_BtnAdd`。
		- 樣式類名走 `Cls` 常量，值用 `nameof`。
		- 抽出的子區塊方法用 `Mk` 前綴並回傳控件，例如 `MkList`。
	]

	#H[文件位置][
		一屏兩檔，View 與它的巢狀 Vm 共用同一組檔案：
		````
		MyProj/Views/Sample/
			ViewSample.cs        聲明：View 的成員 ＋ 巢狀 Vm 的成員
			ViewSample.Impl.cs   實現：View 的函數 ＋ 巢狀 Vm 的函數
		````
		- 不要另開 `ViewSample.Vm.Impl.cs`。
		- 類型與所有函數都聲明成 `partial`：聲明檔只寫聲明，函數體寫在 `.Impl.cs`。
		- 巢狀 Vm 的型別在 View 的基底子句裡還不可見，故用別名引入：
			`using Vm = ViewSample.Vm;`，之後寫 `AppViewBase<Vm>`。
	]
]


#H[View 與 Vm 規範][
	示例代碼：

	`ViewSample.cs`（聲明）:
	#raw(read("MyProj/Views/Sample/ViewSample.cs"), lang: "cs")

	`ViewSample.Impl.cs`（實現）:
	#raw(read("MyProj/Views/Sample/ViewSample.Impl.cs"), lang: "cs")

	注:
	- `AppViewBase<TVm>`、`ViewModelBase`、`IMk<T>`、`App.DiOrMk<T>()`、`Todo.I18n()`、
		`AutoGrid`（自動落號的 Grid 子類）、`[Doc]` 與全域別名
		（`str`、`obj`、`nil`、`NIL`、`CT`）由項目提供，
		`Init`、`CheckInit`、`Fire`、`HandleErr` 定義在 `ViewModelBase` 上。
		若項目未定義則應請示用戶。
]
