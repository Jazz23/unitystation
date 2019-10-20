﻿using UnityEngine;

/// <summary>
/// Main component for Mop. Allows mopping to be done on tiles.
/// </summary>
[RequireComponent(typeof(Pickupable))]
public class Mop : Interactable<PositionalHandApply>
{
	protected override bool WillInteract(PositionalHandApply interaction, NetworkSide side)
	{
		if (!base.WillInteract(interaction, side)) return false;
		//can only mop tiles
		if (!Validations.HasComponent<InteractableTiles>(interaction.TargetObject)) return false;
		return true;
	}

	protected override void ServerPerformInteraction(PositionalHandApply interaction)
	{
		//server is performing server-side logic for the interaction
		//do the mopping
		var progressFinishAction = new FinishProgressAction(
			reason =>
			{
				if (reason == FinishReason.COMPLETED)
				{
					CleanTile(interaction.WorldPositionTarget);
				}
			}
		);

		//Start the progress bar:
		UIManager.ServerStartProgress(interaction.WorldPositionTarget.RoundToInt(),
			5f, progressFinishAction, interaction.Performer, true);
	}

	public void CleanTile(Vector3 worldPos)
	{
		var worldPosInt = worldPos.CutToInt();
		var matrix = MatrixManager.AtPoint(worldPosInt, true);
		var localPosInt = MatrixManager.WorldToLocalInt(worldPosInt, matrix);
		matrix.MetaDataLayer.Clean(worldPosInt, localPosInt, true);
	}
}