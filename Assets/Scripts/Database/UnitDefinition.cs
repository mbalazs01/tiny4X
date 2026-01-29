using System.Collections.Generic;

public class UnitDefinition : IBuildable
{
    // Unit Type
    public UnitType UnitType {get; }
    // Unit Stats
    public int MaxHP {get; }
    public int MaxMovement {get; }
    public int Attack {get; }
    public int Defense {get; }
    // Unit Construction Information
    public int GoldCost {get; }
    public int ProductionCost {get; }
    // Other
    public int Score {get; }
    public int MaintenanceCost {get; }
    // Unit Abilities
    public HashSet<UnitAbility> Abilities {get; } 
    public UnitDefinition(UnitType unitType,
     int maxHP, 
      int maxMovement,
       int attack,
        int defense, 
         int goldCost,
          int productionCost,
           int score,
            int maintenanceCost,
             IEnumerable<UnitAbility> abilities)
    {
        UnitType = unitType;
        MaxHP = maxHP;
        MaxMovement = maxMovement;
        Attack = attack;
        Defense = defense;
        GoldCost = goldCost;
        ProductionCost = productionCost;
        Score = score;
        MaintenanceCost = maintenanceCost;
        Abilities = new HashSet<UnitAbility>(abilities);
    }
}
