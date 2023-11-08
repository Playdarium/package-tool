using Playdarium.ExampleProject.Runtime;
using UnityEditor;

namespace Playdarium.ExampleProject
{
	public class ExampleScriptEditor
	{
		[MenuItem("Tools/ExampleProject/Hello")]
		public static void Hello() => ExampleScript.Hello();
	}
}