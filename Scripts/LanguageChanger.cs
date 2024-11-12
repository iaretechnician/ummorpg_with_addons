using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

public class LanguageChanger : MonoBehaviour
{
	public string currentLanguage = "English";
	
	private void Start()
	{
		LocalizationManager.CurrentLanguage = currentLanguage;		
	}
}
