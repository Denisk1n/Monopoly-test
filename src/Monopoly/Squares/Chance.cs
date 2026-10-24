namespace Monopoly;

public class Chance : Square
{
    public Deck Deck { get; }

    public Chance(string name, int position, Deck deck) : base(name, position)
    {
        Deck = deck;
    }

    public override void OnLand(Player player, Game game)
    {
        Card card = Deck.DrawCard();
        game.Log($"Шанс: {card.Description}");
        Deck.ReturnToBottom(card);
        card.ApplyEffect(player, game);
    }
}
