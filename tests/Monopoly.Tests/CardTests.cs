using Monopoly;
using Monopoly.Tests.Fakes;

namespace Monopoly.Tests;

public class CardTests
{
    [Fact]
    public void Fine_TakesMoney()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];

        new Card("Штраф", EffectType.Fine, amount: 15).ApplyEffect(andrey, game);

        Assert.Equal(1485, andrey.Cash);
    }

    [Fact]
    public void Bonus_GivesMoney()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];

        new Card("Бонус", EffectType.Bonus, amount: 200).ApplyEffect(andrey, game);

        Assert.Equal(1700, andrey.Cash);
    }

    [Fact]
    public void MoveTo_Go_GivesRewardForPassingGo()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        andrey.Move(7);

        new Card("На Старт", EffectType.MoveTo, targetPosition: 0).ApplyEffect(andrey, game);

        Assert.Equal(0, andrey.Position);
        Assert.Equal(1700, andrey.Cash);
    }

    [Fact]
    public void MoveTo_Property_NewSquareIsProcessed()
    {
        // после перемещения игроку предлагают купить Арбат, он соглашается
        var game = TestGame.Create(new FakeInput().Yes());
        var andrey = game.Players[0];
        andrey.Move(7);

        new Card("На Арбат", EffectType.MoveTo, targetPosition: 39).ApplyEffect(andrey, game);

        Assert.Equal(39, andrey.Position);
        Assert.Same(andrey, TestGame.Square<Street>(game, 39).Owner);
        Assert.Equal(1100, andrey.Cash);
    }

    [Fact]
    public void GoToJail_SendsToJail()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];

        new Card("В тюрьму", EffectType.GoToJail).ApplyEffect(andrey, game);

        Assert.True(andrey.InJail);
    }
}
