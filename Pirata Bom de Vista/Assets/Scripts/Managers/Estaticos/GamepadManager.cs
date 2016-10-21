using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamepadManager : MonoBehaviour
{
	public int GamepadCount = 1; // Número de controles para gerenciar

	private List<x360_Gamepad> gamepads;
	private static GamepadManager singleton; 

	void Awake()
	{
		// Destroi outra instância, caso haja mais de uma
		if (singleton != null && singleton != this)
		{
			Destroy(this.gameObject);
			return;
		}
		else
		{
			// Cria uma instância
			singleton = this;
			DontDestroyOnLoad(this.gameObject);

			// Bloqueia contagem para o número de controles suportados
			GamepadCount = Mathf.Clamp(GamepadCount, 1, 4);

			gamepads = new List<x360_Gamepad>();

			// Cria o número específico de instâncias conforme o número de controles suportados
			for (int i = 0; i < GamepadCount; ++i)
				gamepads.Add(new x360_Gamepad(i + 1));
		}
	}

	public static GamepadManager Instance
	{
		get
		{
			if (singleton == null)
			{
				Debug.LogError("[GamepadManager]: Não existe nenhuma instância!");
				return null;
			}

			return singleton;
		}
	}
	
	void Update()
	{
		for (int i = 0; i < gamepads.Count; ++i)
			gamepads[i].Update();
	}

	public void Refresh()
	{
		for (int i = 0; i < gamepads.Count; ++i)
			gamepads[i].Refresh();
	}

	public x360_Gamepad GetGamepad(int index)
	{
		for (int i = 0; i < gamepads.Count;)
		{
			if (gamepads[i].Index == (index - 1))
				return gamepads[i];
			else
				++i;
		}

			Debug.LogError("[GamepadManager]: " + index
				+ " Não é um índice válido para um joystick!");

		return null;
	}

	// Retorna número de controles conectados
	public int ConnectedTotal()
	{
		int total = 0;

		for (int i = 0; i < gamepads.Count; ++i)
		{
			if (gamepads[i].IsConnected)
				total++;
		}

		return total;
	}

	// Confere se há botões pressionados em todos os controles
	public bool GetButtonAny(string button)
	{
		for (int i = 0; i < gamepads.Count; ++i)
		{
			// Controle conectado e botão pressionado?
			if (gamepads[i].IsConnected && gamepads[i].GetButton(button))
				return true;
		}

		return false;
	}

    // Confere se há botões pressionados em todos os controles - no frame ATUAL.
    public bool GetButtonDownAny(string button)
	{
		for (int i = 0; i < gamepads.Count; ++i)
		{
            // Controle conectado e botão pressionado?
            if (gamepads[i].IsConnected && gamepads[i].GetButtonDown(button))
				return true;
		}
		
		return false;
	}
}