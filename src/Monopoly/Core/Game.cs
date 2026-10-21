namespace Monopoly;

public class Game
{
    public const int StartCash = 1500;
    public const int MinPlayers = 2;
    public const int MaxPlayers = 6;

    private readonly IInput _input;
    private readonly Random? _random;

    public List<Player> Players { get; } = new List<Player>();
    public Board Board { get; }
    public Dice Dice { get; }
    public Deck ChanceDeck { get; }
    public Deck CommunityChestDeck { get; }
    public Auction? Auction { get; private set; }
    public GameStatus Status { get; private set; } = GameStatus.NotStarted;
    public int CurrentPlayerIndex { get; private set; }

    public Game(IInput input, Dice dice, Random? random = null)
    {
        _input = input;
        Dice = dice;
        _random = random;

        ChanceDeck = Deck.CreateChanceDeck();
        CommunityChestDeck = Deck.CreateCommunityChestDeck();

        if (random != null)
        {
            ChanceDeck.Shuffle(random);
            CommunityChestDeck.Shuffle(random);
        }

        Board = Board.CreateStandard(ChanceDeck, CommunityChestDeck);
    }

    public void StartGame(List<string> playerNames)
    {
        if (playerNames.Count < MinPlayers || playerNames.Count > MaxPlayers)
        {
            throw new ArgumentException($"Игроков должно быть от {MinPlayers} до {MaxPlayers}");
        }

        Players.Clear();
        foreach (string name in playerNames)
        {
            Players.Add(new Player(name, StartCash));
        }

        // очерёдность ходов определяется случайно (в тестах random не передаём)
        if (_random != null)
        {
            for (int i = Players.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (Players[i], Players[j]) = (Players[j], Players[i]);
            }
        }

        CurrentPlayerIndex = 0;
        Status = GameStatus.InProgress;
    }

    public Player GetCurrentPlayer()
    {
        return Players[CurrentPlayerIndex];
    }

    public List<Player> GetActivePlayers()
    {
        return Players.Where(p => !p.Bankrupt).ToList();
    }

    public void PlayTurn()
    {
        if (Status != GameStatus.InProgress)
        {
            return;
        }

        Player player = GetCurrentPlayer();
        Log($"--- Ход игрока {player.Name} (деньги: {player.Cash}) ---");

        if (player.InJail)
        {
            bool released = player.TryLeaveJail();
            if (released)
            {
                Log($"{player.Name} отсидел срок и в следующий ход выйдет из тюрьмы");
            }
            else
            {
                Log($"{player.Name} в тюрьме, пропущено ходов: {player.JailTurns}");
            }

            AdvanceToNextPlayer();
            return;
        }

        int sum = Dice.Roll();
        Log($"Выпало {Dice.Value1} и {Dice.Value2}, всего {sum}");

        if (player.Move(sum))
        {
            Log($"{player.Name} прошёл Старт и получил {Player.GoReward}");
        }

        Square square = Board.GetSquare(player.Position);
        Log($"{player.Name} попал на клетку: {square.Name}");
        square.OnLand(player, this);

        CheckGameOver();
        AdvanceToNextPlayer();
    }

    public bool AskBuyDecision(Player player, Property property)
    {
        return _input.AskYesNo($"{player.Name}, купить «{property.Name}» за {property.Price}?");
    }

    public void OfferPurchase(Player player, Property property)
    {
        if (player.CanAfford(property.Price) && AskBuyDecision(player, property))
        {
            player.Pay(property.Price);
            property.SetOwner(player);
            Log($"{player.Name} купил «{property.Name}»");
            return;
        }

        Log($"«{property.Name}» выставляется на аукцион");
        StartAuction(property);
    }

    // Игроки по кругу делают ставки, 0 = пас.
    // Аукцион идёт, пока не останется один лидер
    public Player? StartAuction(Property property)
    {
        Auction = new Auction(property, GetActivePlayers());

        while (!Auction.IsFinished())
        {
            foreach (Player bidder in Auction.Participants.ToList())
            {
                if (bidder == Auction.HighestBidder)
                {
                    continue;
                }

                int bid = _input.AskNumber(
                    $"{bidder.Name}, ваша ставка за «{property.Name}» (сейчас {Auction.CurrentBid}, 0 - пас): ");

                if (!Auction.PlaceBid(bidder, bid))
                {
                    Auction.Pass(bidder);
                }

                if (Auction.IsFinished())
                {
                    break;
                }
            }
        }

        Player? winner = Auction.Conclude();

        if (winner != null)
        {
            Log($"Аукцион выиграл {winner.Name} за {Auction.CurrentBid}");
        }
        else
        {
            Log("Ставок не было, клетка остаётся свободной");
        }

        Auction = null;
        return winner;
    }

    // receiver = null, если деньги никому не идут (налог, штраф)
    public void ChargePayment(Player payer, Player? receiver, int amount)
    {
        if (payer.CanAfford(amount))
        {
            payer.Pay(amount);
            receiver?.Receive(amount);
            return;
        }

        HandleBankruptcy(payer, receiver, amount);
    }

    public void HandleBankruptcy(Player debtor, Player? creditor, int amount)
    {
        // сначала пробуем продать дома за полцены
        SellAllBuildings(debtor);

        if (debtor.CanAfford(amount))
        {
            Log($"{debtor.Name} продал дома и смог заплатить");
            debtor.Pay(amount);
            creditor?.Receive(amount);
            return;
        }

        Log($"{debtor.Name} не может заплатить {amount} и становится банкротом!");

        if (creditor != null)
        {
            creditor.Receive(debtor.Cash);
            Board.TransferProperties(debtor, creditor);
        }
        else
        {
            Board.ReleaseProperties(debtor);
        }

        debtor.DeclareBankrupt();
        CheckGameOver();
    }

    private void SellAllBuildings(Player player)
    {
        foreach (Street street in Board.GetPropertiesOf(player).OfType<Street>())
        {
            int level = street.GetBuildingLevel();
            if (level > 0)
            {
                player.Receive(level * street.HouseCost / 2);
                street.ClearBuildings();
            }
        }
    }

    public bool BuildHouse(Player player, Street street)
    {
        if (!street.CanBuild(player, Board))
        {
            return false;
        }

        player.Pay(street.HouseCost);
        street.AddHouse();
        Log($"{player.Name} построил дом на «{street.Name}»");
        return true;
    }

    public void AdvanceToNextPlayer()
    {
        if (Status != GameStatus.InProgress)
        {
            return;
        }

        for (int i = 1; i <= Players.Count; i++)
        {
            int index = (CurrentPlayerIndex + i) % Players.Count;
            if (!Players[index].Bankrupt)
            {
                CurrentPlayerIndex = index;
                return;
            }
        }
    }

    public bool CheckGameOver()
    {
        if (GetActivePlayers().Count <= 1)
        {
            Status = GameStatus.Finished;
            return true;
        }

        return false;
    }

    public Player? GetWinner()
    {
        if (Status != GameStatus.Finished)
        {
            return null;
        }

        return GetActivePlayers().FirstOrDefault();
    }

    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}
