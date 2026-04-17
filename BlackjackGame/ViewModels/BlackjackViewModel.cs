using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BlackjackGame.Common;
using BlackjackGame.Model;

namespace BlackjackGame.ViewModels
{
    public class BlackjackViewModel : INotifyPropertyChanged
    {
        private Hand dealerHand;
        private Hand playerHand;
        private Card dealerCard;
        private Desk desk;
        private ICommand hitCommand;
        private ICommand standCommand;
        private ICommand newGameCommand;
        private ICommand nextGameCommand;
        private ICommand stopPlayingCommand;
        private GameState gameState;
        private string gameStatus;
        private int credits;
        private int lastCreditChange;
        private string creditChangeMessage;

        public enum GameState
        {
            Playing,
            PlayerBusted,
            StandingWait,
            GameOver,
            RoundOver
        }

        public Hand DealerHand
        {
            get => dealerHand;
            set
            {
                if (dealerHand != value)
                {
                    dealerHand = value;
                    OnPropertyChanged();
                }
            }
        }

        public Hand PlayerHand
        {
            get => playerHand;
            set
            {
                if (playerHand != value)
                {
                    playerHand = value;
                    OnPropertyChanged();
                }
            }
        }

        public Card DealerCard
        {
            get => dealerCard;
            set
            {
                if (dealerCard != value)
                {
                    dealerCard = value;
                    OnPropertyChanged();
                }
            }
        }

        public GameState CurrentGameState
        {
            get => gameState;
            set
            {
                if (gameState != value)
                {
                    gameState = value;
                    OnPropertyChanged();
                }
            }
        }

        public string GameStatus
        {
            get => gameStatus;
            set
            {
                if (gameStatus != value)
                {
                    gameStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Credits
        {
            get => credits;
            set
            {
                if (credits != value)
                {
                    credits = value;
                    OnPropertyChanged();
                }
            }
        }

        public int LastCreditChange
        {
            get => lastCreditChange;
            set
            {
                if (lastCreditChange != value)
                {
                    lastCreditChange = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CreditChangeMessage
        {
            get => creditChangeMessage;
            set
            {
                if (creditChangeMessage != value)
                {
                    creditChangeMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DealerHandValue => $"(Total: {DealerHand?.getValue() ?? 0})";

        public string PlayerHandValue => $"(Total: {PlayerHand?.getValue() ?? 0})";

        public ICommand HitCommand
        {
            get
            {
                if (hitCommand == null)
                {
                    hitCommand = new RelayCommand(_ => Hit(), _ => CurrentGameState == GameState.Playing);
                }
                return hitCommand;
            }
        }

        public ICommand StandCommand
        {
            get
            {
                if (standCommand == null)
                {
                    standCommand = new RelayCommand(_ => Stand(), _ => CurrentGameState == GameState.Playing);
                }
                return standCommand;
            }
        }

        public ICommand NewGameCommand
        {
            get
            {
                if (newGameCommand == null)
                {
                    newGameCommand = new RelayCommand(_ => NewGame(), _ => CurrentGameState == GameState.GameOver);
                }
                return newGameCommand;
            }
        }

        public ICommand NextGameCommand
        {
            get
            {
                if (nextGameCommand == null)
                {
                    nextGameCommand = new RelayCommand(_ => NextGame(), _ => CurrentGameState == GameState.RoundOver && Credits > 0);
                }
                return nextGameCommand;
            }
        }

        public ICommand StopPlayingCommand
        {
            get
            {
                if (stopPlayingCommand == null)
                {
                    stopPlayingCommand = new RelayCommand(_ => StopPlaying());
                }
                return stopPlayingCommand;
            }
        }

        public BlackjackViewModel()
        {
            gameState = GameState.Playing;
            credits = 5;
            gameStatus = "Game started. Your move!";
            lastCreditChange = 0;
            creditChangeMessage = "";
            InitializeGame();
        }

        private void InitializeGame()
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

            // Notify UI of hand value changes
            OnPropertyChanged(nameof(DealerHandValue));
            OnPropertyChanged(nameof(PlayerHandValue));

            // Clear previous round credit messages after showing the new hand
            LastCreditChange = 0;
            CreditChangeMessage = "";

            // Check for blackjack
            if (PlayerHand.getValue() == 21)
            {
                GameStatus = "Blackjack! You have 21!";
            }
            else
            {
                GameStatus = "Your move!";
            }
        }

        private void Hit()
        {
            if (CurrentGameState != GameState.Playing)
                return;

            PlayerHand.Cards.Add(desk.Draw());
            OnPropertyChanged(nameof(PlayerHandValue));

            int playerValue = PlayerHand.getValue();

            if (PlayerHand.IsBusted)
            {
                CurrentGameState = GameState.PlayerBusted;
                GameStatus = $"Bust! You have {playerValue}. Dealer wins!";

                LastCreditChange = -1;
                Credits -= 1;
                CreditChangeMessage = "-1 Credit";

                if (Credits <= 0)
                {
                    CurrentGameState = GameState.GameOver;
                    GameStatus = "Game Over! You're out of credits! Click 'New Game' to start again.";
                }
                else
                {
                    CurrentGameState = GameState.RoundOver;
                    GameStatus = $"Bust! You have {playerValue}. Dealer wins! Click 'Next Game' to continue.";
                }
            }
            else if (playerValue == 21)
            {
                GameStatus = $"You have 21! Your turn is over.";
            }
            else
            {
                GameStatus = $"You have {playerValue}. Hit or Stand?";
            }
        }

        private void Stand()
        {
            if (CurrentGameState != GameState.Playing)
                return;

            CurrentGameState = GameState.StandingWait;
            GameStatus = "Dealer's turn...";

            // Reveal dealer's hole card
            if (DealerHand.Cards.Count >= 2)
            {
                DealerHand.Cards[1].IsHidden = false;
                OnPropertyChanged(nameof(DealerHandValue));
            }

            // Dealer hits until reaching 17 or higher
            while (DealerHand.getValue() < 17)
            {
                DealerHand.Cards.Add(desk.Draw());
                OnPropertyChanged(nameof(DealerHandValue));
            }

            DetermineWinner();
        }

        private void DetermineWinner()
        {
            int playerValue = PlayerHand.getValue();
            int dealerValue = DealerHand.getValue();

            if (dealerValue > 21)
            {
                LastCreditChange = 1;
                Credits += 1;
                CreditChangeMessage = "+1 Credit";
                GameStatus = $"Dealer busts with {dealerValue}! You win! (You: {playerValue}, Credits: {Credits})";
            }
            else if (playerValue > dealerValue)
            {
                LastCreditChange = 1;
                Credits += 1;
                CreditChangeMessage = "+1 Credit";
                GameStatus = $"You win! (You: {playerValue}, Dealer: {dealerValue}, Credits: {Credits})";
            }
            else if (dealerValue > playerValue)
            {
                LastCreditChange = -1;
                Credits -= 1;
                CreditChangeMessage = "-1 Credit";
                GameStatus = $"Dealer wins! (You: {playerValue}, Dealer: {dealerValue}, Credits: {Credits})";
            }
            else
            {
                LastCreditChange = 0;
                CreditChangeMessage = "Tie - No Change";
                GameStatus = $"Push! It's a tie! (Both: {playerValue}, Credits: {Credits})";
            }

            if (Credits <= 0)
            {
                CurrentGameState = GameState.GameOver;
                GameStatus = "Game Over! You're out of credits! Click 'New Game' to start again with 5 credits.";
            }
            else
            {
                CurrentGameState = GameState.RoundOver;
                GameStatus += " Click 'Next Game' to continue.";
            }
        }

        private void NextGame()
        {
            // Clear previous round data before starting fresh round
            LastCreditChange = 0;
            CreditChangeMessage = "";
            CurrentGameState = GameState.Playing;
            GameStatus = "Game started. Your move!";
            InitializeGame();
        }

        private void NewGame()
        {
            // Reset credits to 5 and start fresh
            credits = 5;
            Credits = 5;

            // Clear previous round data before starting fresh
            LastCreditChange = 0;
            CreditChangeMessage = "";
            CurrentGameState = GameState.Playing;
            GameStatus = "Game started. Your move!";
            InitializeGame();
        }

        private void StopPlaying()
        {
            CurrentGameState = GameState.GameOver;
            GameStatus = $"You stopped playing. Final credits: {Credits}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
