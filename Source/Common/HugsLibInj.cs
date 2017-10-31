using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using HugsLib;
using HugsLib.Settings;
using UnityEngine.SceneManagement;
using HugsLib.Core;

namespace rjw
{
	public class HugsLibInj : ModBase
	{

		public override string ModIdentifier
		{
			get
			{
				return "RJW";
			}
		}

		public override VersionShort GetVersion()
		{
			Logger.Message("GetVersion() called");
			return base.GetVersion();
		}

		private SettingHandle<bool> option_WildMode;
		private SettingHandle<uint> option_sexneed_decay_rate;
		private SettingHandle<bool> option_nymphs_join;
		private SettingHandle<bool> option_STD_floor_catch;
		private SettingHandle<bool> option_rape_beating;
		//private SettingHandle<bool> animals_enabled;
		//private SettingHandle<bool> comfort_prisoners_enabled;
		//private SettingHandle<bool> colonists_can_be_comfort_prisoners;
		//private SettingHandle<bool> cum_enabled;
		//private SettingHandle<bool> rape_me_sticky_enabled;
		//private SettingHandle<bool> sounds_enabled;
		//private SettingHandle<bool> stds_enabled;
		//private SettingHandle<bool> bondage_gear_enabled;
		//private SettingHandle<bool> show_regular_dick_and_vag;
		private SettingHandle<uint> option_pregnancy_weight_parent;
		private SettingHandle<uint> option_pregnancy_weight_species;
		private SettingHandle<uint> option_pregnancy_coefficient_human;
		private SettingHandle<uint> option_pregnancy_coefficient_animals;
		private SettingHandle<bool> option_pregnancy_use_parent_method;
		private SettingHandle<uint> option_sex_free_for_all_age;
		private SettingHandle<uint> option_sex_minimum_age;
		private SettingHandle<uint> option_NonFutaWomenRaping_MaxVulnerability;
		private SettingHandle<uint> option_Rapee_MinVulnerability_human;
		private SettingHandle<uint> option_Rapee_MinVulnerability_animals;

		//Default values
		//public static bool WildMode = false;
		//public static bool nymphos = true;
		//public static bool std_floor = true;
		//public static bool prisoner_beating = true;
		//public static uint pregnancy_weight_parent = 50;
		//public static uint pregnancy_weight_species = 50;
		//public static uint pregnancy_coefficient_human = 20;
		//public static uint pregnancy_coefficient_animals = 50;
		//public static bool pregnancy_use_parent_method = true;
		//public static uint sex_free_for_all_age;
		//public static uint sex_minimum_age;
		//public static float NonFutaWomenRaping_MaxVulnerability = 0.2f;
		//public static float Rapee_MinVulnerability_human = 0.5f;
		//public static float Rapee_MinVulnerability_animals = 0.4f;

		public static bool WildMode;
		public static float sexneed_decay_rate;
		public static bool nymphos;
		public static bool std_floor;
		public static bool prisoner_beating;
		public static uint pregnancy_weight_parent;
		public static uint pregnancy_weight_species;
		public static uint pregnancy_coefficient_human;
		public static uint pregnancy_coefficient_animals
		public static bool pregnancy_use_parent_method;
		public static uint sex_free_for_all_age;
		public static uint sex_minimum_age;
		public static float NonFutaWomenRaping_MaxVulnerability;
		public static float Rapee_MinVulnerability_human;
		public static float Rapee_MinVulnerability_animals;


		public override void Initialize()
		{
			Logger.Message("Initialize() called");
			base.Initialize();
		}

		public override void DefsLoaded()
		{
			Logger.Message("DefsLoaded() called");
			option_WildMode = Settings.GetHandle<bool>("WildMode", "WildMode_name".Translate(), "WildMode_desc".Translate(), false);
			option_sexneed_decay_rate = Settings.GetHandle<float>("sexneed_decay_rate", "sexneed_decay_rate_name".Translate(), "sexneed_decay_rate_desc".Translate(), 100, Validators.IntRangeValidator(0, 10000));
			option_nymphs_join = Settings.GetHandle<bool>("nymphs_join", "NymphsJoin".Translate(), "NymphsJoin_desc".Translate(), true);
			option_STD_floor_catch = Settings.GetHandle<bool>("STD_floor_catch", "STD_FromFloors".Translate(), "STD_FromFloors_desc".Translate(), true);
			option_rape_beating = Settings.GetHandle<bool>("rape_beating", "PrisonersBeating".Translate(), "PrisonersBeating_desc".Translate(), true);
			option_pregnancy_weight_parent = Settings.GetHandle<uint>("pregnancy_weight_parent", "OffspringLookLikeTheirMother".Translate(), "OffspringLookLikeTheirMother_desc".Translate(), 50, Validators.IntRangeValidator(0, 100));
			option_pregnancy_weight_species = Settings.GetHandle<uint>("pregnancy_weight_species", "OffspringIsHuman".Translate(), "OffspringIsHuman_desc".Translate(), 50, Validators.IntRangeValidator(0, 100));
			option_pregnancy_coefficient_human = Settings.GetHandle<uint>("pregnancy_coefficient_human", "PregnantCoeffecientForHuman".Translate(), "PregnantCoeffecientForHuman_desc".Translate(), 20, Validators.IntRangeValidator(0, 300));
			option_pregnancy_coefficient_animals = Settings.GetHandle<uint>("pregnancy_coefficient_animals", "PregnantCoeffecientForAnimals".Translate(), "PregnantCoeffecientForAnimals_desc".Translate(), 50, Validators.IntRangeValidator(0, 300));
			option_pregnancy_use_parent_method = Settings.GetHandle<bool>("pregnancy_use_parent_method", "UseParentMethod".Translate(), "UseParentMethod_desc".Translate(), true);
			option_sex_free_for_all_age = Settings.GetHandle<uint>("sex_free_for_all_age", "SexFreeForAllAge".Translate(), "SexFreeForAllAge_desc".Translate(), 15);
			option_sex_minimum_age = Settings.GetHandle<uint>("sex_minimum_age", "SexMinimumAge".Translate(), "SexMinimumAge_desc".Translate(), 15);
			option_NonFutaWomenRaping_MaxVulnerability = Settings.GetHandle<float>("nonFutaWomenRaping_MaxVulnerability", "NonFutaWomenRaping_MaxVulnerability".Translate(), "NonFutaWomenRaping_MaxVulnerability_desc".Translate(), 20, Validators.IntRangeValidator(0, 300));
			option_Rapee_MinVulnerability_human = Settings.GetHandle<float>("rapee_MinVulnerability_human", "Rapee_MinVulnerability_human".Translate(), "Rapee_MinVulnerability_human_desc".Translate(), 50, Validators.IntRangeValidator(0, 300));
			option_Rapee_MinVulnerability_animals = Settings.GetHandle<float>("rapee_MinVulnerability_animals", "Rapee_MinVulnerability_animals".Translate(), "Rapee_MinVulnerability_animals_desc".Translate(), 40, Validators.IntRangeValidator(0, 300));

			WildMode = option_WildMode.Value;
			sexneed_decay_rate = option_sexneed_decay_rate;
			nymphos = option_nymphs_join.Value;
			std_floor = option_STD_floor_catch.Value;
			prisoner_beating = option_rape_beating.Value;
			pregnancy_weight_parent = option_pregnancy_weight_parent.Value;
			pregnancy_weight_species = option_pregnancy_weight_species.Value;
			pregnancy_coefficient_human = option_pregnancy_coefficient_human.Value;
			pregnancy_coefficient_animals = option_pregnancy_coefficient_animals.Value;
			pregnancy_use_parent_method = option_pregnancy_use_parent_method.Value;
			sex_free_for_all_age = option_sex_free_for_all_age.Value;
			sex_minimum_age = option_sex_minimum_age.Value;
			NonFutaWomenRaping_MaxVulnerability = option_NonFutaWomenRaping_MaxVulnerability.Value;
			Rapee_MinVulnerability_human = option_Rapee_MinVulnerability_human.Value;
			Rapee_MinVulnerability_animals = option_Rapee_MinVulnerability_animals.Value;

		}

		public override void SettingsChanged()
		{
			Logger.Message("SettingsChanged() called");
			WildMode = option_WildMode.Value;
			sexneed_decay_rate = (float)(option_sexneed_decay_rate / 100);
			nymphos = option_nymphs_join.Value;
			std_floor = option_STD_floor_catch.Value;
			prisoner_beating = option_rape_beating.Value;
			pregnancy_weight_parent = option_pregnancy_weight_parent.Value;
			pregnancy_weight_species = option_pregnancy_weight_species.Value;
			pregnancy_coefficient_human = option_pregnancy_coefficient_human.Value;
			pregnancy_coefficient_animals = option_pregnancy_coefficient_animals.Value;
			pregnancy_use_parent_method = option_pregnancy_use_parent_method.Value;
			sex_free_for_all_age = option_sex_free_for_all_age.Value;
			sex_minimum_age = option_sex_minimum_age.Value;
			NonFutaWomenRaping_MaxVulnerability = (float)(option_NonFutaWomenRaping_MaxVulnerability.Value / 100);
			Rapee_MinVulnerability_human = (float)(option_Rapee_MinVulnerability_human.Value / 100);
			Rapee_MinVulnerability_animals = (float)(option_Rapee_MinVulnerability_animals.Value / 100);
		}

		public override void MapLoaded(Map map)
		{
			Logger.Message("RimJobWorld settings loaded:");
			Logger.Message("WildMode = " + (string)WildMode);
			Logger.Message("sexneed_decay_rate = " + (string)sexneed_decay_rate);
			Logger.Message("nymphos = " + (string)nymphos);
			Logger.Message("std_floor = " + (string)std_floor);
			Logger.Message("prisoner_beating = " + (string)prisoner_beating);
			Logger.Message("pregnancy_weight_parent = " + (string)pregnancy_weight_parent);
			Logger.Message("pregnancy_weight_species = " + (string)pregnancy_weight_species);
			Logger.Message("pregnancy_coefficient_human = " + (string)pregnancy_coefficient_human);
			Logger.Message("pregnancy_coefficient_animals = " + (string)pregnancy_coefficient_animals);
			Logger.Message("pregnancy_use_parent_method = " + (string)pregnancy_use_parent_method);
			Logger.Message("sex_free_for_all_age = " + (string)sex_free_for_all_age);
			Logger.Message("sex_minimum_age = " + (string)sex_minimum_age);
			Logger.Message("NonFutaWomenRaping_MaxVulnerability = " + (string)NonFutaWomenRaping_MaxVulnerability);
			Logger.Message("Rapee_MinVulnerability_human = " + (string)Rapee_MinVulnerability_human);
			Logger.Message("Rapee_MinVulnerability_animals = " + (string)Rapee_MinVulnerability_animals);
			base.MapLoaded(map);
		}

		/*
		public override void Update() {
			base.Update();
		}

        public override void FixedUpdate() {
            base.FixedUpdate();
        }
      
        public override void MapComponentsInitializing(Map map) {
            Logger.Message("MapComponentsInitializing() called");
            base.MapComponentsInitializing(map);
        }
     
        public override void MapDiscarded(Map map) {
            Logger.Message("MapDiscarded() called");
            base.MapDiscarded(map);
        }
     
        public override void MapGenerated(Map map) {
            Logger.Message("MapGenerated() called");
            base.MapGenerated(map);
        }
        
        public override void MapLoaded(Map map) {
            Logger.Message("MapLoaded() called");
            base.MapLoaded(map);
        }
     
        public override void OnGUI() {
            base.OnGUI();
        }
       
        public override void SceneLoaded(Scene scene) {
            Logger.Message("SceneLoaded() called");
            base.SceneLoaded(scene);
        }
     
        public override void Tick(int currentTick) {
            base.Tick(currentTick);
        }
      
        public override void WorldLoaded() {
            Logger.Message("WorldLoaded() called");
            base.WorldLoaded();
        }
        */

	}
}
