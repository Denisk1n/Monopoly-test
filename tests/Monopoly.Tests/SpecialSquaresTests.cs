using Monopoly;
using Monopoly.Tests.Fakes;

namespace Monopoly.Tests;

public class SpecialSquaresTests
{
    [Fact]
    public void Tax_OnLand_TakesAmount()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var tax = TestGame.Square<Tax>(game, 4);

        tax.OnLand(andrey, game);

        Assert.Equal(1500 - 200, andrey.Cash);
    }

    [Fact]
    public void Tax_NotEnoughMoney_PlayerBecomesBankrupt()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        andrey.Pay(1450);
        var tax = TestGame.Square<Tax>(game, 38);

        tax.OnLand(andrey, game);

        Assert.True(andrey.Bankrupt);
    }

    [Fact]
    public void GoToJail_OnLand_SendsPlayerToJail()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        andrey.Move(30);

        TestGame.Square<GoToJail>(game, 30).OnLand(andrey, game);

        Assert.True(andrey.InJail);
        Assert.Equal(Player.JailPosition, andrey.Position);
    }

    [Fact]
    public void EmptySquares_OnLand_NothingChanges()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];

        TestGame.Square<Go>(game, 0).OnLand(andrey, game);
        TestGame.Square<Jail>(game, 10).OnLand(andrey, game);
        TestGame.Square<FreeParking>(game, 20).OnLand(andrey, game);

        Assert.Equal(1500, andrey.Cash);
        Assert.False(andrey.InJail);
    }

    [Fact]
    public void Chance_OnLand_AppliesTopCardAndPutsItBack()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var deck = new Deck("Тест", new List<Card>
        {
            new Card("Бонус 100", EffectType.Bonus, amount: 100),
            new Card("Штраф 10", EffectType.Fine, amount: 10)
        });
        var chance = new Chance("Шанс", 7, deck);

        chance.OnLand(andrey, game);

        Assert.Equal(1600, andrey.Cash);
        Assert.Equal(2, deck.Count);
        Assert.Equal("Штраф 10", deck.DrawCard().Description);
    }

    [Fact]
    public void CommunityChest_OnLand_AppliesTopCard()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var deck = new Deck("Тест", new List<Card>
        {
            new Card("Штраф 50", EffectType.Fine, amount: 50)
        });
        var chest = new CommunityChest("Общая казна", 2, deck);

        chest.OnLand(andrey, game);

        Assert.Equal(1450, andrey.Cash);
        Assert.Equal(1, deck.Count);
    }

    [Fact]
    public void Square_ToString_ShowsPositionAndName()
    {
        var game = TestGame.Create();

        Assert.Equal("39. ул. Арбат", game.Board.GetSquare(39).ToString());
    }
}
