using UnityEngine;
using UnityEngine.UI;
using WoF.Core;

namespace WoF.UI
{
	/// <summary>
	/// Base for every clickable control. It owns the one thing all buttons genuinely share,
	/// the <see cref="Button"/>, and nothing else: dependencies arrive through
	/// <see cref="OnInject"/> so a button only ever knows the command interface it needs.
	/// </summary>
	public abstract class ButtonController : MonoBehaviour, IInjectable
	{
		[SerializeField]
		private Button _button;

		protected Button Button
		{
			get
			{
				if (_button == null)
				{
					_button = GetComponent<Button>();
				}

				return _button;
			}
		}

		protected virtual void OnValidate()
		{
			_button = GetComponent<Button>();
		}

		protected virtual void OnEnable()
		{
			if (Button != null)
			{
				Button.onClick.AddListener(OnButtonClicked);
			}
		}

		protected virtual void OnDisable()
		{
			if (Button != null)
			{
				Button.onClick.RemoveListener(OnButtonClicked);
			}
		}

		void IInjectable.Inject(IServiceResolver services)
		{
			OnInject(services);
		}

		/// <summary>
		/// Called once by the composition root, before the first frame.
		/// </summary>
		protected abstract void OnInject(IServiceResolver services);

		protected abstract void OnButtonClicked();

		public void SetInteractable(bool interactable)
		{
			if (Button != null)
			{
				Button.interactable = interactable;
			}
		}
	}
}
