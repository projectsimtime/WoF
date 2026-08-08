using System;
using TMPro;
using UnityEngine;

namespace WoF.LosePanel
{
	public class LosePanelReviveCostText : MonoBehaviour
	{
		[SerializeField] 
		private TextMeshProUGUI _text;

		private void OnValidate()
		{
			_text = GetComponent<TextMeshProUGUI>();
		}

		public void SetReviveCost(int amount)
		{
			_text.text = amount.ToString();
		}
	}
}