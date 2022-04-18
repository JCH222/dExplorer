namespace dExplorer.Editor.Mathematics
{
	using dExplorer.Runtime.Mathematics;
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Unity.Mathematics;
	using UnityEditor;
	using UnityEditor.UIElements;
	using UnityEngine;
	using UnityEngine.UIElements;

    [Flags]
    public enum SelectableDESolvingTypes
	{
        EXPLICIT_EULER = 1,
        EXPLICIT_RUNGE_KUTTA_2 = EXPLICIT_EULER << 1,
        EXPLICIT_RUNGE_KUTTA_4 = EXPLICIT_RUNGE_KUTTA_2 << 1
    }

    public abstract class AnalysableDEModelVisualizer : VisualElement
    {
        #region Classes
        public new abstract class UxmlTraits : VisualElement.UxmlTraits
        {
            #region Methods
            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext creationContext)
            {
                base.Init(visualElement, bag, creationContext);
                AnalysableDEModelVisualizer analysableDEModelVisualizer = visualElement as AnalysableDEModelVisualizer;
                analysableDEModelVisualizer.Init();
            }
            #endregion Methods
        }
        #endregion Classes

        #region Fields
        protected string _relativeSaveFolderPath;
        protected AnalysableDEModel _model;
        protected Button _saveFolderSelectionButton;
        protected TextField _reportFolderPathField;
        protected TextField _reportNameField;
        protected Toggle _fullReportOption;
        protected EnumFlagsField _solvingTypesField;
        protected FloatField _minParameterField;
        protected FloatField _maxParameterField;
        protected FloatField _minParameterStepField;
        protected FloatField _maxParameterStepField;
        protected IntegerField _samplingFrequencyField;
		#endregion Fields

		#region Accessors
        public AnalysableDEModel Model
		{
            get { return _model; }
		}
        #endregion Accessors

        #region Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSaveFolderPathChanged(string absoluteFolderPath)
		{
            _relativeSaveFolderPath = string.Empty;

            if (absoluteFolderPath.StartsWith(Application.dataPath))
            {
                _relativeSaveFolderPath = "Assets" + absoluteFolderPath.Substring(Application.dataPath.Length);
            }
            else
			{
                // TODO : Add error log
                _relativeSaveFolderPath = "Assets";
			}

            _reportFolderPathField.value = _relativeSaveFolderPath;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSaveFolderPathChanged()
        {
            string absoluteFolderPath = string.IsNullOrEmpty(_relativeSaveFolderPath) ? Application.dataPath : Application.dataPath + _relativeSaveFolderPath.Substring("Assets".Length);
             absoluteFolderPath = EditorUtility.SaveFolderPanel("Select Report Folder", absoluteFolderPath, string.Empty);

            if (string.IsNullOrEmpty(absoluteFolderPath) == false)
			{
                OnSaveFolderPathChanged(absoluteFolderPath);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSelectedSolvingTypesChanged(SelectableDESolvingTypes solvingTypes)
        {
            if (_model != null)
            {
                _model.ClearSolvingTypes();

                if (solvingTypes.HasFlag(SelectableDESolvingTypes.EXPLICIT_EULER))
                {
                    _model.AddSolvingType(DESolvingType.EXPLICIT_EULER);
                }

                if (solvingTypes.HasFlag(SelectableDESolvingTypes.EXPLICIT_RUNGE_KUTTA_2))
                {
                    _model.AddSolvingType(DESolvingType.EXPLICIT_RUNGE_KUTTA_2);
                }

                if (solvingTypes.HasFlag(SelectableDESolvingTypes.EXPLICIT_RUNGE_KUTTA_4))
                {
                    _model.AddSolvingType(DESolvingType.EXPLICIT_RUNGE_KUTTA_4);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSelectedSolvingTypesChanged(ChangeEvent<Enum> evt)
		{
            SelectableDESolvingTypes solvingTypes = (SelectableDESolvingTypes)evt.newValue;
            OnSelectedSolvingTypesChanged(solvingTypes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMinParameterChanged(float minParameter, bool sanityCheck = true)
        {
            if (_model != null)
            {
                _model.MinParameter = minParameter;
                _minParameterField.value = _model.MinParameter;

                if (sanityCheck == true)
                {
                    _maxParameterField.value = _model.MaxParameter;
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMinParameterChanged(ChangeEvent<float> evt)
        {
            OnMinParameterChanged(evt.newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMaxParameterChanged(float maxParameter, bool sanityCheck = true)
        {
            if (_model != null)
            {
                _model.MaxParameter = maxParameter;
                _maxParameterField.value = _model.MaxParameter;

                if (sanityCheck == true)
                {
                    _minParameterField.value = _model.MinParameter;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMaxParameterChanged(ChangeEvent<float> evt)
        {
            OnMaxParameterChanged(evt.newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMaxParameterStepChanged(float maxParameterStep, bool sanityCheck = true)
        {
            if (_model != null)
            {
                _maxParameterStepField.value = math.max(maxParameterStep, 0.0f);

                if (sanityCheck == true && maxParameterStep < _minParameterStepField.value)
                {
                    _minParameterStepField.value = maxParameterStep;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMaxParameterStepChanged(ChangeEvent<float> evt)
        {
            OnMaxParameterStepChanged(evt.newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMinParameterStepChanged(float minParameterStep, bool sanityCheck = true)
        {
            _minParameterStepField.value = math.max(minParameterStep, 0.0f);

            if (sanityCheck == true && minParameterStep > _maxParameterStepField.value)
            {
                _maxParameterStepField.value = minParameterStep;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnMinParameterStepChanged(ChangeEvent<float> evt)
        {
            OnMinParameterStepChanged(evt.newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSamplingFrequencyChanged(int samplingFrequency)
        {
            _samplingFrequencyField.value = math.max(samplingFrequency, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSamplingFrequencyChanged(ChangeEvent<int> evt)
        {
            OnSamplingFrequencyChanged(evt.newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Init()
		{
            Clear();

            _model = InstantiateModel();

            _reportFolderPathField = new TextField();
            _reportFolderPathField.SetEnabled(false);

            _saveFolderSelectionButton = new Button()
            {
                text = "Select Report Folder"
            };
            _saveFolderSelectionButton.clicked += () => OnSaveFolderPathChanged();
            OnSaveFolderPathChanged(Application.dataPath);

            _reportNameField = new TextField("Report Name");

            _fullReportOption = new Toggle("Full Report");

            SelectableDESolvingTypes defaultSolvingType = SelectableDESolvingTypes.EXPLICIT_EULER;
            _solvingTypesField = new EnumFlagsField("Solving Types", defaultSolvingType);
			_solvingTypesField.RegisterValueChangedCallback(OnSelectedSolvingTypesChanged);
            OnSelectedSolvingTypesChanged(defaultSolvingType);

            float defaultMinParameter = 0.0f;
            _minParameterField = new FloatField("Min Parameter")
            {
                value = defaultMinParameter
            };
            _minParameterField.RegisterValueChangedCallback(OnMinParameterChanged);
            OnMinParameterChanged(defaultMinParameter, false);

            float defaultMaxParameter = 0.0f;
            _maxParameterField = new FloatField("Max Parameter")
            {
                value = defaultMaxParameter
            };
            _maxParameterField.RegisterValueChangedCallback(OnMaxParameterChanged);
            OnMaxParameterChanged(defaultMaxParameter, false);

            float defaultMaxParameterStep = 0.0f;
            _maxParameterStepField = new FloatField("Max Parameter Step")
            {
                value = defaultMaxParameterStep
            };
            _maxParameterStepField.RegisterValueChangedCallback(OnMaxParameterStepChanged);
            OnMaxParameterStepChanged(defaultMaxParameterStep, false);

            float defaultMinParameterStep = 0.0f;
            _minParameterStepField = new FloatField("Min Parameter Step")
            {
                value = defaultMinParameterStep
            };
            _minParameterStepField.RegisterValueChangedCallback(OnMinParameterStepChanged);
            OnMinParameterStepChanged(defaultMinParameterStep, false);

            int defaultSamplingFrequency = 1;
            _samplingFrequencyField = new IntegerField("Sampling Frequency")
            {
                value = defaultSamplingFrequency
            };
            _samplingFrequencyField.RegisterValueChangedCallback(OnSamplingFrequencyChanged);
            OnSamplingFrequencyChanged(defaultSamplingFrequency);

            Add(_saveFolderSelectionButton);
            Add(_reportFolderPathField);
            Add(_reportNameField);
            Add(_fullReportOption);
            Add(_solvingTypesField);
            Add(_minParameterField);
            Add(_maxParameterField);
            Add(_maxParameterStepField);
            Add(_minParameterStepField);
            Add(_samplingFrequencyField);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<AnalysisProgression> Analyse()
		{
            if (AssetDatabase.IsValidFolder(_relativeSaveFolderPath))
			{
                _model.ClearParameterSteps();

                float deltaParameterStep = (_maxParameterStepField.value - _minParameterStepField.value) / (float)_samplingFrequencyField.value;

                for (int i = 0; i < _samplingFrequencyField.value + 1; i++)
                {
                    _model.AddParameterStep(_maxParameterStepField.value - (float)i * deltaParameterStep);
                }

                foreach(AnalysisProgression progression in _model.Analyse(_reportNameField.value, _relativeSaveFolderPath, _fullReportOption.value))
				{
                    yield return progression;
				}
            }
            else
			{
                // TODO : Add error log
                yield return new AnalysisProgression
                {
                    Ratio = float.NaN,
                    Message = string.Empty
                };
			}
        }

        /// <summary>
		/// Dispose the data in the unmanaged memory.
		/// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
        {
            if (_model != null)
			{
                _model.Dispose();
            }
        }
        #endregion Methods

        #region Abstract Methods
        public abstract string GetName();
        public abstract AnalysableDEModel InstantiateModel();
		#endregion Abtract Methods
	}
}
