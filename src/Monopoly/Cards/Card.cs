namespace Monopoly;

public class Card
{
    public string Description { get; }
    public EffectType EffectType { get; }
    public int Amount { get; }
    public int TargetPosition { get; }

    public Card(string description, EffectType effectType, int amount = 0, int targetPosition = 0)
    {
        Description = description;
        EffectType = effectType;
        Amount = amount;
        TargetPosition = targetPosition;
    }

    public void ApplyEffect(Player player, Game game)
    {
        switch (EffectType)
        {
            case EffectType.Fine:
                game.ChargePayment(player, null, Amount);
                break;

            case EffectType.Bonus:
                player.Receive(Amount);
                break;

            case EffectType.MoveTo:
                if (player.MoveTo(TargetPosition))
                {
                    game.Log($"{player.Name} прошёл Старт и получил {Player.GoReward}");
                }
                // после перемещения обрабатываем новую клетку
                Square square = game.Board.GetSquare(TargetPosition);
                game.Log($"{player.Name} перемещается на клетку {square.Name}");
                square.OnLand(player, game);
                break;

            case EffectType.GoToJail:
                player.SendToJail();
                break;
        }
    }
}
