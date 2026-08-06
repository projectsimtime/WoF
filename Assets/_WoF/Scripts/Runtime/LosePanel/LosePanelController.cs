using System;
using UnityEngine;

namespace WoF
{
	public class LosePanelController : MonoBehaviour
	{
		[SerializeField] 
		private GiveUpButton _giveUpButton;
		[SerializeField] 
		private ReviveButton _reviveButton;

		private void OnValidate()
		{
			_giveUpButton = GetComponentInChildren<GiveUpButton>();
			_reviveButton = GetComponentInChildren<ReviveButton>();
		}
		
		
	}
}