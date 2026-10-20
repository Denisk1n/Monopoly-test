namespace Monopoly;

public class CommunityChest : Square
{
    public Deck Deck { get; }

    public CommunityChest(string name, int position, Deck deck) : base(name, position)
    {
        Deck = deck;
    }

    public override void OnLand(Player player, Game game)
    {
        Card card = Deck.DrawCard();
        game.Log($"Общая казна: {card.Description}");
        Deck.ReturnToBottom(card);
        card.ApplyEffect(player, game);
    }
}
