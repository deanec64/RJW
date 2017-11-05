
using System;
using System.Collections.Generic;

using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace rjw
{
	public class ABFCommon
	{
		
		public static bool is_Insect(Pawn pawn)
		{
			if (pawn.def == ThingDef.Named("Megascarab"))
			{
				return true;
			}
			if (pawn.def == ThingDef.Named("Megaspider"))
			{
				return true;
			}
			if (pawn.def == ThingDef.Named("Spelopede"))
			{
				return true;
			}
			return false;
		}
		public static float would_fuck_ignoreSatisfy(Pawn fucker, Pawn p, bool invert_opinion = false)
		{
			if (xxx.is_mechanoid(fucker) && xxx.is_female(p) && xxx.is_human(p))
			{
				return 1f; // Need some value
			}
			int ageBiologicalYears = fucker.ageTracker.AgeBiologicalYears;
			int ageBiologicalYears2 = p.ageTracker.AgeBiologicalYears;
			bool flag = (xxx.is_animal(fucker) || xxx.is_animal(p)) && !xxx.config.animals_enabled;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				bool flag2 = xxx.is_animal(fucker) && (long)ageBiologicalYears2 >= (long)((ulong)ModSettings.sex_free_for_all_age);
				bool flag3;
				if (flag2)
				{
					flag3 = true;
				}
				else
				{
					bool flag4 = xxx.is_animal(p) && (long)ageBiologicalYears >= (long)((ulong)ModSettings.sex_free_for_all_age);
					if (flag4)
					{
						flag3 = true;
					}
					else
					{
						bool flag5 = (long)ageBiologicalYears >= (long)((ulong)ModSettings.sex_free_for_all_age) && (long)ageBiologicalYears2 >= (long)((ulong)ModSettings.sex_free_for_all_age);
						if (flag5)
						{
							flag3 = true;
						}
						else
						{
							bool flag6 = (long)ageBiologicalYears < (long)((ulong)ModSettings.sex_minimum_age) || (long)ageBiologicalYears2 < (long)((ulong)ModSettings.sex_minimum_age);
							flag3 = (!flag6 && Math.Abs(fucker.ageTracker.AgeBiologicalYearsFloat - p.ageTracker.AgeBiologicalYearsFloat) < 2.05f);
						}
					}
				}
				bool flag7 = flag3;
				if (flag7)
				{
					bool flag8 = !fucker.Dead && (!p.Dead | (xxx.is_necrophiliac(fucker) && p.Dead)) && !p.IsBurning();
					if (flag8)
					{
						Gender gender = (!xxx.is_gay(fucker)) ? xxx.opposite_gender(fucker.gender) : fucker.gender;
						bool flag9 = xxx.is_asexual(fucker);
						float num;
						if (flag9)
						{
							num = 0f;
						}
						else
						{
							bool flag10 = xxx.is_bisexual(fucker);
							if (flag10)
							{
								num = 1f;
							}
							else
							{
								bool flag11 = p.gender == gender;
								if (flag11)
								{
									num = 1f;
								}
								else
								{
									num = 0.1f;
								}
							}
						}
						float num2 = 1f;
						bool flag12 = xxx.is_animal(fucker);
						if (flag12)
						{
							num2 = 1f;
						}
						else
						{
							bool flag13 = xxx.is_animal(p);
							if (flag13)
							{
								bool flag14 = !xxx.is_zoophiliac(fucker);
								if (flag14)
								{
									num2 *= 0.1f;
								}
							}
						}
						bool flag15 = p.story != null;
						float num3;
						if (flag15)
						{
							bool flag16 = p.story.bodyType == BodyType.Female;
							if (flag16)
							{
								num3 = 1.25f;
							}
							else
							{
								bool flag17 = p.story.bodyType == BodyType.Fat;
								if (flag17)
								{
									num3 = 1f;
								}
								else
								{
									num3 = 1.1f;
								}
							}
							bool flag18 = RelationsUtility.IsDisfigured(p);
							if (flag18)
							{
								num3 *= 0.8f;
							}
						}
						else
						{
							num3 = 1.25f;
						}
						bool flag19 = p.story != null;
						float num5;
						if (flag19)
						{
							int num4 = p.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
							num5 = 1f + 0.15f * (float)num4;
						}
						else
						{
							num5 = 1f;
						}
						bool flag20 = p.relations != null;
						float num7;
						if (flag20)
						{
							float num6 = (float)((!invert_opinion) ? fucker.relations.OpinionOf(p) : checked(100 - fucker.relations.OpinionOf(p)));
							num7 = 0.8f + (num6 + 100f) * 0.00225f;
						}
						else
						{
							num7 = 1f;
						}
						float num8 = xxx.need_some_sex(fucker);
						float num9 = num8;
						float obj = num9;
						float num10;
						if (!3f.Equals(obj))
						{
							if (!2f.Equals(obj))
							{
								if (!1f.Equals(obj))
								{
									num10 = 1f;
								}
								else
								{
									num10 = 1.1f;
								}
							}
							else
							{
								num10 = 1.3f;
							}
						}
						else
						{
							num10 = 1.5f;
						}
						float num11 = xxx.get_vulnerability(fucker);
						float num12 = xxx.get_vulnerability(p);
						bool flag21 = num11 > num12;
						float num13;
						if (flag21)
						{
							num13 = 0.5f;
						}
						else
						{
							bool flag22 = xxx.is_masochist(p) || xxx.is_rapist(fucker) || xxx.is_bloodlust(fucker) || xxx.is_psychopath(fucker) || xxx.is_nympho(fucker) || (xxx.is_zoophiliac(fucker) && xxx.is_animal(p));
							if (flag22)
							{
								num13 = 2f + 0.5f * Mathf.InverseLerp(num11, 3f, num12);
							}
							else
							{
								num13 = 1.5f + 0.5f * Mathf.InverseLerp(num11, 3f, num12);
							}
						}
						float num14 = Mathf.InverseLerp(0f, 4f, 0.6f * num * num2 * num3 * num5 * num7 * num10 * num13);
						float num15 = (!xxx.is_nympho(fucker)) ? num14 : (0.2f + 0.8f * num14);
						/*Log.Message(string.Concat(new string[]
						{
					"would_fuck( ",
					fucker.NameStringShort,
					", ",
					p.NameStringShort,
					" ) - prenymph_att = ",
					num14.ToString(),
					", final_att = ",
					num15.ToString()
						}));*/
						result = num15;
					}
					else
					{
						result = 0f;
					}
				}
				else
				{
					result = 0f;
				}
			}
			return result;
		}
	}
}
