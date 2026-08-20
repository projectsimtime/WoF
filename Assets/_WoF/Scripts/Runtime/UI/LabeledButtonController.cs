using TMPro;
using UnityEngine;

namespace WoF.UI
{
	/// <summary>
	/// For the few buttons whose label changes at runtime. Buttons with a fixed label do not
	/// carry the reference, which keeps <see cref="ButtonController"/> down to what every
	/// button really uses.
	/// </summary>
	public abstract class LabeledButtonController : ButtonController
	{
		[SerializeField]
		private TextMeshProUGUI _buttonText;

		protected override void OnValidate()
		{
			base.OnValidate();

			_buttonText = GetComponentInChildren<TextMeshProUGUI>(true);
		}

		public void SetButtonText(string text)
		{
			if (_buttonText != null)
			{
				_buttonText.text = text;
			}
		}
	}
}
