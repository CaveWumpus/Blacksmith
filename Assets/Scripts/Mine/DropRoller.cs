using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class DropRoller
{
    public static DropResult Roll(
        RockDefinition rock,
        MineGenerationContext ctx,
        List<VeinDefinition> orePool,
        List<VeinDefinition> gemPool,
        List<RelicDefinition> relicPool,
        List<RecipeDefinition> recipePool,
        List<PatternDefinition> patternPool)
    {
        // -----------------------------
        // 1. Apply biome multipliers
        // -----------------------------
        float oreChance        = rock.chanceRegularOre   * ctx.biome.oreMultiplier;
        float rareOreChance    = rock.chanceRareOre      * ctx.biome.oreMultiplier;
        float exoticOreChance  = rock.chanceExoticOre    * ctx.biome.oreMultiplier;

        float gemChance        = rock.chanceRegularGem   * ctx.biome.gemMultiplier;
        float rareGemChance    = rock.chanceRareGem      * ctx.biome.gemMultiplier;
        float exoticGemChance  = rock.chanceExoticGem    * ctx.biome.gemMultiplier;

        float relicChance      = rock.chanceRelic        * ctx.biome.relicMultiplier;
        float recipeChance     = rock.chanceRecipe       * ctx.biome.recipeMultiplier;
        float patternChance    = rock.chancePattern      * ctx.biome.patternMultiplier;

        float nothingChance    = rock.chanceNothing;

        // -----------------------------
        // 2. Build weighted table
        // -----------------------------
        var table = new List<(float weight, System.Func<DropResult> action)>();

        table.Add((oreChance,       () => RollOre(orePool,  RarityTier.Regular)));
        table.Add((rareOreChance,   () => RollOre(orePool,  RarityTier.Rare)));
        table.Add((exoticOreChance, () => RollOre(orePool,  RarityTier.Exotic)));

        table.Add((gemChance,       () => RollGem(gemPool,  RarityTier.Regular)));
        table.Add((rareGemChance,   () => RollGem(gemPool,  RarityTier.Rare)));
        table.Add((exoticGemChance, () => RollGem(gemPool,  RarityTier.Exotic)));

        table.Add((relicChance,     () => RollRelic(relicPool, ctx)));
        table.Add((recipeChance,    () => RollRecipe(recipePool)));
        table.Add((patternChance,   () => RollPattern(patternPool)));

        table.Add((nothingChance,   () => DropResult.Nothing));

        // -----------------------------
        // 3. Weighted random selection
        // -----------------------------
        float total = table.Sum(t => t.weight);
        if (total <= 0f)
            return DropResult.Nothing;

        float roll = Random.value * total;

        foreach (var entry in table)
        {
            if (roll < entry.weight)
                return entry.action();
            roll -= entry.weight;
        }

        return DropResult.Nothing;
    }

    // -----------------------------
    // Ore / Gem / Relic / Recipe / Pattern helpers
    // -----------------------------
    private static DropResult RollOre(List<VeinDefinition> pool, RarityTier rarity)
    {
        var candidates = pool.Where(v => v.rarity == rarity).ToList();
        if (candidates.Count == 0) return DropResult.Nothing;

        var chosen = candidates[Random.Range(0, candidates.Count)];
        return new DropResult { dropType = TileType.Ore, vein = chosen };
    }

    private static DropResult RollGem(List<VeinDefinition> pool, RarityTier rarity)
    {
        var candidates = pool.Where(v => v.rarity == rarity).ToList();
        if (candidates.Count == 0) 
            return DropResult.Nothing;

        // ⭐ Relic bonus chance
        float gemBonus = RelicEffectResolver.GetGemDropBonus();
        if (Random.value < gemBonus)
        {
            var chosen = candidates[Random.Range(0, candidates.Count)];
            return new DropResult { dropType = TileType.Gem, vein = chosen };
        }

        // ⭐ Normal gem roll (fallback)
        var fallback = candidates[Random.Range(0, candidates.Count)];
        return new DropResult { dropType = TileType.Gem, vein = fallback };
    }


    private static DropResult RollRelic(List<RelicDefinition> pool, MineGenerationContext ctx)
    {
        if (pool == null || pool.Count == 0)
            return DropResult.Nothing;

        bool hasRelicManager = RelicManager.Instance != null;

        var candidates = pool.Where(r =>
                (r.biome == RelicBiome.Universal || r.biome.ToString() == ctx.biome.biomeName) &&
                ctx.playerLevel >= r.levelStart &&
                ctx.playerLevel <= r.levelEnd &&
                (!hasRelicManager || !RelicManager.Instance.OwnsRelic(r.relicID))
            ).ToList();

        if (candidates.Count == 0)
            return DropResult.Nothing;

        var chosen = candidates[Random.Range(0, candidates.Count)];
        return new DropResult { dropType = TileType.Relic, relic = chosen };
    }


    private static DropResult RollRecipe(List<RecipeDefinition> pool)
    {
        if (pool == null || pool.Count == 0) return DropResult.Nothing;

        var chosen = pool[Random.Range(0, pool.Count)];
        return new DropResult { dropType = TileType.Recipe, recipe = chosen };
    }

    private static DropResult RollPattern(List<PatternDefinition> pool)
    {
        if (pool == null || pool.Count == 0) return DropResult.Nothing;

        var chosen = pool[Random.Range(0, pool.Count)];
        return new DropResult { dropType = TileType.Pattern, pattern = chosen };
    }
}
