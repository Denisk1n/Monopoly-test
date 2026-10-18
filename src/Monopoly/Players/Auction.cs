namespace Monopoly;

public class Auction
{
    public Property Property { get; }
    public List<Player> Participants { get; }
    public int CurrentBid { get; private set; }
    public Player? HighestBidder { get; private set; }

    public Auction(Property property, List<Player> participants)
    {
        Property = property;
        // копируем список, чтобы Pass не удалял игроков из самой игры
        Participants = new List<Player>(participants);
    }

    public bool PlaceBid(Player player, int amount)
    {
        if (!Participants.Contains(player))
        {
            return false;
        }

        if (amount <= CurrentBid || !player.CanAfford(amount))
        {
            return false;
        }

        CurrentBid = amount;
        HighestBidder = player;
        return true;
    }

    public void Pass(Player player)
    {
        Participants.Remove(player);
    }

    // Аукцион закончен, когда остался только лидер (или никого)
    public bool IsFinished()
    {
        if (Participants.Count == 0)
        {
            return true;
        }

        return Participants.Count == 1 && HighestBidder != null;
    }

    // Победитель платит ставку и становится владельцем
    public Player? Conclude()
    {
        if (HighestBidder != null)
        {
            HighestBidder.Pay(CurrentBid);
            Property.SetOwner(HighestBidder);
        }

        return HighestBidder;
    }
}
