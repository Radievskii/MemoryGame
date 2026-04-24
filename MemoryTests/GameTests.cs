using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemoryLibrary;
using System.Linq;

namespace MemoryTests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void InitializeGame_ShouldCreateCorrectNumberOfPairs()
        {
            var game = new GameController();
            game.InitializeGame(8);

            // Для коллекций в .NET 8 лучше использовать Assert.AreEqual для Count
            Assert.AreEqual(16, game.Cards.Count);
        }

        [TestMethod]
        public void CheckMatch_DifferentPairId_ShouldReturnFalse()
        {
            var game = new GameController();
            var card1 = new MemoryCard(1);
            var card2 = new MemoryCard(2); // Разные ID

            bool result = game.CheckMatch(card1, card2);

            Assert.IsFalse(result); // Ожидаем ложь
            Assert.IsFalse(card1.IsMatched); // Карты не должны быть помечены как найденные
        }

        [TestMethod]
        public void CheckMatch_SamePairId_ShouldReturnTrue()
        {
            var game = new GameController();
            var card1 = new MemoryCard(1);
            var card2 = new MemoryCard(1);

            bool result = game.CheckMatch(card1, card2);

            Assert.IsTrue(result);
            Assert.IsTrue(card1.IsMatched);
        }
    }
}