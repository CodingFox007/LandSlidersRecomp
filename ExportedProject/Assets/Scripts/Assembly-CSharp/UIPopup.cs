using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private Text[] m_textFields;

	[SerializeField]
	private Image[] m_images;

	[SerializeField]
	private UnityEvent m_onShow = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onHide = new UnityEvent();

	private bool m_active;

	private int m_id;

	public bool IsShowing
	{
		get
		{
			return m_active;
		}
	}

	public int Id
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}

	public void Create(string[] text, Sprite[] icons)
	{
		if (text != null)
		{
			for (int i = 0; i < text.Length && i < m_textFields.Length; i++)
			{
				m_textFields[i].text = text[i];
			}
		}
		if (icons != null)
		{
			for (int j = 0; j < icons.Length && j < m_images.Length; j++)
			{
				m_images[j].sprite = icons[j];
			}
		}
	}

	public void Show(int id)
	{
		m_id = id;
		m_active = true;
		m_onShow.Invoke();
	}

	public void Hide()
	{
		m_active = false;
		m_onHide.Invoke();
	}

	public void OnDisableScreen()
	{
		m_active = false;
		Singleton<Game>.Instance.UIController.HUD.RemovePopup(this);
	}

	public void DisableMisionAccept()
	{
		OnDisableScreen();
		Singleton<Game>.Instance.UIController.HUD.DisableMisionAccept();
	}

	public void CreateMissionAcceptPopup()
	{
		Singleton<Game>.Instance.UIController.HUD.CreateMissionAcceptPopup(this);
	}
}
