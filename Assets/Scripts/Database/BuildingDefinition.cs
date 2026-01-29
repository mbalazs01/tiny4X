using System.Collections.Generic;
using UnityEngine;

public class BuildingDefinition
{
    public BuildingType buildingType {get; private set; }
    // Building Stats
    public int ExtraProduction {get; private set; }
    public int ExtraFood {get; private set; }
    public int ExtraGold {get; private set; }
    public int ExtraCulture {get; private set; }
    public int ExtraScience {get; private set; }
    // Unit Construction Information
    public int GoldCost {get; }
    public int ProductionCost {get; }
    // Other
    public int Score {get; }
    public int MaintenanceCost {get; }
    public List<HexType> SuitableTiles {get; } 
    public BuildingDefinition(int extraProduction,
     int extraFood,
      int extraGold,
       int extraCulture,
        int extraScience,
         int goldCost,
          int productionCost,
           int score,
            int maintenanceCost,
            IEnumerable<HexType> suitableTiles,
            BuildingType BuildingType)
    {
        ExtraProduction = extraProduction;
        ExtraFood = extraFood;
        ExtraGold = extraGold;
        ExtraCulture = extraCulture;
        ExtraScience = extraScience;
        GoldCost = goldCost;
        ProductionCost = productionCost;
        Score = score;
        MaintenanceCost = maintenanceCost;
        buildingType = BuildingType;
        SuitableTiles = new List<HexType>(suitableTiles);
    }
}
