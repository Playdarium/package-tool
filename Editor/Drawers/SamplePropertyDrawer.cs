using Playdarium.PackageTool.Tools;
using UnityEditor;
using UnityEngine;

namespace Playdarium.PackageTool.Drawers
{
	[CustomPropertyDrawer(typeof(PackageManifestConfig.Sample))]
	public class SamplePropertyDrawer : PropertyDrawer
	{
		private const string DISPLAY_NAME_PROPERTY_NAME = "displayName";
		private const string DESCRIPTION_PROPERTY_NAME = "description";
		private const string PATH_PROPERTY_NAME = "path";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var displayNameRect = new Rect(position)
			{
				height = EditorGUIUtility.singleLineHeight
			};
			displayNameRect.height = EditorConstants.FOLDER_PATH_PICKER_HEIGHT;

			var descriptionRect = new Rect(displayNameRect)
			{
				position = new Vector2(position.x, displayNameRect.y + displayNameRect.height + 2)
			};
			descriptionRect.height = EditorConstants.FOLDER_PATH_PICKER_HEIGHT;

			var pathRect = new Rect(descriptionRect)
			{
				position = new Vector2(position.x, descriptionRect.y + displayNameRect.height + 2),
			};

			pathRect.width -= EditorConstants.FOLDER_PATH_PICKER_BUFFER;
			pathRect.height = EditorConstants.FOLDER_PATH_PICKER_HEIGHT;
			var newPathRect = new Rect(pathRect);

			var pathProperty = property.FindPropertyRelative(PATH_PROPERTY_NAME);
			EditorGUI.PropertyField(displayNameRect, property.FindPropertyRelative(DISPLAY_NAME_PROPERTY_NAME));
			EditorGUI.PropertyField(descriptionRect, property.FindPropertyRelative(DESCRIPTION_PROPERTY_NAME));
			EditorGUI.PropertyField(newPathRect, pathProperty);

			var folderPickerRect = new Rect
			{
				position = new Vector2(
					newPathRect.width + EditorConstants.FOLDER_PATH_PICKER_BUFFER + 10,
					newPathRect.position.y),
				width = EditorConstants.FOLDER_PATH_PICKER_HEIGHT,
				height = EditorConstants.FOLDER_PATH_PICKER_HEIGHT,
			};

			GUILayoutTools.DrawSourceFolderPicker(
				folderPickerRect,
				pathProperty,
				EditorConstants.SELECT_SAMPLES_PATH_FOLDER_TITLE);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 3f;
		}
	}
}