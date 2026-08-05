using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class ScaleToFitContent : UIBehaviour, ILayoutSelfController, ILayoutController
	{
		public enum FitMode
		{
			Unconstrained = 0,
			MinSize = 1,
			PreferredSize = 2
		}

		[SerializeField]
		protected FitMode m_HorizontalFit = FitMode.PreferredSize;

		[SerializeField]
		protected FitMode m_VerticalFit = FitMode.PreferredSize;

		[SerializeField]
		protected float m_MaxWidth;

		[SerializeField]
		protected float m_MaxWidthRelativeToParent = 1f;

		[SerializeField]
		protected float m_MaxHeight;

		[SerializeField]
		protected float m_MaxHeightRelativeToParent = 1f;

		[NonSerialized]
		private RectTransform m_Rect;

		private DrivenRectTransformTracker m_Tracker;

		public FitMode horizontalFit
		{
			get
			{
				return m_HorizontalFit;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref m_HorizontalFit, value))
				{
					SetDirty();
				}
			}
		}

		public FitMode verticalFit
		{
			get
			{
				return m_VerticalFit;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref m_VerticalFit, value))
				{
					SetDirty();
				}
			}
		}

		public float maxWidth
		{
			get
			{
				return m_MaxWidth;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref m_MaxWidth, value))
				{
					SetDirty();
				}
			}
		}

		public float maxWidthRelativeToParent
		{
			get
			{
				return m_MaxWidthRelativeToParent;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref m_MaxWidthRelativeToParent, value))
				{
					SetDirty();
				}
			}
		}

		public float maxHeight
		{
			get
			{
				return m_MaxHeight;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref m_MaxHeight, value))
				{
					SetDirty();
				}
			}
		}

		public float maxHeightRelativeToParent
		{
			get
			{
				return m_MaxHeightRelativeToParent;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref m_MaxHeightRelativeToParent, value))
				{
					SetDirty();
				}
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				if (m_Rect == null)
				{
					m_Rect = GetComponent<RectTransform>();
				}
				return m_Rect;
			}
		}

		protected ScaleToFitContent()
		{
		}

		protected override void OnEnable()
		{
			SetDirty();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
		}

		private void HandleSelfFittingAlongAxis(int axis)
		{
			FitMode fitMode = ((axis != 0) ? verticalFit : horizontalFit);
			if (fitMode == FitMode.Unconstrained)
			{
				return;
			}
			m_Tracker.Add(this, rectTransform, (DrivenTransformProperties)(((axis != 0) ? 2048 : 1024) | ((axis != 0) ? 8192 : 4096)));
			Vector2 anchorMax = rectTransform.anchorMax;
			Vector2 anchorMin = rectTransform.anchorMin;
			if (anchorMin[axis] == 0f && anchorMax[axis] == 1f)
			{
				anchorMin[axis] = 0.5f;
				anchorMax[axis] = 0.5f;
			}
			else
			{
				anchorMax[axis] = anchorMin[axis];
			}
			rectTransform.anchorMin = anchorMin;
			rectTransform.anchorMax = anchorMax;
			Vector2 sizeDelta = rectTransform.sizeDelta;
			if (fitMode == FitMode.MinSize)
			{
				sizeDelta[axis] = LayoutUtility.GetMinSize(m_Rect, axis);
			}
			else
			{
				sizeDelta[axis] = LayoutUtility.GetPreferredSize(m_Rect, axis);
			}
			rectTransform.sizeDelta = sizeDelta;
			float num = m_MaxWidth;
			float num2 = m_MaxHeight;
			float num3 = 1f;
			if (rectTransform.parent != null)
			{
				RectTransform componentInParent = rectTransform.parent.GetComponentInParent<RectTransform>();
				if (componentInParent != null)
				{
					num += componentInParent.sizeDelta.x * m_MaxWidthRelativeToParent;
				}
				if (componentInParent != null)
				{
					num2 += componentInParent.sizeDelta.y * m_MaxHeightRelativeToParent;
				}
			}
			if (sizeDelta.x > 0f && horizontalFit != FitMode.Unconstrained)
			{
				num3 = Mathf.Min(num3, num / sizeDelta.x);
			}
			if (sizeDelta.y > 0f && verticalFit != FitMode.Unconstrained)
			{
				num3 = Mathf.Min(num3, num2 / sizeDelta.y);
			}
			rectTransform.localScale = Vector3.one * num3;
		}

		public void SetLayoutHorizontal()
		{
			m_Tracker.Clear();
			HandleSelfFittingAlongAxis(0);
		}

		public void SetLayoutVertical()
		{
			HandleSelfFittingAlongAxis(1);
		}

		protected void SetDirty()
		{
			if (IsActive())
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
		}
	}
}
