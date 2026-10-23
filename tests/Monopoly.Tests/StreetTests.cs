using Monopoly;
using Monopoly.Tests.Fakes;

namespace Monopoly.Tests;

public class StreetTests
{
    // Коричневая группа: Житная (1) и Нагатинская (3)
    private static (Game game, Player andrey, Street zhitnaya, Street nagatinskaya) CreateBrownGroup()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var zhitnaya = TestGame.Square<Street>(game, 1);
        var nagatinskaya = TestGame.Square<Street>(game, 3);
        return (game, andrey, zhitnaya, nagatinskaya);
    }

    [Fact]
    public void Constructor_WrongRentLevels_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Street("Тест", 1, 60, "Коричневый", 50, new[] { 1, 2, 3 }));
    }

    [Fact]
    public void CalculateRent_NoHouses_BaseRent()
    {
        var (game, andrey, zhitnaya, _) = CreateBrownGroup();
        zhitnaya.SetOwner(andrey);

        Assert.Equal(2, zhitnaya.CalculateRent(game));
    }

    [Fact]
    public void CalculateRent_WholeGroupNoHouses_DoubleRent()
    {
        var (game, andrey, zhitnaya, nagatinskaya) = CreateBrownGroup();
        zhitnaya.SetOwner(andrey);
        nagatinskaya.SetOwner(andrey);

        Assert.Equal(4, zhitnaya.CalculateRent(game));
    }

    [Fact]
    public void CalculateRent_WithHousesAndHotel()
    {
        var (game, andrey, zhitnaya, _) = CreateBrownGroup();
        zhitnaya.SetOwner(andrey);

        zhitnaya.AddHouse();
        zhitnaya.AddHouse();
        Assert.Equal(30, zhitnaya.CalculateRent(game));

        zhitnaya.AddHouse();
        zhitnaya.AddHouse();
        zhitnaya.AddHouse();
        Assert.True(zhitnaya.HasHotel);
        Assert.Equal(250, zhitnaya.CalculateRent(game));
    }

    [Fact]
    public void AddHouse_FifthHouseBecomesHotel()
    {
        var (_, _, zhitnaya, _) = CreateBrownGroup();

        for (int i = 0; i < 5; i++)
        {
            zhitnaya.AddHouse();
        }

        Assert.True(zhitnaya.HasHotel);
        Assert.Equal(0, zhitnaya.HouseCount);
        Assert.Equal(5, zhitnaya.GetBuildingLevel());

        zhitnaya.AddHouse();
        Assert.Equal(5, zhitnaya.GetBuildingLevel());
    }

    [Fact]
    public void RemoveHouse_FromHotel_LeavesFourHouses()
    {
        var (_, _, zhitnaya, _) = CreateBrownGroup();
        for (int i = 0; i < 5; i++)
        {
            zhitnaya.AddHouse();
        }

        zhitnaya.RemoveHouse();
        Assert.False(zhitnaya.HasHotel);
        Assert.Equal(4, zhitnaya.HouseCount);

        zhitnaya.RemoveHouse();
        Assert.Equal(3, zhitnaya.HouseCount);
    }

    [Fact]
    public void ClearBuildings_RemovesEverything()
    {
        var (_, _, zhitnaya, _) = CreateBrownGroup();
        zhitnaya.AddHouse();
        zhitnaya.AddHouse();

        zhitnaya.ClearBuildings();

        Assert.Equal(0, zhitnaya.GetBuildingLevel());
    }

    [Fact]
    public void CanBuild_WithoutWholeGroup_False()
    {
        var (game, andrey, zhitnaya, _) = CreateBrownGroup();
        zhitnaya.SetOwner(andrey);

        Assert.False(zhitnaya.CanBuild(andrey, game.Board));
    }

    [Fact]
    public void CanBuild_WithWholeGroup_True()
    {
        var (game, andrey, zhitnaya, nagatinskaya) = CreateBrownGroup();
        zhitnaya.SetOwner(andrey);
        nagatinskaya.SetOwner(andrey);

        Assert.True(zhitnaya.CanBuild(andrey, game.Board));
    }

    [Fact]
    public void CanBuild_NotOwner_False()
    {
        var (game, andrey, zhitnaya, nagatinskaya) = CreateBrownGroup();
        zhitnaya.SetOwner(andrey);
        nagatinskaya.SetOwner(andrey);
        var ivan = game.Players[1];

        Assert.False(zhitnaya.CanBuild(ivan, game.Board));
    }

    [Fact]
    public void CanBuild_UnevenBuilding_False()
    {
        var (game, andrey, zhitnaya, nagatinskaya) = CreateBrownGroup();
        zhitnaya.SetOwner(andrey);
        nagatinskaya.SetOwner(andrey);
        zhitnaya.AddHouse();

        // на Житной уже 1 дом, а на Нагатинской 0 - сначала надо строить там
        Assert.False(zhitnaya.CanBuild(andrey, game.Board));
        Assert.True(nagatinskaya.CanBuild(andrey, game.Board));
    }
}
