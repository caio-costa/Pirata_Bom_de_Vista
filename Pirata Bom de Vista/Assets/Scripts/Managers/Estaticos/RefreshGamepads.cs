using UnityEngine;
using System.Collections;

public class RefreshGamepads : MonoBehaviour
{
	void Update()
	{
		// Atualiza o script GamepadManager
		GamepadManager.Instance.Refresh();
	}
}
