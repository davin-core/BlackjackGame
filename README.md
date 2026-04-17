# Blackjack Game

A WPF-based Blackjack card game implementation in C#.

## Features

- Play against the dealer
- Track credits/winnings
- Hit and Stand actions
- Multiple rounds with Next Game option
- Visual card display with card images
- Game status updates

## Project Structure

- **Model/** - Card game logic (Card, Hand, Desk, Enums)
- **ViewModels/** - BlackjackViewModel for game state
- **Views/** - XAML UI components (BlackjackView)
- **Common/** - Shared utilities (RelayCommand)
- **Assets/Cards/** - Card image resources

## Requirements

- .NET 10.0 (Windows)
- Visual Studio or VS Code with C# extension

## Getting Started

1. Open `BlackjackGame.slnx` in Visual Studio
2. Build the solution
3. Run the application

## How to Play

- Click **Hit** to add a card to your hand
- Click **Stand** to end your turn and let the dealer play
- Click **Next Game** to play another round with current credits
- Click **New Game** to start fresh with default credits

## Technologies

- WPF (Windows Presentation Foundation)
- MVVM pattern
- C# .NET 10.0

## License

MIT License - See LICENSE file for details