﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Outfitted
{
    public class ExtendedOutfit : Outfit, IExposable
    {
        public bool targetTemperaturesOverride;
        public FloatRange targetTemperatures = new FloatRange(-100, 100);

        public bool PenaltyWornByCorpse = true;

        static IEnumerable<StatCategoryDef> blacklistedCategories = new List<StatCategoryDef>()
        {
            StatCategoryDefOf.BasicsNonPawn,
            StatCategoryDefOf.Building,
            StatCategoryDefOf.StuffStatFactors,
        };

        static readonly IEnumerable<StatDef> blacklistedStats = new List<StatDef> {
            StatDefOf.ComfyTemperatureMin,
            StatDefOf.ComfyTemperatureMax,
            StatDefOf.Insulation_Cold,
            StatDefOf.Insulation_Heat,
            StatDefOf.StuffEffectMultiplierInsulation_Cold,
            StatDefOf.StuffEffectMultiplierInsulation_Heat,
            StatDefOf.StuffEffectMultiplierArmor,
        };

        internal static IEnumerable<StatDef> AllAvailableStats => DefDatabase<StatDef>
            .AllDefs
            .Where(i => !blacklistedCategories.Contains(i.category))
            .Except(blacklistedStats).ToList();

        public IEnumerable<StatDef> UnassignedStats => AllAvailableStats
            .Except(StatPriorities.Select(i => i.Stat));

        List<StatPriority> statPriorities = new List<StatPriority>();

        public IEnumerable<StatPriority> StatPriorities => statPriorities;

        public ExtendedOutfit(int uniqueId, string label) : base(uniqueId, label)
        {
            // Used by OutfitDatabase_MakeNewOutfit_Patch
        }

        public ExtendedOutfit(Outfit outfit) : base(outfit.uniqueId, outfit.label)
        {
            // Used by OutfitDatabase_ExposeData_Patch

            filter.CopyAllowancesFrom(outfit.filter);
        }

        public ExtendedOutfit()
        {
            // Used by ExposeData
        }

        [UnofficialMultiplayerAPI.SyncMethod]
        public void AddStatPriority(StatDef def, float priority, float defaultPriority = float.NaN)
        {
            statPriorities.Insert(0, new StatPriority(def, priority, defaultPriority));
        }

        public void AddRange(IEnumerable<StatPriority> priorities) {
            statPriorities.AddRange(priorities);
        }

        [UnofficialMultiplayerAPI.SyncMethod]
        public void RemoveStatPriority(StatDef def)
        {
            statPriorities.RemoveAll(i => i.Stat == def);
        }

        public bool AutoWorkPriorities;

        new public void ExposeData()
        {
            Scribe_Values.Look(ref uniqueId, "uniqueId");
            Scribe_Values.Look(ref label, "label");
            Scribe_Deep.Look(ref filter, "filter", new object[0]);
            Scribe_Values.Look(ref targetTemperaturesOverride, "targetTemperaturesOverride");
            Scribe_Values.Look(ref targetTemperatures, "targetTemperatures");
            Scribe_Values.Look(ref PenaltyWornByCorpse, "PenaltyWornByCorpse", true);
            Scribe_Collections.Look(ref statPriorities, "statPriorities", LookMode.Deep);
            Scribe_Values.Look(ref AutoWorkPriorities, "AutoWorkPriorities", false );
        }
    }
}