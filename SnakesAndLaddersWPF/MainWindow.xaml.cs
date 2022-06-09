using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SnakesAndLaddersWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GameState gameState = new();
        private readonly Dictionary<Player, ImageSource> imageSources;
        private readonly Image[,] imageControls = new Image[10, 10];
        private readonly Random random = new();

        private readonly DoubleAnimation fadeOutAnimation = new()
        {
            Duration = TimeSpan.FromMilliseconds(600),
            From = 1,
            To = 0
        };

        private readonly DoubleAnimation fadeInAnimation = new()
        {
            Duration = TimeSpan.FromMilliseconds(600),
            From = 0,
            To = 1
        };

        public int Die1 { get; set; }
        public int Die2 { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            imageSources = new()
            {
                {gameState.Players[0], new BitmapImage(new Uri("Assets/player1.png", UriKind.Relative)) },
                {gameState.Players[1], new BitmapImage(new Uri("Assets/player2.png", UriKind.Relative)) }
            };
            SetupGameGrid();
            SetupGame();

            gameState.MoveMade += OnMoveMade;
            gameState.GameEnded += OnGameEnded;
            gameState.GameRestarted += OnGameRestarted;
        }

        private void SetupGameGrid()
        {
            for (int r = 0; r < gameState.Board.Side; r++)
            {
                for (int c = 0; c < gameState.Board.Side; c++)
                {
                    Image imageControl = new();
                    GameGrid.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
        }

        private void SetupGame()
        {
            CurrentPlayer.Text = gameState.CurrentPlayer.Name;
            PlayerImage.Source = imageSources[gameState.CurrentPlayer];
            TurnPassed.Text = gameState.TurnsPassed.ToString();
            imageControls[CoordinatesOf(1).Item1, CoordinatesOf(1).Item2].Source = new BitmapImage(new Uri("Assets/shared.png", UriKind.Relative));
        }

        private void MoveClick(object sender, RoutedEventArgs e)
        {
            if (PlayerShareBoardField())
            {
                imageControls[CoordinatesOf(gameState.CurrentPlayer.Position).Item1, CoordinatesOf(gameState.CurrentPlayer.Position).Item2].Source = imageSources[gameState.Players.First(p => p != gameState.CurrentPlayer)];
            }
            else
            {
                imageControls[CoordinatesOf(gameState.CurrentPlayer.Position).Item1, CoordinatesOf(gameState.CurrentPlayer.Position).Item2].Source = null;
            }
            gameState.MakeMove(Die1, Die2);
            CurrentPlayer.Text = gameState.CurrentPlayer.Name;
            PlayerImage.Source = imageSources[gameState.CurrentPlayer];
            TurnPassed.Text = gameState.TurnsPassed.ToString();
            Dublet.Visibility = Visibility.Hidden;
        }

        private void OnMoveMade(int position)
        {
            if (PlayerShareBoardField())
            {
                imageControls[CoordinatesOf(position).Item1, CoordinatesOf(position).Item2].Source = new BitmapImage(new Uri("Assets/shared.png", UriKind.Relative));
            }
            else
            {
                imageControls[CoordinatesOf(position).Item1, CoordinatesOf(position).Item2].Source = imageSources[gameState.CurrentPlayer];
            }
            RollDicesButton.IsEnabled = true;
            MoveButton.IsEnabled = false;
        }

        private async void OnGameRestarted()
        {
            for (int r = 0; r < gameState.Board.Side; r++)
            {
                for (int c = 0; c < gameState.Board.Side; c++)
                {
                    imageControls[r, c].Source = null;
                }
            }

            SetupGame();
            await TransitionToGameScreen();
        }

        private async void OnGameEnded(GameResult gameResult)
        {
            await Task.Delay(1500);
            await TransitionToEndScreen($"{gameResult.Winner!.Name} wins in {gameState.TurnsPassed} turns", imageSources[gameResult.Winner]);
        }

        private async Task TransitionToGameScreen()
        {
            //EndScreen.Visibility = Visibility.Hidden;
            await FadeOut(EndScreen);
            //TurnPanel.Visibility = Visibility.Visible;
            //GameCanvas.Visibility = Visibility.Visible;
            //MoveAndRoll.Visibility = Visibility.Visible;
            //CurrentPlayerPanel.Visibility = Visibility.Visible;
            await Task.WhenAll(FadeIn(TurnPanel), FadeIn(GameCanvas), FadeIn(MoveAndRoll), FadeIn(CurrentPlayerPanel));
        }

        private async Task TransitionToEndScreen(string text, ImageSource winnerImage)
        {
            //TurnPanel.Visibility = Visibility.Hidden;
            //GameCanvas.Visibility = Visibility.Hidden;
            //CurrentPlayerPanel.Visibility = Visibility.Hidden;
            //MoveAndRoll.Visibility = Visibility.Hidden;
            await Task.WhenAll(FadeOut(TurnPanel), FadeOut(GameCanvas), FadeOut(CurrentPlayerPanel), FadeOut(MoveAndRoll));
            WinnerImage.Source = winnerImage;
            ResultText.Text = text;
            //EndScreen.Visibility = Visibility.Visible;
            await FadeIn(EndScreen);
        }

        public Tuple<int, int> CoordinatesOf(int boardPosition)
        {
            for (int r = 0; r < gameState.GameGrid.BoardFieldValues.GetLength(0); ++r)
            {
                for (int c = 0; c < gameState.GameGrid.BoardFieldValues.GetLength(1); ++c)
                {
                    if (gameState.GameGrid.BoardFieldValues[r, c].Equals(boardPosition))
                        return Tuple.Create(r, c);
                }
            }

            return Tuple.Create(-1, -1);
        }

        private bool PlayerShareBoardField()
        {
            return gameState.Players.All(p => p.Position.Equals(gameState.CurrentPlayer.Position));
        }

        private int RollDice() => random.Next(1, 7);

        private void RollDicesClick(object sender, RoutedEventArgs e)
        {
            Die1 = RollDice();
            Die2 = RollDice();
            Dice1.Text = $"Dice1: {Die1}";
            Dice2.Text = $"Dice2: {Die2}";
            RollDicesButton.IsEnabled = false;
            MoveButton.IsEnabled = true;
            if (Die1 == Die2)
            {
                Dublet.Visibility = Visibility.Visible;
            }
            else
            {
                Dublet.Visibility = Visibility.Hidden;
            }
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            if (gameState.GameOver)
            {
                gameState.Reset();
            }
        }

        private void QuitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async Task FadeOut(UIElement e)
        {
            e.BeginAnimation(OpacityProperty, fadeOutAnimation);
            await Task.Delay(fadeOutAnimation.Duration.TimeSpan);
            e.Visibility = Visibility.Hidden;
        }

        private async Task FadeIn(UIElement e)
        {
            e.BeginAnimation(OpacityProperty, fadeInAnimation);
            await Task.Delay(fadeInAnimation.Duration.TimeSpan);
            e.Visibility = Visibility.Visible;
        }
    }
}