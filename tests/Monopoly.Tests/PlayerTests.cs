using Monopoly;

namespace Monopoly.Tests;

public class PlayerTests
{
    [Fact]
    public void NewPlayer_HasStartValues()
    {
        var player = new Player("Андрей", 1500);

        Assert.Equal("Андрей", player.Name);
        Assert.Equal(1500, player.Cash);
        Assert.Equal(0, player.Position);
        Assert.False(player.InJail);
        Assert.False(player.Bankrupt);
    }

    [Fact]
    public void Move_ChangesPosition()
    {
        var player = new Player("Андрей", 1500);

        bool passedGo = player.Move(7);

        Assert.Equal(7, player.Position);
        Assert.False(passedGo);
        Assert.Equal(1500, player.Cash);
    }

    [Fact]
    public void Move_PassingGo_Adds200AndStartsNewLap()
    {
        var player = new Player("Андрей", 1500);
        player.Move(38);

        bool passedGo = player.Move(5);

        Assert.True(passedGo);
        Assert.Equal(3, player.Position);
        Assert.Equal(1700, player.Cash);
    }

    [Fact]
    public void MoveTo_ForwardSquare_NoReward()
    {
        var player = new Player("Андрей", 1500);
        player.Move(5);

        bool passedGo = player.MoveTo(24);

        Assert.False(passedGo);
        Assert.Equal(24, player.Position);
        Assert.Equal(1500, player.Cash);
    }

    [Fact]
    public void MoveTo_SquareBehind_CountsAsPassingGo()
    {
        var player = new Player("Андрей", 1500);
        player.Move(36);

        bool passedGo = player.MoveTo(0);

        Assert.True(passedGo);
        Assert.Equal(0, player.Position);
        Assert.Equal(1700, player.Cash);
    }

    [Fact]
    public void PayAndReceive_ChangeCash()
    {
        var player = new Player("Андрей", 1500);

        player.Pay(300);
        player.Receive(50);

        Assert.Equal(1250, player.Cash);
    }

    [Fact]
    public void CanAfford_ComparesWithCash()
    {
        var player = new Player("Андрей", 100);

        Assert.True(player.CanAfford(100));
        Assert.False(player.CanAfford(101));
    }

    [Fact]
    public void SendToJail_MovesToJailSquare()
    {
        var player = new Player("Андрей", 1500);
        player.Move(30);

        player.SendToJail();

        Assert.Equal(Player.JailPosition, player.Position);
        Assert.True(player.InJail);
        Assert.Equal(0, player.JailTurns);
    }

    [Fact]
    public void TryLeaveJail_ReleasesAfterThreeTurns()
    {
        var player = new Player("Андрей", 1500);
        player.SendToJail();

        Assert.False(player.TryLeaveJail());
        Assert.False(player.TryLeaveJail());
        Assert.True(player.TryLeaveJail());
        Assert.False(player.InJail);
    }

    [Fact]
    public void TryLeaveJail_NotInJail_ReturnsTrue()
    {
        var player = new Player("Андрей", 1500);

        Assert.True(player.TryLeaveJail());
    }

    [Fact]
    public void DeclareBankrupt_SetsFlagAndZeroCash()
    {
        var player = new Player("Андрей", 1500);

        player.DeclareBankrupt();

        Assert.True(player.Bankrupt);
        Assert.Equal(0, player.Cash);
    }
}
