using System.ComponentModel;
using System.Runtime.CompilerServices;
using BlackjackGame.Model.Enums;

namespace BlackjackGame.Model
{
    public class Card : INotifyPropertyChanged
    {
        private bool isHidden;

        public Suit Suit { get;}
        public Rank Rank { get; }

        public bool IsHidden
        {
            get => isHidden;
            set
            {
                if (isHidden != value)
                {
                    isHidden = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public int Value { get; }

        public string ImagePath => IsHidden ? "pack://application:,,,/Assets/Cards/back_dark.png" : $"pack://application:,,,/Assets/Cards/{Suit.ToString().ToLower()}_{GetRankShorthand()}.png";

        private string GetRankShorthand() => Rank switch
        {
            Rank.Ace => "A",
            Rank.Two => "2",
            Rank.Three => "3",
            Rank.Four => "4",
            Rank.Five => "5",
            Rank.Six => "6",
            Rank.Seven => "7",
            Rank.Eight => "8",
            Rank.Nine => "9",
            Rank.Ten => "10",
            Rank.Jack => "J",
            Rank.Queen => "Q",
            Rank.King => "K",
            _ => ""
        };

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
            isHidden = false;
            Value = CalculateValue(rank);
        }

        private static int CalculateValue(Rank rank) => rank switch
        {
            Rank.Ace => 11,
            Rank.Two => 2,
            Rank.Three => 3,
            Rank.Four => 4,
            Rank.Five => 5,
            Rank.Six => 6,
            Rank.Seven => 7,
            Rank.Eight => 8,
            Rank.Nine => 9,
            Rank.Ten => 10,
            Rank.Jack => 10,
            Rank.Queen => 10,
            Rank.King => 10,
            _ => 0
        };

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
