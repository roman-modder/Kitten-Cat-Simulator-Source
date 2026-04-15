using UnityEngine;

public class PlayerVariables
{
	private static Color[] playerColors = new Color[4]
	{
		Color.yellow,
		Color.red,
		Color.green,
		Color.cyan
	};

	private static string[] playerColorNames = new string[4] { "yellow", "red", "green", "cyan" };

	private static Material[] playerMaterials = new Material[playerColors.Length];

	public static Color GetColor(int playerId)
	{
		if (playerId <= 0)
		{
			return Color.white;
		}
		return playerColors[playerId % playerColors.Length];
	}

	public static string GetColorName(int playerId)
	{
		if (playerId <= 0)
		{
			return "none";
		}
		return playerColorNames[playerId % playerColors.Length];
	}

	public static Material GetMaterial(Material original, int playerId)
	{
		Material material = playerMaterials[playerId % playerMaterials.Length];
		if (material == null)
		{
			material = new Material(original);
			material.color = GetColor(playerId);
			playerMaterials[playerId % playerMaterials.Length] = material;
		}
		return material;
	}
}
