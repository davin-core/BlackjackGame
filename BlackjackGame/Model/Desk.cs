using System;
using System.Collections.Generic;
using System.Text;
using BlackjackGame.Model.Enums;

namespace BlackjackGame.Model
{
    public class Desk
    {
        private List<Card> cards;
        private Random random = new Random();

        public Desk()
        {
            cards = new List<Card>();
            InitializeDeck();
            shuffle();
        }

        private void InitializeDeck()
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards.Add(new Card(suit, rank));
                }
            }
        }

        public void shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                var temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        public Card Draw()
        {
            if (cards.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty.");
            }

            var card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }
}
