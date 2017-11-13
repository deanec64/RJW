using System.Collections.Generic;

using RimWorld;
using Verse;

namespace rjw
{
	internal class InteractionWorker_AnalSexAttempt : InteractionWorker
	{
		public static bool AttemptAnalSex(Pawn rapist, Pawn victim)
		{
			//Log.Message(rapist.NameStringShort + " is attempting to anally rape " + victim.NameStringShort);
			return true;
		}

		public override float RandomSelectionWeight(Pawn rapist, Pawn victim)
		{
			// this interaction is triggered by the jobdriver
			if (rapist == null || victim == null) return 0.0f;
			return 0.0f; // base.RandomSelectionWeight(initiator, recipient);
		}

		public override void Interacted(Pawn rapist, Pawn victim, List<RulePackDef> extraSentencePacks)
		{
			if (rapist == null || victim == null) return;
			//Log.Message("[RJW] InteractionWorker_AnalRapeAttempt::Interacted( " + rapist.NameStringShort + ", " + victim.NameStringShort + " ) called");
			AttemptAnalSex(rapist, victim);
		}
	}
}