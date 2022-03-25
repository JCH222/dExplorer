namespace dExplorer.Editor.Commons
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UnityEngine.UIElements;

    /// <summary>
    /// Predefined dictionary Visual Element.
    /// </summary>
    /// <typeparam name="T_KEY">Dictionary key type</typeparam>
    /// <typeparam name="T_VALUE">Dictionary value type</typeparam>
    /// <typeparam name="T_VALUE_FIELD">Type of the Visual Element related to the Dictionary value</typeparam>
    public abstract class StaticDictionaryVisualizer<T_KEY, T_VALUE, T_VALUE_FIELD> : VisualElement 
        where T_KEY : IFormattable 
        where T_VALUE_FIELD : VisualElement
    {
        #region Classes
        public new abstract class UxmlTraits : VisualElement.UxmlTraits
        {
            #region Methods
            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext creationContext)
            {
                base.Init(visualElement, bag, creationContext);
                StaticDictionaryVisualizer<T_KEY, T_VALUE, T_VALUE_FIELD> staticDictionaryVisualizer = visualElement as StaticDictionaryVisualizer<T_KEY, T_VALUE, T_VALUE_FIELD>;

                staticDictionaryVisualizer.Clear();

                staticDictionaryVisualizer._keys = new List<T_KEY>();
                staticDictionaryVisualizer._keyNames = new List<string>();
                staticDictionaryVisualizer._values = new List<T_VALUE>();

                static VisualElement MakeItem()
                {
                    Label item = new Label();
                    return item;
                }

                void BindItem(VisualElement visualElement, int index)
                {
                    (visualElement as Label).text = staticDictionaryVisualizer._keyNames[index];
                }

                staticDictionaryVisualizer._keysField = new ListView(staticDictionaryVisualizer._keys, KEY_LIST_ITEM_HEIGHT, MakeItem, BindItem)
                {
                    selectionType = SelectionType.Single
                };
                staticDictionaryVisualizer._keysField.style.minHeight = KEY_LIST_HEIGHT;
                staticDictionaryVisualizer._keysField.style.marginTop = new StyleLength(KEY_LIST_MARGIN_TOP);
                staticDictionaryVisualizer._keysField.onSelectionChange += staticDictionaryVisualizer.OnSelectionChange;

                staticDictionaryVisualizer._valueField = staticDictionaryVisualizer.InitValueField();

                staticDictionaryVisualizer.UpdateValueField();

				staticDictionaryVisualizer.Add(staticDictionaryVisualizer._keysField);
                staticDictionaryVisualizer.Add(staticDictionaryVisualizer._valueField);
            }
            #endregion Methods
        }
        #endregion Classes

        #region Static Fields
        protected readonly static int KEY_LIST_HEIGHT = 100;
        protected readonly static int KEY_LIST_MARGIN_TOP = 8;

        protected readonly static int KEY_LIST_ITEM_HEIGHT = 20;
        #endregion Static Fields

        #region Fields
        protected ListView _keysField;
        protected T_VALUE_FIELD _valueField;
        protected List<T_KEY> _keys;
        protected List<string> _keyNames;
        protected List<T_VALUE> _values;
        #endregion Fields

        #region Accessors
        public T_VALUE this[T_KEY key]
        {
            get
            {
                int index = _keys.IndexOf(key);
                return _values[index];
            }
        }

        public T_VALUE this[T_KEY key, string keyName]
        {
            set
            {
                int index = _keys.IndexOf(key);

                if (index >= 0)
                {
                    _values[index] = value;
                }
                else
                {
                    _keys.Add(key);
                    _keyNames.Add(keyName);
                    _values.Add(value);
                }
            }
        }

		/// <summary>
		/// Check if the dictionary contains the key.
		/// </summary>
		/// <param name="key">Selected key</param>
		/// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(T_KEY key)
		{
            return _keys.Contains(key);

        }
        #endregion Accessors

        #region Methods
        protected abstract T_VALUE_FIELD InitValueField();
        protected abstract void UpdateValueField();
        protected abstract void UpdateValueField(T_KEY key);
        protected abstract void OnSelectionChange(IEnumerable<object> selections);
        #endregion Methods
    }
}
