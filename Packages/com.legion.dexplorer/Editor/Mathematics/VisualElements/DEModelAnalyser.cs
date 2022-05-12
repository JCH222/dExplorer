namespace dExplorer.Editor.Mathematics
{
	using System.Threading;
	using UnityEditor;
	using UnityEngine.UIElements;

	public class DEModelAnalyser : EditorWindow
    {
		#region Constants
		private const string NAME = "Diff Equation Analyser";
		private const string UXML_FILE_PATH = "Packages/com.legion.dexplorer/Editor/Mathematics/VisualElements/Templates/DEModelAnalyser.uxml";
		#endregion Constants

		#region Static Fields
		private const string MODEL_SELECTOR_KEY = "model-selector";
		private const string GENERATE_BUTTON_KEY = "generate-button";
		#endregion Static Fields

		#region Properties
		private AnalysableDEModelSelector ModelSelector { get; set; } = null;
		#endregion Properties

		#region Static Methods
		[MenuItem("Window/Legion/" + NAME, priority = 40)]

        public static void ShowWindow()
        {
			DEModelAnalyser _ = GetWindow(typeof(DEModelAnalyser), false, NAME) as DEModelAnalyser;
		}

		private static void OnAnalyseAsked(AnalysableDEModelSelector modelSelector, Button generateButton)
		{
			modelSelector.SetEnabled(false);
			generateButton.SetEnabled(false);

			foreach (AnalysisProgression progression in modelSelector.Analyse())
			{
				EditorUtility.DisplayProgressBar("Analysis", progression.Message, progression.Ratio);
				Thread.Sleep(100);
			}

			EditorUtility.ClearProgressBar();

			modelSelector.SetEnabled(true);
			generateButton.SetEnabled(true);
		}
		#endregion Static Methods

		#region Methods
		public void OnEnable()
		{
			VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_FILE_PATH);
			VisualElement root = asset.CloneTree();

			AnalysableDEModelSelector modelSelector = root.Query<AnalysableDEModelSelector>(MODEL_SELECTOR_KEY).First();
			Button generateButton = root.Query<Button>(GENERATE_BUTTON_KEY).First();

			modelSelector.GenerateButton = generateButton;

			generateButton.SetEnabled(false);
			generateButton.visible = false;
			generateButton.clicked += () => OnAnalyseAsked(modelSelector, generateButton);

			if (ModelSelector != null)
			{
				ModelSelector.Dispose();
			}

			ModelSelector = modelSelector;
			rootVisualElement.Clear();
			rootVisualElement.Add(root);
		}

		public void OnDestroy()
		{
			if (ModelSelector != null)
			{
				ModelSelector.Dispose();
			}
		}
		#endregion Methods
	}
}
