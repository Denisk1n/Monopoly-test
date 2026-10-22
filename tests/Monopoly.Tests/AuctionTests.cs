using Monopoly;

namespace Monopoly.Tests;

public class AuctionTests
{
    private readonly Street _arbat = new Street("ул. Арбат", 39, 400, "Синий", 200, new[] { 50, 200, 600, 1400, 1700, 2000 });
    private readonly Player _andrey = new Player("Андрей", 1500);
    private readonly Player _ivan = new Player("Иван", 1500);

    private Auction CreateAuction()
    {
        return new Auction(_arbat, new List<Player> { _andrey, _ivan });
    }

    [Fact]
    public void PlaceBid_HigherBid_BecomesLeader()
    {
        var auction = CreateAuction();

        Assert.True(auction.PlaceBid(_andrey, 100));
        Assert.True(auction.PlaceBid(_ivan, 150));

        Assert.Equal(150, auction.CurrentBid);
        Assert.Same(_ivan, auction.HighestBidder);
    }

    [Fact]
    public void PlaceBid_NotHigherThanCurrent_Rejected()
    {
        var auction = CreateAuction();
        auction.PlaceBid(_andrey, 100);

        Assert.False(auction.PlaceBid(_ivan, 100));
        Assert.Same(_andrey, auction.HighestBidder);
    }

    [Fact]
    public void PlaceBid_NotEnoughMoney_Rejected()
    {
        var auction = CreateAuction();

        Assert.False(auction.PlaceBid(_andrey, 2000));
        Assert.Null(auction.HighestBidder);
    }

    [Fact]
    public void PlaceBid_NotParticipant_Rejected()
    {
        var auction = CreateAuction();
        var stranger = new Player("Пётр", 1500);

        Assert.False(auction.PlaceBid(stranger, 100));
    }

    [Fact]
    public void Pass_RemovesOnlyFromAuction()
    {
        var players = new List<Player> { _andrey, _ivan };
        var auction = new Auction(_arbat, players);

        auction.Pass(_andrey);

        Assert.Single(auction.Participants);
        Assert.Equal(2, players.Count);
    }

    [Fact]
    public void IsFinished_WhenOnlyLeaderLeft()
    {
        var auction = CreateAuction();
        auction.PlaceBid(_ivan, 50);

        Assert.False(auction.IsFinished());
        auction.Pass(_andrey);
        Assert.True(auction.IsFinished());
    }

    [Fact]
    public void Conclude_WinnerPaysAndBecomesOwner()
    {
        var auction = CreateAuction();
        auction.PlaceBid(_ivan, 250);
        auction.Pass(_andrey);

        Player? winner = auction.Conclude();

        Assert.Same(_ivan, winner);
        Assert.Equal(1250, _ivan.Cash);
        Assert.Same(_ivan, _arbat.Owner);
    }

    [Fact]
    public void Conclude_NoBids_PropertyStaysFree()
    {
        var auction = CreateAuction();
        auction.Pass(_andrey);
        auction.Pass(_ivan);

        Assert.True(auction.IsFinished());
        Assert.Null(auction.Conclude());
        Assert.True(_arbat.IsFree());
    }
}
