using UnityEngine;
using UnityEngine.UI;

public class UIScrollListScale : MonoBehaviour
{
	[SerializeField]
	private bool m_horizontal = true;

	[SerializeField]
	private Graphic m_graphic;

	[SerializeField]
	private RectTransform m_bounds;

	[SerializeField]
	private float m_minScale;

	[SerializeField]
	private float m_maxScale;

	[SerializeField]
	private float m_rangeFromCenter;

	[SerializeField]
	private AnimationCurve m_curve;

	[SerializeField]
	private float m_minWidth;

	[SerializeField]
	private float m_maxWidth;

	[SerializeField]
	private float m_minHeight;

	[SerializeField]
	private float m_maxHeight;

	[SerializeField]
	private LayoutElement m_layout;

	[SerializeField]
	private UIRenderPickup m_bigPickup;

	[SerializeField]
	private UIRenderCharacter m_characterRenderer;

	[SerializeField]
	private Character m_characterPrefab;

	private float m_width;

	private float m_height;

	private float m_graphicWidth;

	private float m_graphicHeight;

	private Vector3 m_boundCenter;

	private void Start()
	{
	}

	public void SetBounds(RectTransform bounds)
	{
		m_bounds = bounds;
	}

	private void Update()
	{
		if (m_bounds == null)
		{
			return;
		}
		Vector3[] array = new Vector3[4];
		m_bounds.GetWorldCorners(array);
		Vector3 vector = array[0];
		Vector3 vector2 = array[0];
		m_boundCenter = Vector3.zero;
		for (int i = 0; i < 4; i++)
		{
			m_boundCenter += array[i];
			vector.x = Mathf.Min(vector.x, array[i].x);
			vector.y = Mathf.Min(vector.y, array[i].y);
			vector2.x = Mathf.Max(vector2.x, array[i].x);
			vector2.y = Mathf.Max(vector2.y, array[i].y);
		}
		m_boundCenter /= 4f;
		m_width = vector2.x - vector.x;
		m_height = vector2.y - vector.y;
		Vector3[] array2 = new Vector3[4];
		m_graphic.rectTransform.GetWorldCorners(array2);
		Vector3 vector3 = array2[0];
		Vector3 vector4 = array2[0];
		Vector3 zero = Vector3.zero;
		for (int j = 0; j < 4; j++)
		{
			vector3.x = Mathf.Min(vector3.x, array2[j].x);
			vector3.y = Mathf.Min(vector3.y, array2[j].y);
			vector4.x = Mathf.Max(vector4.x, array2[j].x);
			vector4.y = Mathf.Max(vector4.y, array2[j].y);
			zero += array2[j];
		}
		m_graphicWidth = vector4.x - vector3.x;
		m_graphicHeight = vector4.y - vector3.y;
		zero /= 4f;
		if (m_horizontal)
		{
			float num = m_rangeFromCenter * m_width;
			float a = Mathf.Abs(m_boundCenter.x - zero.x);
			a = Mathf.Min(a, num);
			float num2 = Mathf.Lerp(m_minScale, m_maxScale, m_curve.Evaluate(a / num));
			for (int k = 0; k < base.transform.childCount; k++)
			{
				base.transform.GetChild(k).localScale = new Vector3(num2, num2, num2);
			}
			float num3 = Mathf.Lerp(m_maxWidth, m_minWidth, a / num);
			float num4 = Mathf.Lerp(m_maxHeight, m_minHeight, a / num);
			m_layout.minWidth = num3;
			m_layout.preferredWidth = num3;
			m_layout.minHeight = num4;
			m_layout.preferredHeight = num4;
			return;
		}
		float num5 = m_rangeFromCenter * m_height;
		float a2 = Mathf.Abs(m_boundCenter.y - zero.y);
		a2 = Mathf.Min(a2, num5);
		float num6 = Mathf.Lerp(m_minScale, m_maxScale, m_curve.Evaluate(a2 / num5));
		for (int l = 0; l < base.transform.childCount; l++)
		{
			base.transform.GetChild(l).localScale = new Vector3(num6, num6, num6);
		}
		float num7 = Mathf.Lerp(m_maxWidth, m_minWidth, a2 / num5);
		float num8 = Mathf.Lerp(m_maxHeight, m_minHeight, a2 / num5);
		m_layout.minWidth = num7;
		m_layout.preferredWidth = num7;
		m_layout.minHeight = num8;
		m_layout.preferredHeight = num8;
		Rect rect = new Rect(zero.x - m_graphicWidth * 0.5f, zero.y - m_graphicHeight * 0.5f, m_graphicWidth, m_graphicHeight);
		if (m_boundCenter.y >= rect.yMin && m_boundCenter.y <= rect.yMax)
		{
			Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array3 = componentsInChildren;
			foreach (Renderer renderer in array3)
			{
				renderer.enabled = false;
			}
			UIRenderPickup component = base.gameObject.GetComponent<UIRenderPickup>();
			Pickup pickupPrefab = null;
			string empty = string.Empty;
			if (component != null)
			{
				pickupPrefab = component.Prefab;
				empty = component.name;
			}
			Singleton<Game>.Instance.UIController.CharacterSelect.OnChangePickup(pickupPrefab, empty);
		}
		else if (rect.yMax < vector.y)
		{
			Renderer[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array4 = componentsInChildren2;
			foreach (Renderer renderer2 in array4)
			{
				renderer2.enabled = false;
			}
		}
		else if (rect.yMin > vector2.y)
		{
			Renderer[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array5 = componentsInChildren3;
			foreach (Renderer renderer3 in array5)
			{
				renderer3.enabled = false;
			}
		}
		else
		{
			Renderer[] componentsInChildren4 = base.gameObject.GetComponentsInChildren<Renderer>();
			Renderer[] array6 = componentsInChildren4;
			foreach (Renderer renderer4 in array6)
			{
				renderer4.enabled = true;
			}
		}
	}
}
