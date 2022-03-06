namespace dExplorer.Editor.Mathematics
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;

    public class Float2DEAnalysisValues : DEAnalysisValues<Vector2>
    {
		#region Classes
		public new class UxmlFactory : UxmlFactory<Float2DEAnalysisValues, UxmlTraits> { }

        public new class UxmlTraits : DEAnalysisValues<Vector2>.UxmlTraits
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
        protected override string ToString(Vector2 value)
		{
            return string.Format("({0}, {1})", value.x, value.y);
		}
		#endregion Methods
	}
}
