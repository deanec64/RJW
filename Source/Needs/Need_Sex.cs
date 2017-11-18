using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Need_Sex : Need_Seeker
	{
		private bool isHuman => xxx.is_human(pawn);

		private bool isFemale => xxx.is_female(pawn);

		private bool isNympho => xxx.is_nympho(pawn);

		private bool isInvisible => pawn.Map == null;

		private bool BootStrapTriggered = false;

		//private bool isSexualized = false;

		private int needsex_tick = needsex_tick_timer;
		private static int needsex_tick_timer = 10;
		private static float decay_per_day = 0.3f;
		private float decay_rate_modifier = Mod_Settings.sexneed_decay_rate;

		private static float sex_min_age = (float)Mod_Settings.sex_minimum_age;

		//private int startInterval = Find.TickManager.TicksGame;
		//private static int tickInterval = 10;

		//private int std_tick = 1;

		private static SimpleCurve sex_need_factor_from_age = new SimpleCurve
		{
			new CurvePoint(sex_min_age, 0f),
			new CurvePoint(18f, 1.00f),
			new CurvePoint(28f, 1.00f),
			new CurvePoint(30f, 0.90f),
			new CurvePoint(40f, 0.75f),
			new CurvePoint(50f, 0.50f),
			new CurvePoint(60f, 0.20f),
  			new CurvePoint(80f, 0f)

			/* Edited by nizhuan-jjr
			new CurvePoint(5f,  0.25f),
			new CurvePoint(16f, 1.00f),
			new CurvePoint(22f, 1.00f),
			new CurvePoint(30f, 0.90f),
			new CurvePoint(40f, 0.75f),
			new CurvePoint(60f, 0.50f),
			new CurvePoint(80f, 0.25f)
			*/
		};

		/* Edited by nizhuan-jjr : Animals' Sex Need is removed now
        private static readonly SimpleCurve animal_sex_need_factor_from_age = new SimpleCurve
        {
            new CurvePoint(0f,  0.00f),
            new CurvePoint(1f,  1.00f),
            new CurvePoint(5f,  2.00f),
            new CurvePoint(10f, 1.00f),
            new CurvePoint(20f, 1.00f),
            new CurvePoint(30f, 0.90f),
            new CurvePoint(40f, 0.50f),
            new CurvePoint(60f, 0.20f),
        };
        */

		public float thresh_frustrated()
		{
			return 0.05f;
		}

		public float thresh_horny()
		{
			return 0.25f;
		}

		public float thresh_neutral()
		{
			return 0.50f;
		}

		public float thresh_satisfied()
		{
			return 0.75f;
		}

		public float thresh_ahegao()
		{
			return 0.95f;
		}

		public Need_Sex(Pawn pawn) : base(pawn)
		{
			//if (xxx.is_mechanoid(pawn)) return; //Added by nizhuan-jjr:Misc.Robots are not allowed to have sex, so they don't need sex actually.
			threshPercents = new List<float>
			{
				thresh_frustrated(),
				thresh_horny(),
				thresh_neutral(),
				thresh_satisfied(),
				thresh_ahegao()
			};
		}

		/*
        public static float balance_factor(float lev)
        {
            const float one_on_point_three = 1.0f / 0.30f;
            if (lev >= 0.70f)
                return 1.0f + one_on_point_three * (lev - 0.70f) * one_on_point_three;
            else if (lev >= 0.30f)
                return 1.0f;
            else
                return 1.0f - 0.5f * one_on_point_three * (0.30f - lev);
        }
        */

		public static float brokenbodyfactor(Pawn pawn)
		{
			//This adds in the broken body system
			float broken_body_factor = 1f;
			if (pawn.health.hediffSet.HasHediff(xxx.feelingBroken))
			{
				switch (pawn.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken).CurStageIndex)
				{
					case 0:
						return 0.75f;

					case 1:
						return 1.4f;

					case 2:
						return 2f;
				}
			}
			return broken_body_factor;
		}

		public override void NeedInterval() //150 ticks between each calls
		{
			if (isInvisible) return;

			sex_min_age = (float)Mod_Settings.sex_minimum_age;
			float age = pawn.ageTracker.AgeBiologicalYearsFloat;

			if (needsex_tick <= 0 && age > Mod_Settings.sex_minimum_age)
			{
				//Log.Message("[RJW]Need_Sex::NeedInterval is called0 - pawn is "+pawn.NameStringShort);
				needsex_tick = needsex_tick_timer;

				if (!def.freezeWhileSleeping || pawn.Awake())
				{
					decay_rate_modifier = Mod_Settings.sexneed_decay_rate;

					//every 200 calls will have a real functioning call
					var fall_per_tick =
						//def.fallPerDay *
						decay_per_day *
						(isNympho ? 3.0f : 1.0f) *
						brokenbodyfactor(pawn) *
						sex_need_factor_from_age.Evaluate(age) *
						(isFemale ? .95f : 1.0f) /
						60000.0f;
					var fall_per_call =
						150 *
						fall_per_tick *
						needsex_tick_timer;
					CurLevel -= fall_per_call * decay_rate_modifier;
					// Each day has 60000 ticks, each hour has 2500 ticks, so each hour has 50/3 calls, in other words, each call takes .06 hour.
					//Log.Message("[RJW] " + pawn.NameStringShort + "'s sex need stats:: Decay/call: " + fall_per_call * decay_rate_modifier + ", Cur.lvl: " + CurLevel + ", Dec. rate: " + decay_rate_modifier);
				}

				// I just put this here so that it gets called on every pawn on a regular basis. There's probably a
				// better way to do this sort of thing, but whatever. This works.
				//Log.Message("[RJW]Need_Sex::NeedInterval is called1");
				std.update(pawn);

				// the bootstrap of the mapInjector will only be triggered once per visible pawn.
				if (!BootStrapTriggered)
				{
					//--Log.Message("[RJW]Need_Sex::NeedInterval::calling boostrap - pawn is " + pawn.NameStringShort);
					xxx.bootstrap(pawn.Map);
					BootStrapTriggered = true;
				}
			}
			else
			{
				needsex_tick--;
				decay_rate_modifier = Mod_Settings.sexneed_decay_rate;
			}
			//Log.Message("[RJW]Need_Sex::NeedInterval is called2 - needsex_tick is "+needsex_tick);
		}
	}
}