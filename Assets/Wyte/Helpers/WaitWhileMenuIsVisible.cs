using UnityEngine;
using WyteEngine.UI;

namespace WyteEngine.Helper
{
	public class WaitWhileMenuIsVisible : CustomYieldInstruction
	{
		public override bool keepWaiting => ConfigController.Instance.IsVisible;
	}
}