using HugsLib;
using HugsLib.Core;
using HugsLib.Settings;
using Verse;

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
		public bool animals_enabled; // Updated

		public bool comfort_prisoners_enabled; // Updated
		public bool ComfortColonist; // New
		public bool ComfortAnimal; // New
		public bool cum_enabled; // Updated
		public bool rape_me_sticky_enabled; // Updated
		public bool sounds_enabled; // Updated
		public bool stds_enabled; // Updated
		public bool bondage_gear_enabled; // Updated
		public bool nymph_joiners_enabled; // Updated
		public bool whore_beds_enabled; // Updated
		public bool necro_enabled; // Updated
		public bool beastiality_enabled; // Updated
		public bool random_rape_enabled; // Updated
		public bool always_accept_whores; // Updated
		public bool nymphs_always_JoinInBed; // Updated
		public bool zoophis_always_rape; // Updated
		public bool rapists_always_rape; // Updated
		public bool pawns_always_do_fapping; // Updated
		public bool pawns_always_rapeCP; // Updated
		public bool whores_always_findjob; // Updated

		// Display Toggles
		public bool show_regular_dick_and_vag; // Updated

		// STD config
		public bool std_show_roll_to_catch; // Updated

		public float std_min_severity_to_pitch; // Updated
		public float std_env_pitch_cleanliness_exaggeration; // Updated
		public float std_env_pitch_dirtiness_exaggeration; // Updated
		public float std_outdoor_cleanliness; // Updated

		// Age Config

		public float significant_pain_threshold; // Updated
		public float extreme_pain_threshold; // Updated
		public float base_chance_to_hit_prisoner; // Updated
		public int min_ticks_between_hits; // Updated
		public int max_ticks_between_hits; // Updated

		public float max_nymph_fraction; // Updated
		public float opp_inf_initial_immunity; // Updated
		public float comfort_prisoner_rape_mtbh_mul; // Updated
		public float whore_mtbh_mul; // Updated
		public float nymph_spawn_with_std_mul; // Updated

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

			this.option_WildMode = Settings.GetHandle<bool>("WildMode", "WildMode_name".Translate(), "WildMode_desc".Translate(), false);
			this.option_sexneed_decay_rate = Settings.GetHandle<int>("sexneed_decay_rate", "sexneed_decay_rate_name".Translate(), "sexneed_decay_rate_desc".Translate(), 100, Validators.IntRangeValidator(0, 1000000));
			this.option_sexneed_decay_rate.SpinnerIncrement = 25;
			this.option_nymphs_join = Settings.GetHandle<bool>("nymphs_join", "NymphsJoin".Translate(), "NymphsJoin_desc".Translate(), true);
			this.option_STD_floor_catch = Settings.GetHandle<bool>("STD_floor_catch", "STD_FromFloors".Translate(), "STD_FromFloors_desc".Translate(), true);
			this.option_rape_beating = Settings.GetHandle<bool>("rape_beating", "PrisonersBeating".Translate(), "PrisonersBeating_desc".Translate(), true);
			this.option_pregnancy_weight_parent = Settings.GetHandle<int>("pregnancy_weight_parent", "OffspringLookLikeTheirMother".Translate(), "OffspringLookLikeTheirMother_desc".Translate(), 50, Validators.IntRangeValidator(0, 100));
			this.option_pregnancy_weight_parent.SpinnerIncrement = 10;
			this.option_pregnancy_weight_species = Settings.GetHandle<int>("pregnancy_weight_species", "OffspringIsHuman".Translate(), "OffspringIsHuman_desc".Translate(), 50, Validators.IntRangeValidator(0, 100));
			this.option_pregnancy_weight_species.SpinnerIncrement = 10;
			this.option_pregnancy_coefficient_human = Settings.GetHandle<int>("pregnancy_coefficient_human", "PregnantCoeffecientForHuman".Translate(), "PregnantCoeffecientForHuman_desc".Translate(), 20, Validators.IntRangeValidator(0, 300));
			this.option_pregnancy_coefficient_human.SpinnerIncrement = 10;
			this.option_pregnancy_coefficient_animals = Settings.GetHandle<int>("pregnancy_coefficient_animals", "PregnantCoeffecientForAnimals".Translate(), "PregnantCoeffecientForAnimals_desc".Translate(), 50, Validators.IntRangeValidator(0, 300));
			this.option_pregnancy_coefficient_animals.SpinnerIncrement = 10;
			this.option_pregnancy_use_parent_method = Settings.GetHandle<bool>("pregnancy_use_parent_method", "UseParentMethod".Translate(), "UseParentMethod_desc".Translate(), true);
			this.option_sex_free_for_all_age = Settings.GetHandle<int>("sex_free_for_all_age", "SexFreeForAllAge".Translate(), "SexFreeForAllAge_desc".Translate(), 15, Validators.IntRangeValidator(0, 9999));
			this.option_sex_free_for_all_age.SpinnerIncrement = 1;
			this.option_sex_minimum_age = Settings.GetHandle<int>("sex_minimum_age", "SexMinimumAge".Translate(), "SexMinimumAge_desc".Translate(), 15, Validators.IntRangeValidator(0, 9999));
			this.option_sex_minimum_age.SpinnerIncrement = 1;
			this.option_NonFutaWomenRaping_MaxVulnerability = Settings.GetHandle<int>("nonFutaWomenRaping_MaxVulnerability", "NonFutaWomenRaping_MaxVulnerability".Translate(), "NonFutaWomenRaping_MaxVulnerability_desc".Translate(), 20, Validators.IntRangeValidator(0, 300));
			this.option_NonFutaWomenRaping_MaxVulnerability.SpinnerIncrement = 10;
			this.option_Rapee_MinVulnerability_human = Settings.GetHandle<int>("rapee_MinVulnerability_human", "Rapee_MinVulnerability_human".Translate(), "Rapee_MinVulnerability_human_desc".Translate(), 50, Validators.IntRangeValidator(0, 300));
			this.option_Rapee_MinVulnerability_human.SpinnerIncrement = 10;
			this.option_Rapee_MinVulnerability_animals = Settings.GetHandle<int>("rapee_MinVulnerability_animals", "Rapee_MinVulnerability_animals".Translate(), "Rapee_MinVulnerability_animals_desc".Translate(), 40, Validators.IntRangeValidator(0, 300));
			this.option_Rapee_MinVulnerability_animals.SpinnerIncrement = 10;

			this.SettingsChanged();
		}

		public override void SettingsChanged()
		{
			base.SettingsChanged();
			WildMode = option_WildMode.Value;
			sexneed_decay_rate = (float)(option_sexneed_decay_rate.Value / 100);
			nymphos = option_nymphs_join.Value;
			std_floor = option_STD_floor_catch.Value;
			prisoner_beating = option_rape_beating.Value;
			pregnancy_weight_parent = (uint)option_pregnancy_weight_parent.Value;
			pregnancy_weight_species = (uint)option_pregnancy_weight_species.Value;
			pregnancy_coefficient_human = (uint)option_pregnancy_coefficient_human.Value;
			pregnancy_coefficient_animals = (uint)option_pregnancy_coefficient_animals.Value;
			pregnancy_use_parent_method = option_pregnancy_use_parent_method.Value;
			sex_free_for_all_age = (uint)option_sex_free_for_all_age.Value;
			sex_minimum_age = (uint)option_sex_minimum_age.Value;
			NonFutaWomenRaping_MaxVulnerability = (float)(option_NonFutaWomenRaping_MaxVulnerability.Value / 100);
			Rapee_MinVulnerability_human = (float)(option_Rapee_MinVulnerability_human.Value / 100);
			Rapee_MinVulnerability_animals = (float)(option_Rapee_MinVulnerability_animals.Value / 100);

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
        

		private void MakeSettingsCategoryToggle(string labelId, Action buttonAction)
		{
			var toolToggle = Settings.GetHandle<bool>(labelId, labelId.Translate(), null);
			toolToggle.Unsaved = true;
			toolToggle.CustomDrawer = rect =>
			{
				if (Widgets.ButtonText(rect, "setting_showToggles_btn".Translate())) buttonAction();
				return false;
			};
		}
		*/
	}
}