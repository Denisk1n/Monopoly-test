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

    // TODO: покупка или аукцион
    public void OfferPurchase(Player player, Property property)
    {
    }

    // TODO: оплата и банкротство
    public void ChargePayment(Player payer, Player? receiver, int amount)
    {
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
