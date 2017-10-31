
using System;
using System.Collections.Generic;

using Verse;
using RimWorld;

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
		private static int needsex_tick_timer = 150;   
		
		//private int std_tick = 1;

		private static readonly SimpleCurve sex_need_factor_from_age = new SimpleCurve
        {
            /* Edited by nizhuan-jjr: This is the old unrealistic curve， I use a more realistic curve
            new CurvePoint(5f,  0.25f),
  			new CurvePoint(16f, 1.00f),
  			new CurvePoint(22f, 1.00f),
  			new CurvePoint(30f, 0.90f),
 			new CurvePoint(40f, 0.75f),
 			new CurvePoint(60f, 0.50f),
  			new CurvePoint(80f, 0.25f)
            */
            new CurvePoint(5f,  0f),
			new CurvePoint(12f, 0.5f),
            new CurvePoint(14f, 0.75f),
            new CurvePoint(17f, 1.00f),
            new CurvePoint(28f, 1.00f),
            new CurvePoint(30f, 0.90f),
            new CurvePoint(40f, 0.75f),
            new CurvePoint(50f, 0.50f),
			new CurvePoint(60f, 0.20f),
  			new CurvePoint(80f, 0f),
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

        public float thresh_frustrated() { return 0.10f; }

        public float thresh_horny() { return (pawn.gender == Gender.Male) ? 0.50f : 0.25f; }

        public float thresh_satisfied() { return 0.75f; }

        public float thresh_ahegao() { return 0.95f; }

        public Need_Sex(Pawn pawn) : base(pawn)
        {
            if (xxx.is_mechanoid(pawn)) return; //Added by nizhuan-jjr:Misc.Robots are not allowed to have sex, so they don't need sex actually.
            threshPercents = new List<float>
            {
                thresh_frustrated(),
                thresh_horny(),
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

        public override void NeedInterval()
        {
            if (isInvisible) return;
            if (needsex_tick <= 0)
            {
                //Log.Message("[RJW]Need_Sex::NeedInterval is called0 - pawn is "+pawn.NameStringShort);
                needsex_tick = needsex_tick_timer;

                if (!def.freezeWhileSleeping || pawn.Awake())
                {
                    float age = pawn.ageTracker.AgeBiologicalYearsFloat;
					float decay_per_day = 0.5;

					//every 200 calls will have a real functioning call
					var fall_per_tick =
						//def.fallPerDay *
						decay_per_day *
						(isNympho ? 3.0f : 1.0f) * 
						brokenbodyfactor(pawn) * 
						sex_need_factor_from_age.Evaluate(age) * 
						(isFemale ? .95f : 1.0f) / 
						60000.0f;
                    CurLevel -= fall_per_tick * needsex_tick_timer * HugsLibInj.sexneed_decay_rate; // 150 ticks between each call, each day has 60000 ticks, each hour has 2500 ticks, so each hour has 50/3 calls, in other words, each call takes .06 hour.
                }
                
                // I just put this here so that it gets called on every pawn on a regular basis. There's probably a
                // better way to do this sort of thing, but whatever. This works.
                //Log.Message("[RJW]Need_Sex::NeedInterval is called1");
                std.update(pawn);


                // the bootstrap of the mapInjector will only be triggered once per visible pawn.
                if (!BootStrapTriggered)
                {
                    Log.Message("[RJW]Need_Sex::NeedInterval::calling boostrap - pawn is "+pawn.NameStringShort);
                    xxx.bootstrap(pawn.Map);
                    BootStrapTriggered = true;
                }

            }
            else
                needsex_tick--;
                //Log.Message("[RJW]Need_Sex::NeedInterval is called2 - needsex_tick is "+needsex_tick);
        }
    }
}