using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BlackjackGame.Common;
using BlackjackGame.Model;
using BlackjackGame.Model.Enums;    

namespace BlackjackGame.ViewModels
{
    public class BlackjackViewModel : INotifyPropertyChanged
    {
        private GameEngine gameEngine;
        private Hand dealerHand;
        private Hand playerHand;
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

        public Hand DealerHand
        {
            get => dealerHand;
            private set
            {
                if (dealerHand != value)
                {
                    if (dealerHand != null)
                    {
                        dealerHand.PropertyChanged -= Hand_PropertyChanged;
                    }
                    dealerHand = value;
                    if (dealerHand != null)
                    {
                        dealerHand.PropertyChanged += Hand_PropertyChanged;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DealerHandValue));
                }
            }
        }

        public Hand PlayerHand
        {
            get => playerHand;
            private set
            {
                if (playerHand != value)
                {
                    if (playerHand != null)
                    {
                        playerHand.PropertyChanged -= Hand_PropertyChanged;
                    }
                    playerHand = value;
                    if (playerHand != null)
                    {
                        playerHand.PropertyChanged += Hand_PropertyChanged;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PlayerHandValue));
                }
            }
        }

        private void Hand_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Hand.Value) || e.PropertyName == nameof(Hand.IsBusted))
            {
                if (sender == dealerHand)
                {
                    OnPropertyChanged(nameof(DealerHandValue));
                }
                else if (sender == playerHand)
                {
                    OnPropertyChanged(nameof(PlayerHandValue));
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
            gameEngine = new GameEngine();
            gameState = GameState.Playing;
            credits = gameEngine.Credits;
            gameStatus = "Game started. Your move!";
            lastCreditChange = 0;
            creditChangeMessage = "";
            InitializeRound();
        }

        private void InitializeRound()
        {
            gameEngine.InitializeRound();

            // Update hand references (this triggers property change notifications through our handlers)
            DealerHand = gameEngine.DealerHand;
            PlayerHand = gameEngine.PlayerHand;

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

            var hitResult = gameEngine.ProcessHit();

            if (hitResult.IsBusted)
            {
                CurrentGameState = GameState.PlayerBusted;
                LastCreditChange = hitResult.CreditsChanged;
                Credits = gameEngine.Credits;
                CreditChangeMessage = "-1 Credit";

                if (hitResult.IsGameOver)
                {
                    CurrentGameState = GameState.GameOver;
                    GameStatus = "Game Over! You're out of credits! Click 'New Game' to start again.";
                }
                else
                {
                    CurrentGameState = GameState.RoundOver;
                    GameStatus = $"Bust! You have {hitResult.PlayerValue}. Dealer wins! Click 'Next Game' to continue.";
                }
            }
            else if (hitResult.AutoStand)
            {
                // Player got 21 after hitting, automatically stand
                GameStatus = $"You have 21! Dealer's turn...";
                Stand();
            }
            else
            {
                GameStatus = $"You have {hitResult.PlayerValue}. Hit or Stand?";
            }
        }

        private void Stand()
        {
            if (CurrentGameState != GameState.Playing)
                return;

            CurrentGameState = GameState.StandingWait;
            GameStatus = "Dealer's turn...";

            // Reveal dealer's hole card and dealer plays
            var roundResult = gameEngine.ProcessStand();

            DetermineWinner(roundResult);
        }

        private void DetermineWinner(RoundResult roundResult)
        {
            Credits = gameEngine.Credits;
            LastCreditChange = roundResult.CreditsChanged;

            if (roundResult.CreditsChanged == 1)
            {
                CreditChangeMessage = "+1 Credit";
            }
            else if (roundResult.CreditsChanged == -1)
            {
                CreditChangeMessage = "-1 Credit";
            }
            else
            {
                CreditChangeMessage = "Tie - No Change";
            }

            if (roundResult.IsGameOver)
            {
                CurrentGameState = GameState.GameOver;
                GameStatus = $"{roundResult.Outcome} Game Over! You're out of credits! Click 'New Game' to start again with 5 credits.";
            }
            else
            {
                CurrentGameState = GameState.RoundOver;
                GameStatus = $"{roundResult.Outcome} (Credits: {Credits}) Click 'Next Game' to continue.";
            }
        }

        private void NextGame()
        {
            // Clear previous round data before starting fresh round
            LastCreditChange = 0;
            CreditChangeMessage = "";
            CurrentGameState = GameState.Playing;
            GameStatus = "Game started. Your move!";
            InitializeRound();
        }

        private void NewGame()
        {
            // Reset game engine to start fresh with 5 credits
            gameEngine = new GameEngine();
            Credits = gameEngine.Credits;

            // Clear previous round data before starting fresh
            LastCreditChange = 0;
            CreditChangeMessage = "";
            CurrentGameState = GameState.Playing;
            GameStatus = "Game started. Your move!";
            InitializeRound();
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
