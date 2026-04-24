using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryLibrary
{
    public class GameController
    {
        public List<MemoryCard> Cards { get; private set; } = new();
        public int Moves { get; private set; } // Счетчик ходов

        public bool IsGameOver => Cards.Count > 0 && Cards.All(c => c.IsMatched);

        public void InitializeGame(int pairsCount)
        {
            Cards.Clear();
            Moves = 0; // Сброс ходов при старте
            for (int i = 1; i <= pairsCount; i++)
            {
                Cards.Add(new MemoryCard(i));
                Cards.Add(new MemoryCard(i));
            }
            Shuffle();
        }

        private void Shuffle()
        {
            Random rng = new Random();
            Cards = Cards.OrderBy(a => rng.Next()).ToList();
        }

        public void IncrementMoves() => Moves++;

        public bool CheckMatch(MemoryCard? card1, MemoryCard? card2)
        {
            if (card1 == null || card2 == null || card1 == card2) return false;

            if (card1.PairId == card2.PairId)
            {
                card1.IsMatched = card2.IsMatched = true;
                return true;
            }
            return false;
        }
    }
}