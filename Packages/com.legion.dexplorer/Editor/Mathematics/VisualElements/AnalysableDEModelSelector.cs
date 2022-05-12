namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Editor.Commons;
	using System;
	using System.Collections.Generic;
	using UnityEngine.UIElements;

    public class AnalysableDEModelSelector : StaticDictionaryVisualizer<Guid, AnalysableDEModel, AnalysableDEModelVisualizer>
    {
        #region Classes
        public new class UxmlFactory : UxmlFactory<AnalysableDEModelSelector, UxmlTraits> { }

        public new class UxmlTraits : StaticDictionaryVisualizer<Guid, AnalysableDEModel, AnalysableDEModelVisualizer>.UxmlTraits
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
                AnalysableDEModelSelector analysableDEModelSelector = visualElement as AnalysableDEModelSelector;
                analysableDEModelSelector._valueFields = new Dictionary<Guid, AnalysableDEModelVisualizer>();

                base.Init(visualElement, bag, creationContext);
                
                Type visualizerBaseType = typeof(AnalysableDEModelVisualizer);
                Type emptyVisualizerType = typeof(EmptyAnalysableDEModelVisualizer);

                foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(visualizerBaseType) && type != emptyVisualizerType)
                        {
                            Guid key = Guid.NewGuid();

                            AnalysableDEModelVisualizer visualizer = Activator.CreateInstance(type) as AnalysableDEModelVisualizer;
                            visualizer.Init();
                            visualizer.SetEnabled(false);
                            visualizer.visible = false;

                            if (analysableDEModelSelector._valueFields.ContainsKey(key))
							{
                                AnalysableDEModel model = analysableDEModelSelector[key];

                                if (model != null)
								{
                                    model.Dispose();
                                }
                            }
                            else
							{
                                analysableDEModelSelector._valueFields.Add(key, visualizer);
                            }
                            
                            analysableDEModelSelector[key, visualizer.GetName()] = visualizer.InstantiateModel();
                        }
                    }
                }
            }
            #endregion Methods
        }
        #endregion Classes

        #region Fields
        private Dictionary<Guid, AnalysableDEModelVisualizer> _valueFields = null;
		#endregion Fields

		#region Accessors
        public AnalysableDEModel SelectedModel
		{
            get { return _valueField.Model; }
		}

        public Button GenerateButton { get; set; } = null;
		#endregion Accessors

        #region Methods
        protected override AnalysableDEModelVisualizer InitValueField()
        {
            EmptyAnalysableDEModelVisualizer visualizer = new EmptyAnalysableDEModelVisualizer();
            visualizer.Init();
            visualizer.SetEnabled(false);
            visualizer.visible = false;

            _valueFields.Add(Guid.Empty, visualizer);

            return _valueFields[Guid.Empty];
        }

        protected override void UpdateValueField()
        {
            UpdateValueField(Guid.Empty);
        }

        protected override void UpdateValueField(Guid key)
        {
            if (ContainsKey(key) == false)
			{
                key = Guid.Empty;
			}

            _valueField.SetEnabled(false);
            _valueField.visible = false;

            if (this.Contains(_valueField))
			{
                Remove(_valueField);
            }

            _valueField = _valueFields[key];
            _valueField.SetEnabled(true);
            _valueField.visible = true;

            Add(_valueField);
        }

        protected override void OnSelectionChange(IEnumerable<object> _)
        {
            GenerateButton.SetEnabled(true);
            GenerateButton.visible = true;
            UpdateValueField(_keys[_keysField.selectedIndex]);
        }

        public IEnumerable<AnalysisProgression> Analyse()
		{
            foreach (AnalysisProgression progression in _valueField.Analyse())
			{
                yield return progression;
			}
        }

        public void Dispose()
		{
            if (_valueFields != null)
			{
                foreach (AnalysableDEModelVisualizer modelVisualizer in _valueFields.Values)
                {
                    if (modelVisualizer != null)
					{
                        modelVisualizer.Dispose();
                    }
                }

                _valueFields.Clear();
            }
		}
        #endregion Methods
    }
}
