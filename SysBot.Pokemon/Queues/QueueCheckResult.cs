using PKHeX.Core;

namespace SysBot.Pokemon
{
    public class QueueCheckResult<T> where T : PKM, new()
    {
        public readonly bool InQueue;
        public readonly TradeEntry<T>? Detail;
        public readonly int Position;
        public readonly int QueueCount;

        public static readonly QueueCheckResult<T> None = new();

        public QueueCheckResult(bool inQueue = false, TradeEntry<T>? detail = default, int position = -1, int queueCount = -1)
        {
            InQueue = inQueue;
            Detail = detail;
            Position = position;
            QueueCount = queueCount;
        }

        public string GetMessage()
        {
            if (!InQueue || Detail is null)
                return "You're not even in a queue right now. See the image for bot commands.\n https://i.imgur.com/KpysCZb.jpg";
            var position = $"{Position}/{QueueCount}";
            var msg = $"You're in the **{Detail.Type} Queue**! Position: {position} (ID {Detail.Trade.ID})";
            var pk = Detail.Trade.TradeData;
            if (pk.Species != 0)
                msg += $", Receiving: {(Species)Detail.Trade.TradeData.Species}";
            return msg;
        }
    }
}