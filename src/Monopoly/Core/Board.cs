namespace Monopoly;

public class Board
{
    public const int Size = 40;

    private readonly Square[] _squares;

    public Board(Square[] squares)
    {
        if (squares.Length != Size)
        {
            throw new ArgumentException($"На доске должно быть {Size} клеток");
        }

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

    // При банкротстве перед игроком всё имущество переходит кредитору
    public void TransferProperties(Player from, Player to)
    {
        foreach (Property property in GetPropertiesOf(from))
        {
            property.SetOwner(to);
        }
    }

    // При банкротстве из-за налога/штрафа клетки становятся свободными
    public void ReleaseProperties(Player player)
    {
        foreach (Property property in GetPropertiesOf(player))
        {
            if (property is Street street)
            {
                street.ClearBuildings();
            }
            property.SetOwner(null);
        }
    }

    public static Board CreateStandard(Deck chanceDeck, Deck communityChestDeck)
    {
        var squares = new Square[]
        {
            new Go("Старт", 0),
            new Street("Житная ул.", 1, 60, "Коричневый", 50, new[] { 2, 10, 30, 90, 160, 250 }),
            new CommunityChest("Общая казна", 2, communityChestDeck),
            new Street("Нагатинская ул.", 3, 60, "Коричневый", 50, new[] { 4, 20, 60, 180, 320, 450 }),
            new Tax("Подоходный налог", 4, 200),
            new Railroad("Рижская железная дорога", 5, 200),
            new Street("Варшавское шоссе", 6, 100, "Голубой", 50, new[] { 6, 30, 90, 270, 400, 550 }),
            new Chance("Шанс", 7, chanceDeck),
            new Street("ул. Огарёва", 8, 100, "Голубой", 50, new[] { 6, 30, 90, 270, 400, 550 }),
            new Street("ул. Первая Парковая", 9, 120, "Голубой", 50, new[] { 8, 40, 100, 300, 450, 600 }),
            new Jail("Тюрьма", 10),
            new Street("ул. Полянка", 11, 140, "Розовый", 100, new[] { 10, 50, 150, 450, 625, 750 }),
            new Utility("Электростанция", 12, 150),
            new Street("ул. Сретенка", 13, 140, "Розовый", 100, new[] { 10, 50, 150, 450, 625, 750 }),
            new Street("Ростовская набережная", 14, 160, "Розовый", 100, new[] { 12, 60, 180, 500, 700, 900 }),
            new Railroad("Курская железная дорога", 15, 200),
            new Street("Рязанский проспект", 16, 180, "Оранжевый", 100, new[] { 14, 70, 200, 550, 750, 950 }),
            new CommunityChest("Общая казна", 17, communityChestDeck),
            new Street("ул. Вавилова", 18, 180, "Оранжевый", 100, new[] { 14, 70, 200, 550, 750, 950 }),
            new Street("Рублёвское шоссе", 19, 200, "Оранжевый", 100, new[] { 16, 80, 220, 600, 800, 1000 }),
            new FreeParking("Бесплатная стоянка", 20),
            new Street("ул. Тверская", 21, 220, "Красный", 150, new[] { 18, 90, 250, 700, 875, 1050 }),
            new Chance("Шанс", 22, chanceDeck),
            new Street("Пушкинская ул.", 23, 220, "Красный", 150, new[] { 18, 90, 250, 700, 875, 1050 }),
            new Street("пл. Маяковского", 24, 240, "Красный", 150, new[] { 20, 100, 300, 750, 925, 1100 }),
            new Railroad("Казанская железная дорога", 25, 200),
            new Street("ул. Грузинский вал", 26, 260, "Жёлтый", 150, new[] { 22, 110, 330, 800, 975, 1150 }),
            new Street("Новинский бульвар", 27, 260, "Жёлтый", 150, new[] { 22, 110, 330, 800, 975, 1150 }),
            new Utility("Водопровод", 28, 150),
            new Street("Смоленская площадь", 29, 280, "Жёлтый", 150, new[] { 24, 120, 360, 850, 1025, 1200 }),
            new GoToJail("Отправляйся в тюрьму", 30),
            new Street("ул. Щусева", 31, 300, "Зелёный", 200, new[] { 26, 130, 390, 900, 1100, 1275 }),
            new Street("Гоголевский бульвар", 32, 300, "Зелёный", 200, new[] { 26, 130, 390, 900, 1100, 1275 }),
            new CommunityChest("Общая казна", 33, communityChestDeck),
            new Street("Кутузовский проспект", 34, 320, "Зелёный", 200, new[] { 28, 150, 450, 1000, 1200, 1400 }),
            new Railroad("Ленинградская железная дорога", 35, 200),
            new Chance("Шанс", 36, chanceDeck),
            new Street("ул. Малая Бронная", 37, 350, "Синий", 200, new[] { 35, 175, 500, 1100, 1300, 1500 }),
            new Tax("Сверхналог", 38, 100),
            new Street("ул. Арбат", 39, 400, "Синий", 200, new[] { 50, 200, 600, 1400, 1700, 2000 })
        };

        return new Board(squares);
    }
}
