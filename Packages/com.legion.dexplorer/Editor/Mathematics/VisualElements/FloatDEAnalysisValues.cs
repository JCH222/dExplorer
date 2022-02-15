namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Commons;
	using dExplorer.Runtime.Mathematics;
	using System.Collections.Generic;
    using UnityEngine.UIElements;

    public class FloatDEAnalysisValues : StaticDictionaryVisualizer<float, Dictionary<DESolvingType, float>, TextField>
    {
        #region Classes
        public new class UxmlFactory : UxmlFactory<FloatDEAnalysisValues, UxmlTraits> { }

        public new class UxmlTraits : StaticDictionaryVisualizer<float, Dictionary<DESolvingType, float>, TextField>.UxmlTraits
        {
            #region Accessors
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            #endregion Accessors

            #region Methods
            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext creationContext)
            {
                base.Init(visualElement, bag, creationContext);
            }
            #endregion Methods
        }
        #endregion Classes

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
                Dictionary<DESolvingType, float> value = this[key];

                foreach (KeyValuePair<DESolvingType, float> pair in value)
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
