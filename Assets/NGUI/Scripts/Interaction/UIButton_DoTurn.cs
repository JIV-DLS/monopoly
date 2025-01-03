//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------
using System.Collections;
using UnityEngine;

/// <summary>
/// Similar to UIButtonColor, but adds a 'disabled' state based on whether the collider is enabled or not.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton_DoTurn : UIButtonColor
{
	/// <summary>
	/// Color that will be applied when the button is disabled.
	/// </summary>
	
	public PlayController monopolyGame;
	public UIButton_DoMove playerMoves;
	public DicesManager DicesManager;
	private ArrayList playerList;

	public Color disabledColor = Color.grey;

	/// <summary>
	/// If the collider is disabled, assume the disabled color.
	/// </summary>

	protected override void OnEnable ()
	{
		if (isEnabled) base.OnEnable();
		else UpdateColor(false, true);
	}

	public override void OnHover (bool isOver) { if (isEnabled) base.OnHover(isOver); }
	public override void OnPress (bool isPressed) { 
		if (isEnabled) base.OnPress(isPressed);
		// DicesManager.ThrowDice();
		/*if (Input.GetMouseButtonUp (0)) {
			playerList = monopolyGame.GetPlayerList();
			runPlayerTurns();
		}*/
	}

	public void runPlayerTurns (){
		StartCoroutine (runTurns ());
	}

	IEnumerator runTurns(){
		for (int i = 0; i < playerList.Count; i++) {
			playerMoves.playerMove(playerList[i] as PlayMoving);
			//maybe insert an if-else statement here as a conditional IEnumerator
			yield return new WaitForSeconds(1.20f);
		}
	}


	/// <summary>
	/// Whether the button should be enabled.
	/// </summary>
	public bool isEnabled
	{
		get
		{
			Collider col = GetComponent<Collider>();
			return col && col.enabled;
		}
		set
		{
			Collider col = GetComponent<Collider>();
			if (!col) return;

			if (col.enabled != value)
			{
				col.enabled = value;
				UpdateColor(value, false);
			}
		}
	}

	/// <summary>
	/// Update the button's color to either enabled or disabled state.
	/// </summary>

	public void UpdateColor (bool shouldBeEnabled, bool immediate)
	{
		if (tweenTarget)
		{
			if (!mStarted)
			{
				mStarted = true;
				Init();
			}

			Color c = shouldBeEnabled ? defaultColor : disabledColor;
			TweenColor tc = TweenColor.Begin(tweenTarget, 0.15f, c);

			if (immediate)
			{
				tc.color = c;
				tc.enabled = false;
			}
		}
	}
}
