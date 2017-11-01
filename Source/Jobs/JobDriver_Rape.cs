
using Verse;
using Verse.AI;

namespace rjw
{
	public abstract class JobDriver_Rape : JobDriver
	{
		protected int duration;

		protected int ticks_between_hearts;

		protected int ticks_between_hits = 50;

		protected int ticks_between_thrusts;

		protected TargetIndex iTarget = TargetIndex.A;

		protected bool isAnalSex = false;

		public abstract void roll_to_hit(Pawn rapist, Pawn p);

		protected readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
		{
			new CurvePoint(1f,  12f),
			new CurvePoint(16f, 6f),
			new CurvePoint(22f, 9f),
			new CurvePoint(30f, 12f),
			new CurvePoint(50f, 18f),
			new CurvePoint(75f, 24f)
		};

		public virtual void think_after_sex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false)
		{

		}

		public virtual void aftersex(Pawn pawn, Pawn part, bool violent = false, bool isCoreLovin = false, bool isAnalSex = false)
		{
			xxx.aftersex(pawn, part, true, isAnalSex: isAnalSex);
		}

		protected Pawn Target
		{
			get
			{
				return (Pawn)(CurJob.GetTarget(iTarget));
			}
		}


		// Should move these function to common
		public static bool ImplantToGenital(string defName, Pawn target)
		{
			HediffDef privates = HediffDef.Named(defName);
			BodyPartRecord genitalPart = target.RaceProps.body.AllParts.Find(bpr => bpr.def.defName == "Genitals");
			target.health.AddHediff(privates, genitalPart);

			return false;
		}
		public static bool ImplantToAnal(string defName, Pawn target)
		{
			HediffDef privates = HediffDef.Named(defName);
			Hediff hediff = HediffMaker.MakeHediff(privates, target, null);

			BodyPartRecord genitalPart = target.RaceProps.body.AllParts.Find(bpr => bpr.def.defName == "Anus");
			target.health.AddHediff(privates, genitalPart);

			return false;
		}
	}
}
