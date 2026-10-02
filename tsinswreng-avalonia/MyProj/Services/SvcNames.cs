namespace MyProj.Services;

public interface ISvcNames{
	[Doc("初始名字清單。")]
	public IReadOnlyList<str> DefaultNames{get;}
}

public class SvcNames:ISvcNames{
	[Impl]
	public IReadOnlyList<str> DefaultNames{get;} = ["Alice", "Bob", "Carol"];
}

