using Monopoly;
using Monopoly.Tests.Fakes;

namespace Monopoly.Tests;

public class GameTests
{
    [Fact]
    public void StartGame_CreatesPlayersWithStartCash()
    {
        var game = TestGame.Create();

        Assert.Equal(2, game.Players.Count);
        Assert.All(game.Players, p => Assert.Equal(Game.StartCash, p.Cash));
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal("Андрей", game.GetCurrentPlayer().Name);
    }

    [Fact]
    public void StartGame_WrongPlayerCount_Throws()
    {
        var game = new Game(new FakeInput(), new FixedDice());

        Assert.Throws<ArgumentException>(() => game.StartGame(new List<string> { "Один" }));
        Assert.Throws<ArgumentException>(() =>
            game.StartGame(new List<string> { "1", "2", "3", "4", "5", "6", "7" }));
    }

    [Fact]
    public void PlayTurn_MovesPlayerHandlesSquareAndPassesTurn()
    {
        // 2 + 2 = 4 -> клетка "Подоходный налог" (200)
        var game = TestGame.Create(dice: new FixedDice((2, 2)));
        var andrey = game.Players[0];

        game.PlayTurn();

        Assert.Equal(4, andrey.Position);
        Assert.Equal(1300, andrey.Cash);
        Assert.Equal("Иван", game.GetCurrentPlayer().Name);
    }

    [Fact]
    public void PlayTurn_PassingGo_GivesReward()
    {
        // Андрей стоит на 35, выпадает 5 -> клетка 0 (Старт)
        var game = TestGame.Create(dice: new FixedDice((2, 3)));
        var andrey = game.Players[0];
        andrey.Move(35);

        game.PlayTurn();

        Assert.Equal(0, andrey.Position);
        Assert.Equal(1700, andrey.Cash);
    }

    [Fact]
    public void PlayTurn_PlayerInJail_SkipsTurnAndLeavesAfterThree()
    {
        var game = TestGame.Create(dice: new FixedDice((1, 2)));
        var andrey = game.Players[0];
        andrey.SendToJail();

        for (int i = 0; i < 3; i++)
        {
            game.PlayTurn();                 // ход Андрея в тюрьме
            Assert.Equal(Player.JailPosition, andrey.Position);
            game.AdvanceToNextPlayer();      // пропускаем ход Ивана
        }

        Assert.False(andrey.InJail);

        game.PlayTurn();
        Assert.Equal(13, andrey.Position);
    }

    [Fact]
    public void PlayTurn_GameFinished_DoesNothing()
    {
        var game = new Game(new FakeInput(), new FixedDice((2, 2)));

        game.PlayTurn();

        Assert.Equal(GameStatus.NotStarted, game.Status);
    }

    [Fact]
    public void OfferPurchase_NotEnoughMoney_GoesToAuction()
    {
        // у Андрея 50, купить Арбат за 400 не может; Иван выигрывает аукцион за 10
        var input = new FakeInput().Number(0).Number(10);
        var game = TestGame.Create(input);
        var andrey = game.Players[0];
        andrey.Pay(1450);

        game.OfferPurchase(andrey, TestGame.Square<Street>(game, 39));

        Assert.Same(game.Players[1], TestGame.Square<Street>(game, 39).Owner);
        Assert.Null(game.Auction);
    }

    [Fact]
    public void StartAuction_NoBids_PropertyStaysFree()
    {
        var game = TestGame.Create();
        var arbat = TestGame.Square<Street>(game, 39);

        Player? winner = game.StartAuction(arbat);

        Assert.Null(winner);
        Assert.True(arbat.IsFree());
    }

    [Fact]
    public void StartAuction_PlayersRaiseBids_HighestWins()
    {
        // Андрей 100, Иван 150, Андрей 200, Иван пас
        var input = new FakeInput().Number(100).Number(150).Number(200).Number(0);
        var game = TestGame.Create(input);
        var arbat = TestGame.Square<Street>(game, 39);

        Player? winner = game.StartAuction(arbat);

        Assert.Same(game.Players[0], winner);
        Assert.Equal(1300, game.Players[0].Cash);
    }

    [Fact]
    public void ChargePayment_EnoughMoney_TransfersCash()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var ivan = game.Players[1];

        game.ChargePayment(andrey, ivan, 300);

        Assert.Equal(1200, andrey.Cash);
        Assert.Equal(1800, ivan.Cash);
    }

    [Fact]
    public void HandleBankruptcy_GivesEverythingToCreditor()
    {
        var input = new FakeInput();
        var game = new Game(input, new FixedDice());
        game.StartGame(new List<string> { "Андрей", "Иван", "Пётр" });
        var andrey = game.Players[0];
        var ivan = game.Players[1];
        var street = TestGame.Square<Street>(game, 1);
        street.SetOwner(andrey);
        andrey.Pay(1400);

        game.ChargePayment(andrey, ivan, 500);

        Assert.True(andrey.Bankrupt);
        Assert.Equal(1600, ivan.Cash);
        Assert.Same(ivan, street.Owner);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void HandleBankruptcy_SellsHousesFirst()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var ivan = game.Players[1];
        var zhitnaya = TestGame.Square<Street>(game, 1);
        zhitnaya.SetOwner(andrey);
        for (int i = 0; i < 4; i++)
        {
            zhitnaya.AddHouse();
        }
        andrey.Pay(1450); // осталось 50

        // 4 дома по 50 продаются за полцены = 100, всего 150
        game.ChargePayment(andrey, ivan, 120);

        Assert.False(andrey.Bankrupt);
        Assert.Equal(30, andrey.Cash);
        Assert.Equal(0, zhitnaya.HouseCount);
    }

    [Fact]
    public void LastPlayerLeft_GameFinishedWithWinner()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var ivan = game.Players[1];

        Assert.Null(game.GetWinner());

        game.HandleBankruptcy(andrey, null, 5000);

        Assert.Equal(GameStatus.Finished, game.Status);
        Assert.Same(ivan, game.GetWinner());
    }

    [Fact]
    public void BuildHouse_WithWholeGroup_BuildsAndTakesMoney()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var zhitnaya = TestGame.Square<Street>(game, 1);
        zhitnaya.SetOwner(andrey);
        TestGame.Square<Street>(game, 3).SetOwner(andrey);

        Assert.True(game.BuildHouse(andrey, zhitnaya));
        Assert.Equal(1, zhitnaya.HouseCount);
        Assert.Equal(1450, andrey.Cash);
    }

    [Fact]
    public void BuildHouse_WithoutGroup_ReturnsFalse()
    {
        var game = TestGame.Create();
        var andrey = game.Players[0];
        var zhitnaya = TestGame.Square<Street>(game, 1);
        zhitnaya.SetOwner(andrey);

        Assert.False(game.BuildHouse(andrey, zhitnaya));
        Assert.Equal(1500, andrey.Cash);
    }

    [Fact]
    public void AdvanceToNextPlayer_SkipsBankrupt()
    {
        var game = new Game(new FakeInput(), new FixedDice());
        game.StartGame(new List<string> { "Андрей", "Иван", "Пётр" });
        game.Players[1].DeclareBankrupt();

        game.AdvanceToNextPlayer();

        Assert.Equal("Пётр", game.GetCurrentPlayer().Name);

        game.AdvanceToNextPlayer();
        Assert.Equal("Андрей", game.GetCurrentPlayer().Name);
    }

    [Fact]
    public void Game_WithRandom_ShufflesDecksButKeepsCards()
    {
        var game = new Game(new FakeInput(), new FixedDice(), new Random(3));

        Assert.Equal(5, game.ChanceDeck.Count);
        Assert.Equal(5, game.CommunityChestDeck.Count);
    }

    [Fact]
    public void StartGame_WithRandom_KeepsAllPlayers()
    {
        var game = new Game(new FakeInput(), new FixedDice(), new Random(10));

        game.StartGame(new List<string> { "Андрей", "Иван", "Пётр" });

        Assert.Equal(3, game.Players.Count);
        Assert.Contains("Пётр", game.Players.Select(p => p.Name));
    }
}
