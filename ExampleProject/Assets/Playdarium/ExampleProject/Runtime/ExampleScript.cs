using UnityEngine;

namespace Playdarium.ExampleProject.Runtime
{
	public class ExampleScript
	{
		public static void Hello() => Debug.Log($"[{nameof(ExampleScript)}] Hello Package");
	}
}