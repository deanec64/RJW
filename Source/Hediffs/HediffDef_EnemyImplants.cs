using System.Collections.Generic;
using Verse;

namespace rjw
{
	[StaticConstructorOnStartup]
	internal class HediffDef_EnemyImplants : HediffDef
	{
		public string parentDef = "";
		public List<string> parentDefs = new List<string>();

		public bool IsParent(string defnam)
		{
			return parentDef == defnam || parentDefs.Contains(defnam);
		}
	}

	[StaticConstructorOnStartup]
	internal class HediffDef_InsectEgg : HediffDef_EnemyImplants
	{
		public int bornTick = 900000;//1 Quadrum
		public int abortTick = 60000;//1 day
	}
}