using Monopoly;
using Monopoly.Tests.Fakes;

namespace Monopoly.Tests;

public class BoardTests
{
    [Fact]
    public void StandardBoard_HasCorrectSquares()
    {
        var game = TestGame.Create();
        var board = game.Board;

        Assert.IsType<Go>(board.GetSquare(0));
        Assert.IsType<Tax>(board.GetSquare(4));
        Assert.IsType<Railroad>(board.GetSquare(5));
        Assert.IsType<Chance>(board.GetSquare(7));
        Assert.IsType<Jail>(board.GetSquare(10));
        Assert.IsType<Utility>(board.GetSquare(12));
        Assert.IsType<CommunityChest>(board.GetSquare(17));
        Assert.IsType<FreeParking>(board.GetSquare(20));
        Assert.IsType<GoToJail>(board.GetSquare(30));
        Assert.IsType<Street>(board.GetSquare(39));
    }

    [Fact]
    public void Board_PositionsMatchIndexes()
    {
        var board = TestGame.Create().Board;

        for (int i = 0; i < Board.Size; i++)
        {
            Assert.Equal(i, board.GetSquare(i).Position);
        }
    }

    [Fact]
    public void Constructor_WrongSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Board(new Square[10]));
    }

    [Fact]
    public void GetStreetsInGroup_ReturnsGroup()
    {
        var board = TestGame.Create().Board;

        Assert.Equal(2, board.GetStreetsInGroup("Синий").Count);
        Assert.Equal(3, board.GetStreetsInGroup("Красный").Count);
        Assert.Empty(board.GetStreetsInGroup("Фиолетовый"));
    }

    [Fact]
    public void GetPropertiesOf_ReturnsOnlyOwned()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        TestGame.Square<Street>(game, 1).SetOwner(andrey);
        TestGame.Square<Railroad>(game, 5).SetOwner(andrey);

        Assert.Equal(2, game.Board.GetPropertiesOf(andrey).Count);
        Assert.Empty(game.Board.GetPropertiesOf(game.Players[1]));
    }

    [Fact]
    public void OwnsWholeGroup_ChecksAllStreets()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        TestGame.Square<Street>(game, 37).SetOwner(andrey);

        Assert.False(game.Board.OwnsWholeGroup(andrey, "Синий"));

        TestGame.Square<Street>(game, 39).SetOwner(andrey);
        Assert.True(game.Board.OwnsWholeGroup(andrey, "Синий"));
    }

    [Fact]
    public void CountRailroadsAndUtilities()
    {
        var game = TestGame.Create();
        var ivan = game.Players[1];
        TestGame.Square<Railroad>(game, 5).SetOwner(ivan);
        TestGame.Square<Railroad>(game, 35).SetOwner(ivan);
        TestGame.Square<Utility>(game, 12).SetOwner(ivan);

        Assert.Equal(2, game.Board.CountRailroadsOf(ivan));
        Assert.Equal(1, game.Board.CountUtilitiesOf(ivan));
    }

    [Fact]
    public void TransferProperties_ChangesOwner()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var ivan = game.Players[1];
        TestGame.Square<Street>(game, 1).SetOwner(andrey);

        game.Board.TransferProperties(andrey, ivan);

        Assert.Same(ivan, TestGame.Square<Street>(game, 1).Owner);
    }

    [Fact]
    public void ReleaseProperties_MakesFreeAndRemovesHouses()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var street = TestGame.Square<Street>(game, 1);
        street.SetOwner(andrey);
        street.AddHouse();

        game.Board.ReleaseProperties(andrey);

        Assert.True(street.IsFree());
        Assert.Equal(0, street.HouseCount);
    }
}
