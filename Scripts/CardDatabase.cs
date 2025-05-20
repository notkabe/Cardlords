using Godot;
using System;
using System.Collections.Generic;

public partial class CardDatabase : Node
{
	// Base de datos de cartas conformada por Nombre - Ataque - Vida
	public static readonly Dictionary<string, int[]> CARDS = new()
	{
		{ "Knight", new int[] { 2, 3 } },
		{ "Demon",  new int[] { 2, 4 } },
		{ "Fenrir", new int[] { 1, 3 } },
		{ "Mage",   new int[] { 1, 5 } },
		{ "Hunter", new int[] { 4, 2 } }
	};
}
