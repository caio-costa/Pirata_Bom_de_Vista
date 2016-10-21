using UnityEngine;
using XInputDotNetPure;
using System.Collections.Generic;

// Armazena o estado de um botão
public struct xButton
{
	public ButtonState prev_state;
	public ButtonState state;
}

// Armazena o valor de um trigger
public struct TriggerState {
	public float prev_value;
	public float current_value;
}

public struct ThumbSticks{
    public float prev_value;
    public float current_value;
}

// Evento para vibração
class xRumble
{
	public float timer;    // Timer
	public float fadeTime; // Fade (segundos)
	public Vector2 power;  // Intensidade
	
	// Decrementa o tempo
	public void Update() {
		this.timer -= Time.deltaTime;
	}
}

// Xbox 360 Gamepad
public class x360_Gamepad
{
	private GamePadState prev_state; // Estado anterior do controle
	private GamePadState state;      // Estado Atual
	
	private int gamepadIndex;           // Índice numérico (1 - 4)
	private PlayerIndex playerIndex;    // Índice XInput do Player
	private List<xRumble> rumbleEvents; // Armazena eventos p/ vibre
	
	// Dicionário de mapa para os botões
	private Dictionary<string, xButton> inputMap;

	// Estados para todos os botões suportados
	private xButton A,B,X,Y;
	private xButton DPad_Up, DPad_Down, DPad_Left, DPad_Right;
	
	private xButton Guide;       // Xbox logo
	private xButton Back, Start;
	private xButton L3, R3;
	private xButton LB, RB;
	private TriggerState LT, RT; // Trigger

    private ThumbSticks RightAxisX, RightAxisY;
    private ThumbSticks LeftAxisX, LeftAxisY;

    public bool EnableRumble; // Vibre lig/desl
	
	// Construtor
	public x360_Gamepad(int index)
	{
		// Configura o índice do controle
		gamepadIndex = index - 1;
		playerIndex = (PlayerIndex)gamepadIndex;

		EnableRumble = true;
		
		// Cria lista de vibres e dicionário de entradas
		rumbleEvents = new List<xRumble>();
		inputMap = new Dictionary<string, xButton>();
	}
	
	// Atualiza o mapa de entradas
	void UpdateInputMap()
	{
		inputMap["A"] = A;
		inputMap["B"] = B;
		inputMap["X"] = X;
		inputMap["Y"] = Y;

		inputMap["DPad_Up"]    = DPad_Up;
		inputMap["DPad_Down"]  = DPad_Down;
		inputMap["DPad_Left"]  = DPad_Left;
		inputMap["DPad_Right"] = DPad_Right;
		
		inputMap["Guide"] = Guide;
		inputMap["Back"]  = Back;
		inputMap["Start"] = Start;
		
		inputMap["L3"] = L3;
		inputMap["R3"] = R3;
		inputMap["LB"] = LB;
        inputMap["RB"] = RB;

    }
	
	#region Update
	
	// Atualiza o estado do controle
	public void Update()
	{
		// Estado atual
		state = GamePad.GetState(playerIndex);

		// Confere se está conectado
		if (state.IsConnected)
		{
			A.state = state.Buttons.A;
			B.state = state.Buttons.B;
			X.state = state.Buttons.X;
			Y.state = state.Buttons.Y;
			
			DPad_Up.state    = state.DPad.Up;
			DPad_Down.state  = state.DPad.Down;
			DPad_Left.state  = state.DPad.Left;
			DPad_Right.state = state.DPad.Right;
			
			Guide.state = state.Buttons.Guide;
			Back.state  = state.Buttons.Back;
			Start.state = state.Buttons.Start;
			L3.state    = state.Buttons.LeftStick;
			R3.state    = state.Buttons.RightStick;
			LB.state    = state.Buttons.LeftShoulder;
			RB.state    = state.Buttons.RightShoulder;
			
			LT.current_value = state.Triggers.Left;
			RT.current_value = state.Triggers.Right;

            //RightAxisX.current_value = state.ThumbSticks.Right.X;
            //RightAxisY.current_value = state.ThumbSticks.Right.Y;

            //LeftAxisX.current_value = state.ThumbSticks.Left.X;
            //LeftAxisY.current_value = state.ThumbSticks.Left.Y;

            UpdateInputMap(); // Atualiza dicionário de entradas
			HandleRumble();   // Atualiza os eventos de vibre
		}
	}
	
	#endregion
	
	#region Refresh
	
	// Atualiza o estado anterior do controle
	public void Refresh()
	{
		// Salva o estado atual para a p~´oxima atualização
		prev_state = state;
		
		// Checa se o controle está conectado
		if (state.IsConnected)
		{
			A.prev_state = prev_state.Buttons.A;
			B.prev_state = prev_state.Buttons.B;
			X.prev_state = prev_state.Buttons.X;
			Y.prev_state = prev_state.Buttons.Y;
			
			DPad_Up.prev_state    = prev_state.DPad.Up;
			DPad_Down.prev_state  = prev_state.DPad.Down;
			DPad_Left.prev_state  = prev_state.DPad.Left;
			DPad_Right.prev_state = prev_state.DPad.Right;
			
			Guide.prev_state = prev_state.Buttons.Guide;
			Back.prev_state  = prev_state.Buttons.Back;
			Start.prev_state = prev_state.Buttons.Start;
			L3.prev_state    = prev_state.Buttons.LeftStick;
			R3.prev_state    = prev_state.Buttons.RightStick;
			LB.prev_state    = prev_state.Buttons.LeftShoulder;
			RB.prev_state    = prev_state.Buttons.RightShoulder;

			LT.prev_value = prev_state.Triggers.Left;
			RT.prev_value = prev_state.Triggers.Right;

            //RightAxisX.prev_value = prev_state.ThumbSticks.Right.X;
            //RightAxisY.prev_value = prev_state.ThumbSticks.Right.Y;

            //LeftAxisX.prev_value = prev_state.ThumbSticks.Left.X;
            //LeftAxisY.prev_value = prev_state.ThumbSticks.Left.Y;

            UpdateInputMap();
		}
	}
	
	#endregion
	
	public int Index { get { return gamepadIndex; } }

	public bool IsConnected { get { return state.IsConnected; } }
	
	// Atualiza e aplica os eventos de vibre
	void HandleRumble()
	{
		if (rumbleEvents.Count > 0)
		{
			Vector2 currentPower = new Vector2(0,0);

			for (int i = 0; i < rumbleEvents.Count; ++i)
			{
				rumbleEvents[i].Update();
				
				if (rumbleEvents[i].timer > 0)
				{
					float timeLeft = Mathf.Clamp(rumbleEvents[i].timer / rumbleEvents[i].fadeTime, 0f, 1f);
					currentPower = new Vector2(Mathf.Max(rumbleEvents[i].power.x * timeLeft, currentPower.x),
					                           Mathf.Max(rumbleEvents[i].power.y * timeLeft, currentPower.y));

					// Aplica o vibre aos motores
					GamePad.SetVibration(playerIndex, currentPower.x, currentPower.y);
				}
				else
				{
					// Remove eventos expirados
					rumbleEvents.Remove(rumbleEvents[i]);
				}
			}
		}
	}
	
	// Adiciona um evento de vibre
	public void AddRumble(float timer, Vector2 power, float fadeTime = 0f)
	{
		xRumble rumble = new xRumble();
		
		rumble.timer = timer;
		rumble.power = power;
		rumble.fadeTime = fadeTime;
		
		// Adiciona o evento para a lista
		rumbleEvents.Add(rumble);
	}
	
    //Retorna os estados dos botões

	// Eixo thumbstick esquerdo X
	public float GetStick_LX() {
		return state.ThumbSticks.Left.X;
	}

    // Eixo thumbstick esquerdo Y
    public float GetStick_LY()
    {
        return state.ThumbSticks.Left.Y;
    }

    // Eixo thumbstick direito X
    public float GetStick_RX()
    {
        return state.ThumbSticks.Right.X;
    }

    // Eixo thumbstick direito Y
    public float GetStick_RY()
    {
        return state.ThumbSticks.Right.Y;
    }


    // Retorna os dois eixos juntos (Esquerdo)
    public XInputDotNetPure.GamePadThumbSticks.StickValue GetStick_L()
    {
        return state.ThumbSticks.Left;
    }

    // Retorna os dois eixos juntos (Direito)
    public XInputDotNetPure.GamePadThumbSticks.StickValue GetStick_R() {
		return state.ThumbSticks.Right;
	}
	
	public float GetTrigger_L { get { return state.Triggers.Left; } }
	
	public float GetTrigger_R { get { return state.Triggers.Right; } }
	
	// Confere se o trigger esquerdo foi pressionado - no frame ATUAL
	public bool GetTriggerTap_L() {
		return (LT.prev_value == 0f && LT.current_value >= 0.1f) ? true : false;
	}

    // Confere se o trigger direito foi pressionado - no frame ATUAL
    public bool GetTriggerTap_R() {
		return (RT.prev_value == 0f && RT.current_value >= 0.1f) ? true : false;
	}
	
	// Retorna o estado do botão (A, B, X, Y, LB, RB)
	public bool GetButton(string button) {
		return inputMap[button].state == ButtonState.Pressed ? true : false;
	}

    // Retorna o estado do botão - no frame ATUAL (A, B, X, Y, LB, RB)
    public bool GetButtonDown(string button) {
		return (inputMap[button].prev_state == ButtonState.Released &&
		        inputMap[button].state == ButtonState.Pressed) ? true : false;
	}
}