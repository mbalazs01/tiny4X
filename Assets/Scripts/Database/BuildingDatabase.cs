using UnityEngine;

public class BuildingDatabase
{
    public static readonly BuildingDefinition Farm = new(
        BuildingType: BuildingType.Farm,
        extraProduction: 0,
        extraCulture: 0,
        extraFood: 2,
        extraGold: 0,
        extraScience: 0,
        goldCost: 150,
        productionCost: 30,
        maintenanceCost: 1,
        score: 1,
        suitableTiles: new[]
        {
            HexType.Land
        }
    );
        public static readonly BuildingDefinition Mine = new(
        BuildingType: BuildingType.Mine,
        extraProduction: 2,
        extraCulture: 0,
        extraFood: 0,
        extraGold: 0,
        extraScience: 0,
        goldCost: 150,
        productionCost: 30,
        maintenanceCost: 1,
        score: 1,
        suitableTiles: new[]
        {
            HexType.Land
        }
    );
        public static readonly BuildingDefinition Fishery = new(
        BuildingType: BuildingType.Fishery,
        extraProduction: 0,
        extraCulture: 0,
        extraFood: 2,
        extraGold: 2,
        extraScience: 0,
        goldCost: 150,
        productionCost: 30,
        maintenanceCost: 1,
        score: 1,
        suitableTiles: new[]
        {
            HexType.Water
        }
    );
}
