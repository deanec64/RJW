using RimWorld;
using Verse;

namespace rjw
{
	public class IncidentWorker_Sexualize : IncidentWorker
	{
		public override bool TryExecute(IncidentParms parms)
		{
			Genital_Helper.sexualize_everyone();
			Find.LetterStack.ReceiveLetter("Sexualization Complete", "All pawns have been sexualized.", LetterDefOf.Good, null);
			return true;
		}
	}
}