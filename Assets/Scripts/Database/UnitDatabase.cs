public class UnitDatabase
{
    public static readonly UnitDefinition Settler = new(
        UnitType.Settler,
        maxHP: 1,
        attack: 0,
        defense: 0,
        maxMovement: 2,
        score: 0,
        goldCost: 150,
        productionCost: 30,
        maintenanceCost: 0,
        abilities: new[]
        {
            UnitAbility.Settle
        }
    );
    public static readonly UnitDefinition Scout = new(
        UnitType.Scout,
        maxHP: 5,
        attack: 2,
        defense: 0,
        maxMovement: 5,
        score: 1,
        goldCost: 80,
        productionCost: 20,
        maintenanceCost: 1,
        abilities: new[]
        {
            UnitAbility.Attack
        }
    );

    public static readonly UnitDefinition Warrior = new(
        UnitType.Warrior,
        maxHP: 10,
        attack: 5,
        defense: 2,
        maxMovement: 2,
        score: 1,
        goldCost: 50,
        productionCost: 15,
        maintenanceCost: 2,
        abilities: new[]
        {
            UnitAbility.Attack
        }
    );

    public static readonly UnitDefinition Archer = new(
        UnitType.Archer,
        maxHP: 5,
        attack: 4,
        defense: 1,
        maxMovement: 2,
        score: 1,
        goldCost: 100,
        productionCost: 40,
        maintenanceCost: 2,
        abilities: new[]
        {
            UnitAbility.RangedAttack
        }
    );

    public static readonly UnitDefinition CommercialShip = new(
        UnitType.CommercialShip,
        maxHP: 3,
        attack: 0,
        defense: 0,
        maxMovement: 4,
        score: 0,
        goldCost: 100,
        productionCost: 25,
        maintenanceCost: 3,
        abilities: new[]
        {
            UnitAbility.Embark,
            UnitAbility.ExploitResource
        }
    );

    public static readonly UnitDefinition WarShip = new(
        UnitType.WarShip,
        maxHP: 10,
        attack: 3,
        defense: 2,
        maxMovement: 4,
        score: 1,
        goldCost: 200,
        productionCost: 50,
        maintenanceCost: 5,
        abilities: new[]
        {
            UnitAbility.Embark,
            UnitAbility.RangedAttack
        }
    );
}
