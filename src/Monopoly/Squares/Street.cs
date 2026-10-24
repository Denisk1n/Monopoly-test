namespace Monopoly;

public class Street : Property
{
    public const int MaxHouses = 4;

    public string ColorGroup { get; }
    public int HouseCost { get; }

    // аренда: [без домов, 1 дом, 2 дома, 3 дома, 4 дома, отель]
    public int[] RentLevels { get; }

    public int HouseCount { get; private set; }
    public bool HasHotel { get; private set; }

    public Street(string name, int position, int price, string colorGroup, int houseCost, int[] rentLevels)
        : base(name, position, price)
    {
        if (rentLevels.Length != 6)
        {
            throw new ArgumentException("Нужно 6 значений аренды");
        }

        ColorGroup = colorGroup;
        HouseCost = houseCost;
        RentLevels = rentLevels;
    }

    // Уровень застройки: 0-4 дома, 5 = отель
    public int GetBuildingLevel()
    {
        return HasHotel ? MaxHouses + 1 : HouseCount;
    }

    public override int CalculateRent(Game game)
    {
        if (HasHotel)
        {
            return RentLevels[5];
        }

        if (HouseCount > 0)
        {
            return RentLevels[HouseCount];
        }

        // если у владельца вся цветовая группа - аренда без домов двойная
        if (Owner != null && game.Board.OwnsWholeGroup(Owner, ColorGroup))
        {
            return RentLevels[0] * 2;
        }

        return RentLevels[0];
    }

    public bool CanBuild(Player player, Board board)
    {
        if (Owner != player || HasHotel)
        {
            return false;
        }

        if (!board.OwnsWholeGroup(player, ColorGroup))
        {
            return false;
        }

        if (!board.IsBuildingEven(this))
        {
            return false;
        }

        return player.CanAfford(HouseCost);
    }

    public void AddHouse()
    {
        if (HasHotel)
        {
            return;
        }

        if (HouseCount < MaxHouses)
        {
            HouseCount++;
        }
        else
        {
            // пятый дом превращается в отель
            HouseCount = 0;
            HasHotel = true;
        }
    }

    public void RemoveHouse()
    {
        if (HasHotel)
        {
            HasHotel = false;
            HouseCount = MaxHouses;
        }
        else if (HouseCount > 0)
        {
            HouseCount--;
        }
    }

    public void ClearBuildings()
    {
        HouseCount = 0;
        HasHotel = false;
    }
}
