
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI;
using Verse.Sound;



namespace rjw
{
	class JobDef_RapeEnemy : JobDef
	{
		public List<string> TargetDefNames = new List<string>();
		public float targetAcquireRadius = 20f;
		public int priority = 0;
		protected JobDriver_RapeEnemy intance
		{
			get
			{
				if (_tmpInstance == null)
				{
					_tmpInstance = (JobDriver_RapeEnemy)Activator.CreateInstance(this.driverClass);
				}
				return _tmpInstance;
			}
		}
		private JobDriver_RapeEnemy _tmpInstance;

		public virtual bool CanUseThisJobForPawn(Pawn rapist)
		{
			return intance.CanUseThisJobForPawn(rapist) || TargetDefNames.Contains(rapist.def.defName);
		}
		public virtual Pawn FindVictim(Pawn rapist, Map m)
		{
			return intance.FindVictim(rapist, m, targetAcquireRadius);
		}

	}
}
