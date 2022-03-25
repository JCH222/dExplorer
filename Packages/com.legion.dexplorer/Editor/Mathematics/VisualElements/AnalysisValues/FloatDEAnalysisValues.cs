namespace dExplorer.Editor.Mathematics
{
	using System.Collections.Generic;
    using UnityEngine.UIElements;

	/// <summary>
	/// Visualizer of the float analysis values.
	/// </summary>
	public class FloatDEAnalysisValues : DEAnalysisValues<float>
    {
        #region Classes
        public new class UxmlFactory : UxmlFactory<FloatDEAnalysisValues, UxmlTraits> { }

        public new class UxmlTraits : DEAnalysisValues<float>.UxmlTraits
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
	}
}
