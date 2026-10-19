namespace Monopoly;

public class Board
{
    private readonly Square[] _squares;

    public Board(Square[] squares)
    {
        _squares = squares;
    }

    public Square GetSquare(int position)
    {
        return _squares[position];
    }

    public List<Property> GetPropertiesOf(Player player)
    {
        return _squares.OfType<Property>().Where(p => p.Owner == player).ToList();
    }

    public List<Street> GetStreetsInGroup(string colorGroup)
    {
        return _squares.OfType<Street>().Where(s => s.ColorGroup == colorGroup).ToList();
    }

    public bool OwnsWholeGroup(Player player, string colorGroup)
    {
        List<Street> streets = GetStreetsInGroup(colorGroup);
        return streets.Count > 0 && streets.All(s => s.Owner == player);
    }

    // Строить можно только равномерно: на улице не должно быть больше домов,
    // чем на других улицах этой же группы
    public bool IsBuildingEven(Street street)
    {
        return GetStreetsInGroup(street.ColorGroup)
            .All(s => s.GetBuildingLevel() >= street.GetBuildingLevel());
    }

    public int CountRailroadsOf(Player player)
    {
        return _squares.OfType<Railroad>().Count(r => r.Owner == player);
    }

    public int CountUtilitiesOf(Player player)
    {
        return _squares.OfType<Utility>().Count(u => u.Owner == player);
    }
}
