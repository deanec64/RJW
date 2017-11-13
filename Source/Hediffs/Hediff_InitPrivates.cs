// This is kind of a hack but it's the only way I could find to add a hediff automatically to every pawn. The way it works is that there's a dummy hediff
// that gets added to a pawn's genitals on every birthday (this just uses the same code that generates cancer and other diseases of aging, but with a 100%
// chance to trigger). Then this class gets invoked on the dummy hediff, which it drops and replaces with proper sex parts.

using Verse;

namespace rjw
{
	public class Hediff_InitPrivates : Hediff_AddedPart
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			//Log.Message("[RJW]Hediff_InitPrivates::PostAdd is called0 - pawn is " + pawn.NameStringShort);
			if (!Genital_Helper.is_sexualized(pawn))
			{
				Genital_Helper.sexualize(pawn);
				//Log.Message("[RJW]Hediff_InitPrivates::PostAdd is called1 - pawn is " + pawn.NameStringShort);

				std.generate_on(pawn);
			}

			// Remove the dummy hediff
			pawn.health.RemoveHediff(this);
		}
	}
}