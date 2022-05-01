namespace dExplorer.Editor.Mathematics
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Visualizer of the float 3 analysis values.
    /// </summary>
    public class Float3DEAnalysisValues : DEAnalysisValues<Vector3>
    {
        #region Classes
        public new class UxmlFactory : UxmlFactory<Float3DEAnalysisValues, UxmlTraits> { }

        public new class UxmlTraits : DEAnalysisValues<Vector3>.UxmlTraits
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
        protected override string ToString(Vector3 value)
        {
            return string.Format("({0}, {1}, {2})", value.x, value.y, value.z);
        }
        #endregion Methods
    }
}
