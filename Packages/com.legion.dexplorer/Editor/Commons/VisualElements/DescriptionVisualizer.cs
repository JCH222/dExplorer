namespace dExplorer.Editor.Commons
{
    using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UnityEditor.UIElements;
	using UnityEngine.UIElements;

	public class DescriptionVisualizer : VisualElement
    {
        #region Enums
        [Flags]
        public enum DescriptionItem
        {
            SHORT_DESCRIPTION = (1 << 0),
            LONG_DESCRIPTION = (1 << 1)
        }
        #endregion Enums

        #region Classes
        public new class UxmlFactory : UxmlFactory<DescriptionVisualizer, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            #region Fields
            private UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription { name = "Title", defaultValue = string.Empty };
            #endregion Fields

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
                DescriptionVisualizer descriptionVisualizer = visualElement as DescriptionVisualizer;

                descriptionVisualizer.Clear();

                descriptionVisualizer._title = m_Title.GetValueFromBag(bag, creationContext);
                descriptionVisualizer._shortDescription = string.Empty;
                descriptionVisualizer._longDescription = string.Empty;

                descriptionVisualizer._header = new EnumFlagsField();
                descriptionVisualizer._header.Init(DescriptionItem.SHORT_DESCRIPTION | DescriptionItem.LONG_DESCRIPTION);
                descriptionVisualizer._header.RegisterValueChangedCallback(descriptionVisualizer.OnConfigurationChanged);

                descriptionVisualizer._itemsGroup = new VisualElement();
                
                descriptionVisualizer._longDescriptionScrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
                descriptionVisualizer._longDescriptionScrollView.style.maxHeight = descriptionVisualizer.LONG_DESCRIPTION_HEIGHT;

                descriptionVisualizer._shortDescriptionField = new TextField
                {
                    label = string.Empty,
					multiline = false
				};

				descriptionVisualizer._longDescriptionField = new TextField
				{
                    label = string.Empty,
                    multiline = true
				};
                descriptionVisualizer._longDescriptionField.style.minHeight = descriptionVisualizer.LONG_DESCRIPTION_HEIGHT;

                descriptionVisualizer.Add(descriptionVisualizer._header);
                descriptionVisualizer.Add(descriptionVisualizer._itemsGroup);
                descriptionVisualizer._longDescriptionScrollView.Add(descriptionVisualizer._longDescriptionField);

                descriptionVisualizer.Update();

                descriptionVisualizer.OnConfigurationChanged(null);
            }
            #endregion Methods
        }
        #endregion Classes

        #region Static Fields
        private readonly int LONG_DESCRIPTION_HEIGHT = 100;
		#endregion Static Fields

		#region Fields
		private EnumFlagsField _header;
        private VisualElement _itemsGroup;
        private TextField _shortDescriptionField;
        private ScrollView _longDescriptionScrollView;
        private TextField _longDescriptionField;

        private string _title;
        private string _shortDescription;
        private string _longDescription;
        #endregion Fields

        #region Accessors
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Update();
            }
        }

        public string ShortDescription
        {
            get
            {
                return _shortDescription;
            }
            set
            {
                _shortDescription = value;
                Update();
            }
        }

        public string LongDescription
        {
            get
            {
                return _longDescription;
            }
            set
            {
                _longDescription = value;
                Update();
            }
        }
        #endregion Accessors

        #region Methods
        private void OnConfigurationChanged(ChangeEvent<Enum> _)
        {
            _itemsGroup.Clear();

            if (_header.value.HasFlag(DescriptionItem.SHORT_DESCRIPTION))
            {
                _itemsGroup.Add(_shortDescriptionField);
            }
            if (_header.value.HasFlag(DescriptionItem.LONG_DESCRIPTION))
            {
                _itemsGroup.Add(_longDescriptionScrollView);
            }
        }

        private void Update()
        {
            _header.label = _title;
            _shortDescriptionField.value = _shortDescription;
            _longDescriptionField.value = _longDescription;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterShortDescriptionValueChangedCallback(EventCallback<ChangeEvent<string>> callback)
		{
            _shortDescriptionField.RegisterValueChangedCallback(callback);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterLongDescriptionValueChangedCallback(EventCallback<ChangeEvent<string>> callback)
        {
            _longDescriptionField.RegisterValueChangedCallback(callback);
        }
        #endregion Methods
    }
}
