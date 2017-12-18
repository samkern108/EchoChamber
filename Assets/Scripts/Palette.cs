using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Palette {

	private static int[] colors = new int[] 
	{
		0xDC143C,
		0xFF6347,
		0xBA55D3,
		0x228B22,
		0x20B2AA,
		0x48D1CC,
		0x1E90FF
	};

	public static Color PlayerColor = new Color(91/255.0f,198/255.0f,251/255.0f);
	public static Color GhostColor = new Color(145/255.0f,187/255.0f,207/255.0f);
	public static Color invisible = new Color(1.0f, 1.0f, 1.0f, 0.0f);

	public static Color GetRandomGhostColor() {
		return ColorFromInt(colors[Random.Range(0, colors.Length - 1)]);
	}

	public static Color ColorFromInt(int c, float alpha = 1.0f)
	{
		int r = (c >> 16) & 0x000000FF;
		int g = (c >> 8) & 0x000000FF;
		int b = c & 0x000000FF;

		Color ret = ColorFromIntRGB(r, g, b);
		ret.a = alpha;

		return ret;
	}

		public static Color ColorFromIntRGB(int r, int g, int b)
	{
		return new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, 1.0f);
	}
}
