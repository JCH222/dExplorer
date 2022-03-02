namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Commons;
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
	using UnityEngine.UIElements;

	public abstract class DEAnalysisValues<T_VARIABLE> : StaticDictionaryVisualizer<float, Dictionary<DESolvingType, T_VARIABLE>, TextField>
		where T_VARIABLE : struct
	{
		#region Methods
		protected override TextField InitValueField()
		{
			TextField textField = new TextField
			{
				isReadOnly = true,
				multiline = true
			};

			return textField;
		}

		protected override void UpdateValueField()
		{
			UpdateValueField(float.NaN);
		}

		protected override void UpdateValueField(float key)
		{
			string text = "Mean Absolute Errors : \n";

			if (ContainsKey(key) == true)
			{
				Dictionary<DESolvingType, T_VARIABLE> value = this[key];

				foreach (KeyValuePair<DESolvingType, T_VARIABLE> pair in value)
				{
					text += string.Format("\n{0}", pair.Key);
					text += string.Format("\n-> {0}", pair.Value);
					text += "\n";
				}
			}

			_valueField.value = text;
		}

		protected override void OnSelectionChange(IEnumerable<object> _)
		{
			UpdateValueField(_keys[_keysField.selectedIndex]);
		}
		#endregion Methods
	}
}
