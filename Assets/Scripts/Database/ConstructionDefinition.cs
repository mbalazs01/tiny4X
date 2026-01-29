
using UnityEngine;

public class ConstructionDefinition : IBuildable
{
    public UnitDefinition UnitDefinition;
    public BuildingDefinition BuildingDefinition;
    public int GoldCost {get; }
    public int ProductionCost {get; }
    public ConstructionDefinition(UnitDefinition unitDefinition)
    {
        Debug.Log("New Unit Construction");
        UnitDefinition = unitDefinition;
        GoldCost = UnitDefinition.GoldCost;
        ProductionCost = unitDefinition.ProductionCost;
    }
    public ConstructionDefinition(BuildingDefinition buildingDefinition)
    {
        Debug.Log("New Building Construction");
        BuildingDefinition = buildingDefinition;
        GoldCost = BuildingDefinition.GoldCost;
        ProductionCost = BuildingDefinition.ProductionCost;
    }
    public void Build(HexTile targetTile)
    {
        if(UnitDefinition != null)
        {
            new Unit(UnitDefinition, targetTile);
        }
        else if(BuildingDefinition != null)
        {
            new Building(BuildingDefinition, targetTile);
        } 
        else
        {
            return;
        }
    }
}
