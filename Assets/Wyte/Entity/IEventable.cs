
/// <summary>
/// イベントの実行をサポートするインターフェイスです．
/// </summary>
public interface IEventable
{
	EventCondition EventWhen { get; }
	string Label { get; }
}