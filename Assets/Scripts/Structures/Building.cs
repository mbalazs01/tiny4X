using UnityEngine;

public class Building
{
    public Building(BuildingDefinition buildingDefinition, HexTile ownerTile)
    {
        BuildingDefinition = buildingDefinition;
        OwnerTile = ownerTile;
        OwnerCity = ownerTile.city;
        OwnerCountry = OwnerCity.ownerCountry;

        OwnerTile.BuildingConstructed(this);
    }
    public void DestroyBuilding()
    {
        OwnerTile.BuildingDemolished(this);
    }
    public BuildingDefinition BuildingDefinition { get; private set; }
    public HexTile OwnerTile { get; private set; }
    public City OwnerCity { get; private set; }
    public Country OwnerCountry { get; private set; }
}
