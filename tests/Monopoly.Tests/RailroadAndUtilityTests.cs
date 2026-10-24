using Monopoly;
using Monopoly.Tests.Fakes;

namespace Monopoly.Tests;

public class RailroadAndUtilityTests
{
    [Fact]
    public void Railroad_RentDependsOnCount()
    {
        var game = TestGame.Create();
        var ivan = game.Players[1];
        var rizhskaya = TestGame.Square<Railroad>(game, 5);
        var kurskaya = TestGame.Square<Railroad>(game, 15);
        var kazanskaya = TestGame.Square<Railroad>(game, 25);
        var leningradskaya = TestGame.Square<Railroad>(game, 35);

        rizhskaya.SetOwner(ivan);
        Assert.Equal(25, rizhskaya.CalculateRent(game));

        kurskaya.SetOwner(ivan);
        Assert.Equal(50, rizhskaya.CalculateRent(game));

        kazanskaya.SetOwner(ivan);
        leningradskaya.SetOwner(ivan);
        Assert.Equal(200, rizhskaya.CalculateRent(game));
    }

    [Fact]
    public void Railroad_NoOwner_ZeroRent()
    {
        var game = TestGame.Create();

        Assert.Equal(0, TestGame.Square<Railroad>(game, 5).CalculateRent(game));
    }

    [Fact]
    public void Utility_OneOwned_DiceSumTimesFour()
    {
        var game = TestGame.Create(dice: new FixedDice((3, 4)));
        var ivan = game.Players[1];
        var electro = TestGame.Square<Utility>(game, 12);
        electro.SetOwner(ivan);
        game.Dice.Roll();

        Assert.Equal(28, electro.CalculateRent(game));
    }

    [Fact]
    public void Utility_BothOwned_DiceSumTimesTen()
    {
        var game = TestGame.Create(dice: new FixedDice((3, 4)));
        var ivan = game.Players[1];
        var electro = TestGame.Square<Utility>(game, 12);
        var water = TestGame.Square<Utility>(game, 28);
        electro.SetOwner(ivan);
        water.SetOwner(ivan);
        game.Dice.Roll();

        Assert.Equal(70, water.CalculateRent(game));
    }

    [Fact]
    public void Utility_NoOwner_ZeroRent()
    {
        var game = TestGame.Create();

        Assert.Equal(0, TestGame.Square<Utility>(game, 28).CalculateRent(game));
    }
}
