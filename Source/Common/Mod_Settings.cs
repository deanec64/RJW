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
	public class Mod_Settings : ModBase
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
			//Log.Message("GetVersion() called");
			return base.GetVersion();
		}

		public static bool WildMode;
		public static float sexneed_decay_rate;
		public static bool nymphos;
		public static bool std_floor;
		public static bool prisoner_beating;
		public static uint pregnancy_weight_parent; //to-do: convert 'em into float
		public static uint pregnancy_weight_species;
		public static uint pregnancy_coefficient_human;
		public static uint pregnancy_coefficient_animals;
		public static bool pregnancy_use_parent_method;
		public static uint sex_free_for_all_age;
		public static uint sex_minimum_age;
		public static float NonFutaWomenRaping_MaxVulnerability;
		public static float Rapee_MinVulnerability_human;
		public static float Rapee_MinVulnerability_animals;

		// Feature Toggles
		public bool animals_enabled; // New
		public bool comfort_prisoners_enabled; // New
		public bool colonists_can_be_comfort_prisoners; // New
		public bool cum_enabled; // New
		public bool rape_me_sticky_enabled; // New
		public bool sounds_enabled; // New
		public bool stds_enabled; // New
		public bool bondage_gear_enabled; // New
		public bool nymph_joiners_enabled; // New
		public bool whore_beds_enabled; // New
		public bool necro_enabled; // New
		public bool beastiality_enabled; // New
		public bool random_rape_enabled; // New
		public bool always_accept_whores; // New
		public bool nymphs_always_JoinInBed; // New
		public bool zoophis_always_rape; // New
		public bool rapists_always_rape; // New
		public bool pawns_always_do_fapping; // New
		public bool pawns_always_rapeCP; // New
		public bool whores_always_findjob; // New

		// Display Toggles
		public bool show_regular_dick_and_vag; // New

		// STD config
		public bool std_show_roll_to_catch; // New
		public float std_min_severity_to_pitch; // New
		public float std_env_pitch_cleanliness_exaggeration; // New
		public float std_env_pitch_dirtiness_exaggeration; // New
		public float std_outdoor_cleanliness; // New

		// Age Config
		//public int sex_free_for_all_age;
		//public int sex_minimum_age;

		public float significant_pain_threshold;    // 0.6
		public float extreme_pain_threshold;        // 0.8
		public float base_chance_to_hit_prisoner;   // 50
		public int min_ticks_between_hits;          // 500
		public int max_ticks_between_hits;          // 700

		public float max_nymph_fraction;
		public float opp_inf_initial_immunity;
		public float comfort_prisoner_rape_mtbh_mul;
		public float whore_mtbh_mul;
		public float nymph_spawn_with_std_mul;

		//Mod Settings handles
		private SettingHandle<bool> option_WildMode;
		private SettingHandle<int> option_sexneed_decay_rate;
		private SettingHandle<bool> option_nymphs_join;
		private SettingHandle<bool> option_STD_floor_catch;
		private SettingHandle<bool> option_rape_beating;
		private SettingHandle<int> option_pregnancy_weight_parent;
		private SettingHandle<int> option_pregnancy_weight_species;
		private SettingHandle<int> option_pregnancy_coefficient_human;
		private SettingHandle<int> option_pregnancy_coefficient_animals;
		private SettingHandle<bool> option_pregnancy_use_parent_method;
		private SettingHandle<int> option_sex_free_for_all_age;
		private SettingHandle<int> option_sex_minimum_age;
		private SettingHandle<int> option_NonFutaWomenRaping_MaxVulnerability;
		private SettingHandle<int> option_Rapee_MinVulnerability_human;
		private SettingHandle<int> option_Rapee_MinVulnerability_animals;

		public override void Initialize()
		{
			Logger.Message("Initialize() called");
			base.Initialize();
		}

		public override void DefsLoaded()
		{
			Log.Message("DefsLoaded() called");

			option_WildMode = Settings.GetHandle<bool>("WildMode", "WildMode_name".Translate(), "WildMode_desc".Translate(), false);
			option_sexneed_decay_rate = Settings.GetHandle<int>("sexneed_decay_rate", "sexneed_decay_rate_name".Translate(), "sexneed_decay_rate_desc".Translate(), 100, Validators.IntRangeValidator(0, 1000000));
			option_sexneed_decay_rate.SpinnerIncrement = 25;
			option_nymphs_join = Settings.GetHandle<bool>("nymphs_join", "NymphsJoin".Translate(), "NymphsJoin_desc".Translate(), true);
			option_STD_floor_catch = Settings.GetHandle<bool>("STD_floor_catch", "STD_FromFloors".Translate(), "STD_FromFloors_desc".Translate(), true);
			option_rape_beating = Settings.GetHandle<bool>("rape_beating", "PrisonersBeating".Translate(), "PrisonersBeating_desc".Translate(), true);
			option_pregnancy_weight_parent = Settings.GetHandle<int>("pregnancy_weight_parent", "OffspringLookLikeTheirMother".Translate(), "OffspringLookLikeTheirMother_desc".Translate(), 50, Validators.IntRangeValidator(0, 100));
			option_pregnancy_weight_parent.SpinnerIncrement = 10;
			option_pregnancy_weight_species = Settings.GetHandle<int>("pregnancy_weight_species", "OffspringIsHuman".Translate(), "OffspringIsHuman_desc".Translate(), 50, Validators.IntRangeValidator(0, 100));
			option_pregnancy_weight_species.SpinnerIncrement = 10;
			option_pregnancy_coefficient_human = Settings.GetHandle<int>("pregnancy_coefficient_human", "PregnantCoeffecientForHuman".Translate(), "PregnantCoeffecientForHuman_desc".Translate(), 20, Validators.IntRangeValidator(0, 300));
			option_pregnancy_coefficient_human.SpinnerIncrement = 10;
			option_pregnancy_coefficient_animals = Settings.GetHandle<int>("pregnancy_coefficient_animals", "PregnantCoeffecientForAnimals".Translate(), "PregnantCoeffecientForAnimals_desc".Translate(), 50, Validators.IntRangeValidator(0, 300));
			option_pregnancy_coefficient_animals.SpinnerIncrement = 10;
			option_pregnancy_use_parent_method = Settings.GetHandle<bool>("pregnancy_use_parent_method", "UseParentMethod".Translate(), "UseParentMethod_desc".Translate(), true);
			option_sex_free_for_all_age = Settings.GetHandle<int>("sex_free_for_all_age", "SexFreeForAllAge".Translate(), "SexFreeForAllAge_desc".Translate(), 15, Validators.IntRangeValidator(0, 9999));
			option_sex_free_for_all_age.SpinnerIncrement = 1;
			option_sex_minimum_age = Settings.GetHandle<int>("sex_minimum_age", "SexMinimumAge".Translate(), "SexMinimumAge_desc".Translate(), 15, Validators.IntRangeValidator(0, 9999));
			option_sex_minimum_age.SpinnerIncrement = 1;
			option_NonFutaWomenRaping_MaxVulnerability = Settings.GetHandle<int>("nonFutaWomenRaping_MaxVulnerability", "NonFutaWomenRaping_MaxVulnerability".Translate(), "NonFutaWomenRaping_MaxVulnerability_desc".Translate(), 20, Validators.IntRangeValidator(0, 300));
			option_NonFutaWomenRaping_MaxVulnerability.SpinnerIncrement = 10;
			option_Rapee_MinVulnerability_human = Settings.GetHandle<int>("rapee_MinVulnerability_human", "Rapee_MinVulnerability_human".Translate(), "Rapee_MinVulnerability_human_desc".Translate(), 50, Validators.IntRangeValidator(0, 300));
			option_Rapee_MinVulnerability_human.SpinnerIncrement = 10;
			option_Rapee_MinVulnerability_animals = Settings.GetHandle<int>("rapee_MinVulnerability_animals", "Rapee_MinVulnerability_animals".Translate(), "Rapee_MinVulnerability_animals_desc".Translate(), 40, Validators.IntRangeValidator(0, 300));
			option_Rapee_MinVulnerability_animals.SpinnerIncrement = 10;

			MakeSettingsCategoryToggle

			this.SettingsChanged();
		}

		public override void SettingsChanged()
		{
			base.SettingsChanged();
			WildMode = option_WildMode.get_Value;
			sexneed_decay_rate = (float)(option_sexneed_decay_rate.get_Value / 100);
			nymphos = option_nymphs_join.get_Value;
			std_floor = option_STD_floor_catch.get_Value;
			prisoner_beating = option_rape_beating.get_Value;
			pregnancy_weight_parent = (uint)option_pregnancy_weight_parent.get_Value;
			pregnancy_weight_species = (uint)option_pregnancy_weight_species.get_Value;
			pregnancy_coefficient_human = (uint)option_pregnancy_coefficient_human.get_Value;
			pregnancy_coefficient_animals = (uint)option_pregnancy_coefficient_animals.get_Value;
			pregnancy_use_parent_method = option_pregnancy_use_parent_method.get_Value;
			sex_free_for_all_age = (uint)option_sex_free_for_all_age.get_Value;
			sex_minimum_age = (uint)option_sex_minimum_age.get_Value;
			NonFutaWomenRaping_MaxVulnerability = (float)(option_NonFutaWomenRaping_MaxVulnerability.get_Value / 100);
			Rapee_MinVulnerability_human = (float)(option_Rapee_MinVulnerability_human.get_Value / 100);
			Rapee_MinVulnerability_animals = (float)(option_Rapee_MinVulnerability_animals.get_Value / 100);

			Log.Message("[RJW] Settings Changed:");
			Log.Message("WildMode = " + WildMode);
			Log.Message("sexneed_decay_rate = " + sexneed_decay_rate);
			Log.Message("nymphos = " + nymphos);
			Log.Message("std_floor = " + std_floor);
			Log.Message("prisoner_beating = " + prisoner_beating);
			Log.Message("pregnancy_weight_parent = " + pregnancy_weight_parent);
			Log.Message("pregnancy_weight_species = " + pregnancy_weight_species);
			Log.Message("pregnancy_coefficient_human = " + pregnancy_coefficient_human);
			Log.Message("pregnancy_coefficient_animals = " + pregnancy_coefficient_animals);
			Log.Message("pregnancy_use_parent_method = " + pregnancy_use_parent_method);
			Log.Message("sex_free_for_all_age = " + sex_free_for_all_age);
			Log.Message("sex_minimum_age = " + sex_minimum_age);
			Log.Message("NonFutaWomenRaping_MaxVulnerability = " + NonFutaWomenRaping_MaxVulnerability);
			Log.Message("Rapee_MinVulnerability_human = " + Rapee_MinVulnerability_human);
			Log.Message("Rapee_MinVulnerability_animals = " + Rapee_MinVulnerability_animals);

		}

		
		public override void MapLoaded(Map map)
		{
			Log.Message("[RJW] Settings loaded:");
			Log.Message("WildMode = " + WildMode);
			Log.Message("sexneed_decay_rate = " + sexneed_decay_rate);
			Log.Message("nymphos = " + nymphos);
			Log.Message("std_floor = " + std_floor);
			Log.Message("prisoner_beating = " + prisoner_beating);
			Log.Message("pregnancy_weight_parent = " + pregnancy_weight_parent);
			Log.Message("pregnancy_weight_species = " + pregnancy_weight_species);
			Log.Message("pregnancy_coefficient_human = " + pregnancy_coefficient_human);
			Log.Message("pregnancy_coefficient_animals = " + pregnancy_coefficient_animals);
			Log.Message("pregnancy_use_parent_method = " + pregnancy_use_parent_method);
			Log.Message("sex_free_for_all_age = " + sex_free_for_all_age);
			Log.Message("sex_minimum_age = " + sex_minimum_age);
			Log.Message("NonFutaWomenRaping_MaxVulnerability = " + NonFutaWomenRaping_MaxVulnerability);
			Log.Message("Rapee_MinVulnerability_human = " + Rapee_MinVulnerability_human);
			Log.Message("Rapee_MinVulnerability_animals = " + Rapee_MinVulnerability_animals);
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
		private void MakeSettingsCategoryToggle(string labelId, Action buttonAction)
		{
			var toolToggle = Settings.GetHandle<bool>(labelId, labelId.Translate(), null);
			toolToggle.Unsaved = true;
			toolToggle.CustomDrawer = rect => {
				if (Widgets.ButtonText(rect, "setting_showToggles_btn".Translate())) buttonAction();
				return false;
			};
		}
	}
}
