using Monopoly;

namespace Monopoly.Tests;

public class DiceTests
{
    [Fact]
    public void Roll_ValuesAreFromOneToSix()
    {
        var dice = new Dice(new Random(42));

        for (int i = 0; i < 100; i++)
        {
            dice.Roll();
            Assert.InRange(dice.Value1, 1, 6);
            Assert.InRange(dice.Value2, 1, 6);
        }
    }

    [Fact]
    public void Roll_ReturnsSumOfBothDice()
    {
        var dice = new Dice(new Random(7));

        int sum = dice.Roll();

        Assert.Equal(dice.Value1 + dice.Value2, sum);
        Assert.Equal(sum, dice.GetSum());
    }

    [Fact]
    public void Roll_SameSeed_SameResult()
    {
        var dice1 = new Dice(new Random(5));
        var dice2 = new Dice(new Random(5));

        Assert.Equal(dice1.Roll(), dice2.Roll());
    }
}
