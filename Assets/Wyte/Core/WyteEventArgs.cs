using System;

public class WyteEventArgs : EventArgs
{
	/// <summary>
	/// このイベントをキャンセルするかどうか取得します．
	/// </summary>
	/// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
	public bool Cancel { get; set; }
}