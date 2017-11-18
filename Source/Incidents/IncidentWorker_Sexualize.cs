using RimWorld;
using Verse;

namespace rjw
{
	public class IncidentWorker_Sexualize : IncidentWorker
	{
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Genital_Helper.sexualize_everyone();
			Find.LetterStack.ReceiveLetter("Sexualization Complete", "All pawns have been sexualized.", LetterDefOf.PositiveEvent, null);
			return true;
		}
	}
}