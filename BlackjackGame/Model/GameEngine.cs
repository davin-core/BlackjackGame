using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BlackjackGame.Model.Enums;

namespace BlackjackGame.Model
{
    public class GameEngine : INotifyPropertyChanged
    {
        private Desk desk;
        private int credits;

        public Hand DealerHand { get; private set; }
        public Hand PlayerHand { get; private set; }

        public int Credits
        {
            get => credits;
            private set
            {
                if (credits != value)
                {
                    credits = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameEngine()
        {
            credits = 5;
            InitializeNewGame();
        }

        public void InitializeNewGame()
        {
            Credits = 5;
            InitializeRound();
        }

        public void InitializeRound()
        {
            desk = new Desk();
            DealerHand = new Hand();
            PlayerHand = new Hand();

            // Deal initial cards
            var dealerCard1 = desk.Draw();
            var dealerCard2 = desk.Draw();
            dealerCard2.IsHidden = true; // Hide the dealer's hole card

            DealerHand.Cards.Add(dealerCard1);
            DealerHand.Cards.Add(dealerCard2);

            PlayerHand.Cards.Add(desk.Draw());
            PlayerHand.Cards.Add(desk.Draw());
        }

        public HitResult ProcessHit()
        {
            PlayerHand.Cards.Add(desk.Draw());
            int playerValue = PlayerHand.getValue();

            if (PlayerHand.IsBusted)
            {
                Credits -= 1;
                return new HitResult
                {
                    IsBusted = true,
                    PlayerValue = playerValue,
                    CreditsChanged = -1,
                    IsGameOver = Credits <= 0,
                    AutoStand = false
                };
            }

            // If player gets 21, automatically stand
            if (playerValue == 21)
            {
                return new HitResult
                {
                    IsBusted = false,
                    PlayerValue = playerValue,
                    CreditsChanged = 0,
                    IsGameOver = false,
                    AutoStand = true
                };
            }

            return new HitResult
            {
                IsBusted = false,
                PlayerValue = playerValue,
                CreditsChanged = 0,
                IsGameOver = false,
                AutoStand = false
            };
        }

        public RoundResult ProcessStand()
        {
            // Reveal dealer's hole card
            if (DealerHand.Cards.Count >= 2)
            {
                DealerHand.Cards[1].IsHidden = false;
            }

            // Dealer hits until reaching 17 or higher
            while (DealerHand.getValue() < 17)
            {
                DealerHand.Cards.Add(desk.Draw());
            }

            return DetermineWinner();
        }

        private RoundResult DetermineWinner()
        {
            int playerValue = PlayerHand.getValue();
            int dealerValue = DealerHand.getValue();

            int creditChange = 0;
            string outcome;

            if (dealerValue > 21)
            {
                creditChange = 1;
                Credits += 1;
                outcome = $"Dealer busts with {dealerValue}! You win! (You: {playerValue})";
            }
            else if (playerValue > dealerValue)
            {
                creditChange = 1;
                Credits += 1;
                outcome = $"You win! (You: {playerValue}, Dealer: {dealerValue})";
            }
            else if (dealerValue > playerValue)
            {
                creditChange = -1;
                Credits -= 1;
                outcome = $"Dealer wins! (You: {playerValue}, Dealer: {dealerValue})";
            }
            else
            {
                creditChange = 0;
                outcome = $"Push! It's a tie! (Both: {playerValue})";
            }

            return new RoundResult
            {
                PlayerValue = playerValue,
                DealerValue = dealerValue,
                Outcome = outcome,
                CreditsChanged = creditChange,
                IsGameOver = Credits <= 0
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class HitResult
    {
        public bool IsBusted { get; set; }
        public int PlayerValue { get; set; }
        public int CreditsChanged { get; set; }
        public bool IsGameOver { get; set; }
        public bool AutoStand { get; set; }
    }

    public class RoundResult
    {
        public int PlayerValue { get; set; }
        public int DealerValue { get; set; }
        public string Outcome { get; set; }
        public int CreditsChanged { get; set; }
        public bool IsGameOver { get; set; }
    }
}
