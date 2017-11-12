using System;
using System.Text;
using UnityEngine;

using Verse;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;

namespace rjw
{
	[StaticConstructorOnStartup]
	class HediffDef_EnemyImplants : HediffDef
	{
		public string parentDef = "";
		public List<string> parentDefs = new List<string>();

		public bool IsParent(string defnam)
		{
			return parentDef == defnam || parentDefs.Contains(defnam);
		}
	}

	[StaticConstructorOnStartup]
	class HediffDef_InsectEgg : HediffDef_EnemyImplants
	{
		public int bornTick = 900000;//1 Quadrum
		public int abortTick = 60000;//1 day
	}
}
