namespace MemoryLibrary
{
    public class MemoryCard
    {
        // ID пары (например, цифра 1, 2, 3... чтобы сравнивать карты)
        public int PairId { get; set; }
        // Открыта ли карта сейчас
        public bool IsRevealed { get; set; }
        // Найдена ли пара для этой карты
        public bool IsMatched { get; set; }

        public MemoryCard(int pairId)
        {
            PairId = pairId;
            IsRevealed = false;
            IsMatched = false;
        }
    }
}