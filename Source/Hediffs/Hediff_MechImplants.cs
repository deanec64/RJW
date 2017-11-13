using System.Collections.Generic;
using Verse;

namespace rjw
{
	internal class Hediff_MechImplants : HediffWithComps
	{
		public override bool TryMergeWith(Hediff other)
		{
			return false;
		}
	}

	[StaticConstructorOnStartup]
	internal class HediffDef_MechImplants : HediffDef_EnemyImplants
	{
		public List<string> randomHediffDefs = new List<string>();
		public int minEventInterval = 30000;
		public int maxEventInterval = 90000;
	}
}