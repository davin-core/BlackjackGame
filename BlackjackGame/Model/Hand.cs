using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlackjackGame.Model
{
    public class Hand
    {
        public ObservableCollection<Card> Cards { get; } = new ObservableCollection<Card>();

        public int getValue()
        {
            int value = 0;
            int aceCount = 0;
            foreach (var card in Cards)
            {
                value += card.Value;
                if (card.Rank == Enums.Rank.Ace)
                {
                    aceCount++;
                }
            }
            while (value > 21 && aceCount > 0)
            {
                value -= 10; // Treat Ace as 1 instead of 11
                aceCount--;
            }
            return value;
        }
        public bool IsBusted => getValue() > 21;
    }
}
