using Monopoly;
using Monopoly.Tests.Fakes;

namespace Monopoly.Tests;

public class PropertyTests
{
    [Fact]
    public void NewProperty_IsFree()
    {
        var game = TestGame.Create();
        var street = TestGame.Square<Street>(game, 1);

        Assert.True(street.IsFree());
        Assert.Null(street.GetOwner());
    }

    [Fact]
    public void OnLand_FreeProperty_PlayerAgrees_BuysIt()
    {
        var game = TestGame.Create(new FakeInput().Yes());
        var andrey = game.Players[0];
        var street = TestGame.Square<Street>(game, 1);

        street.OnLand(andrey, game);

        Assert.Same(andrey, street.Owner);
        Assert.Equal(1500 - 60, andrey.Cash);
    }

    [Fact]
    public void OnLand_FreeProperty_PlayerRefuses_GoesToAuction()
    {
        // Андрей отказался и сказал пас (0), Иван поставил 40
        var input = new FakeInput().No().Number(0).Number(40);
        var game = TestGame.Create(input);
        var andrey = game.Players[0];
        var ivan = game.Players[1];
        var street = TestGame.Square<Street>(game, 1);

        street.OnLand(andrey, game);

        Assert.Same(ivan, street.Owner);
        Assert.Equal(1500 - 40, ivan.Cash);
        Assert.Equal(1500, andrey.Cash);
    }

    [Fact]
    public void OnLand_OwnProperty_NothingHappens()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var street = TestGame.Square<Street>(game, 1);
        street.SetOwner(andrey);

        street.OnLand(andrey, game);

        Assert.Equal(1500, andrey.Cash);
    }

    [Fact]
    public void OnLand_OtherOwner_PaysRent()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var ivan = game.Players[1];
        var street = TestGame.Square<Street>(game, 1);
        street.SetOwner(ivan);

        street.OnLand(andrey, game);

        Assert.Equal(1500 - 2, andrey.Cash);
        Assert.Equal(1500 + 2, ivan.Cash);
    }
}
