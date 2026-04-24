using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MemoryLibrary;

namespace MemoryGame
{
    public partial class Form1 : Form
    {
        // Игровая логика
        private GameController _game = new GameController();
        private Button? _first;
        private Button? _second;

        // Таймеры
        private System.Windows.Forms.Timer _flipTimer = new System.Windows.Forms.Timer { Interval = 750 };
        private System.Windows.Forms.Timer _gameClock = new System.Windows.Forms.Timer { Interval = 1000 };

        // Статистика
        private Label _lblTime = new Label();
        private Label _lblMoves = new Label();
        private int _secondsElapsed = 0;

        // Элементы управления
        private ComboBox _cbPairs = new ComboBox();
        private Button _btnNewGame = new Button();

        // Темы
        private enum AppTheme { Light, Dark }
        private AppTheme _currentTheme = AppTheme.Light;
        private Button _btnTheme = new Button();

        // Пауза
        private Button _btnPause = new Button();
        private bool _paused = false;

        public Form1()
        {
            SetupInterface();
            _flipTimer.Tick += FlipTimer_Tick;
            _gameClock.Tick += GameClock_Tick;
            StartGame();
        }

        // Размещаем все элементы управления на форме
        private void SetupInterface()
        {
            this.Text = "Memory Game";
            this.Size = new Size(450, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(450, 400);

            // Надпись времени
            _lblTime.Text = "Время: 0 сек.";
            _lblTime.Location = new Point(20, 10);
            _lblTime.AutoSize = true;
            _lblTime.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(_lblTime);

            // Надпись ходов
            _lblMoves.Text = "Ходы: 0";
            _lblMoves.Location = new Point(300, 10);
            _lblMoves.AutoSize = true;
            _lblMoves.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Controls.Add(_lblMoves);

            // Выбор количества пар
            _cbPairs.Items.AddRange(new object[] { "4 пары", "8 пар", "12 пар" });
            _cbPairs.SelectedIndex = 0;
            _cbPairs.Location = new Point(20, 40);
            _cbPairs.Size = new Size(100, 24);
            _cbPairs.DropDownStyle = ComboBoxStyle.DropDownList;
            _cbPairs.SelectedIndexChanged += CbPairs_SelectedIndexChanged;
            this.Controls.Add(_cbPairs);

            // Кнопка "Новая игра"
            _btnNewGame.Text = "Новая игра";
            _btnNewGame.Location = new Point(130, 40);
            _btnNewGame.Size = new Size(100, 30);
            _btnNewGame.Click += BtnNewGame_Click;
            this.Controls.Add(_btnNewGame);

            // Переключатель темы
            _btnTheme.Text = _currentTheme == AppTheme.Light ? "Тёмная" : "Светлая";
            _btnTheme.Location = new Point(240, 40);
            _btnTheme.Size = new Size(75, 30);
            _btnTheme.Click += BtnTheme_Click;
            this.Controls.Add(_btnTheme);

            // Пауза
            _btnPause.Text = "Пауза";
            _btnPause.Location = new Point(325, 40);
            _btnPause.Size = new Size(80, 30);
            _btnPause.Click += BtnPause_Click;
            this.Controls.Add(_btnPause);
        }

        // Смена количества пар -> перезапуск
        private void CbPairs_SelectedIndexChanged(object? sender, EventArgs e) => StartGame();

        // Ручной перезапуск
        private void BtnNewGame_Click(object? sender, EventArgs e)
        {
            if (_paused) TogglePause();
            StartGame();
        }

        // Циклическое переключение темы
        private void BtnTheme_Click(object? sender, EventArgs e)
        {
            _currentTheme = _currentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
            _btnTheme.Text = _currentTheme == AppTheme.Light ? "Тёмная" : "Светлая";
            ApplyTheme();
        }

        // Применяем тему ко всем элементам интерфейса
        private void ApplyTheme()
        {
            Color backForm, foreForm, backButton, foreButton, backControl, foreControl;
            if (_currentTheme == AppTheme.Light)
            {
                backForm = SystemColors.Control;
                foreForm = SystemColors.ControlText;
                backButton = Color.LightGray;
                foreButton = SystemColors.ControlText;
                backControl = SystemColors.Window;
                foreControl = SystemColors.ControlText;
            }
            else
            {
                backForm = Color.FromArgb(30, 30, 30);
                foreForm = Color.White;
                backButton = Color.FromArgb(60, 60, 60);
                foreButton = Color.White;
                backControl = Color.FromArgb(50, 50, 50);
                foreControl = Color.White;
            }

            this.BackColor = backForm;
            this.ForeColor = foreForm;

            _lblTime.ForeColor = foreForm;
            _lblMoves.ForeColor = foreForm;

            _cbPairs.BackColor = backControl;
            _cbPairs.ForeColor = foreControl;
            _btnNewGame.BackColor = backControl;
            _btnNewGame.ForeColor = foreControl;
            _btnTheme.BackColor = backControl;
            _btnTheme.ForeColor = foreControl;
            _btnPause.BackColor = backControl;
            _btnPause.ForeColor = foreControl;

            // Красим карты в зависимости от их состояния
            foreach (Control c in this.Controls)
            {
                if (c is Button btn && btn != _btnNewGame && btn != _btnTheme && btn != _btnPause)
                {
                    if (btn.Tag is MemoryCard card)
                    {
                        if (card.IsMatched)
                        {
                            btn.BackColor = Color.LightGreen;
                            btn.ForeColor = Color.Black;
                        }
                        else if (card.IsRevealed)
                        {
                            btn.BackColor = Color.White;
                            btn.ForeColor = _currentTheme == AppTheme.Light ? SystemColors.ControlText : Color.Black;
                        }
                        else
                        {
                            btn.BackColor = backButton;
                            btn.ForeColor = foreButton;
                        }
                    }
                }
            }
        }

        // Пауза/продолжение
        private void BtnPause_Click(object? sender, EventArgs e) => TogglePause();

        private void TogglePause()
        {
            _paused = !_paused;
            if (_paused)
            {
                _gameClock.Stop();
                _flipTimer.Stop();
                _btnPause.Text = "Продолж.";
            }
            else
            {
                if (!_game.IsGameOver)
                    _gameClock.Start();
                _btnPause.Text = "Пауза";
            }
        }

        // Запуск/перезапуск игры
        private void StartGame()
        {
            if (_paused) TogglePause();

            _gameClock.Stop();
            _flipTimer.Stop();
            _secondsElapsed = 0;

            // Удаляем кнопки карт (все, кроме управляющих)
            var toRemove = new List<Control>();
            foreach (Control c in this.Controls)
                if (c is Button btn && btn != _btnNewGame && btn != _btnTheme && btn != _btnPause)
                    toRemove.Add(c);
            foreach (var b in toRemove) this.Controls.Remove(b);

            // Число пар из комбобокса
            int pairsCount = 4;
            if (_cbPairs.SelectedIndex >= 0)
            {
                string? sel = _cbPairs.SelectedItem as string;
                if (sel != null && sel.StartsWith("4")) pairsCount = 4;
                else if (sel != null && sel.StartsWith("8")) pairsCount = 8;
                else if (sel != null && sel.StartsWith("12")) pairsCount = 12;
            }

            _game.InitializeGame(pairsCount);
            _secondsElapsed = 0;
            UpdateStats();
            _gameClock.Start();

            // Сетка: оптимизируем число столбцов/строк
            int total = _game.Cards.Count;
            (int cols, int rows) = GetOptimalGrid(total);

            const int cardW = 80, cardH = 80;
            const int marginX = 25, marginY = 90;
            const int gapX = 15, gapY = 15;

            // Цвета закрытой карты по теме
            Color closedBack = _currentTheme == AppTheme.Light ? Color.LightGray : Color.FromArgb(60, 60, 60);
            Color closedFore = _currentTheme == AppTheme.Light ? SystemColors.ControlText : Color.White;

            // Создаём кнопки карт
            for (int i = 0; i < _game.Cards.Count; i++)
            {
                Button btn = new Button
                {
                    Width = cardW,
                    Height = cardH,
                    Left = marginX + (i % cols) * (cardW + gapX),
                    Top = marginY + (i / cols) * (cardH + gapY),
                    Tag = _game.Cards[i],
                    BackColor = closedBack,
                    Font = new Font("Arial", 18, FontStyle.Bold),
                    ForeColor = closedFore
                };
                btn.Click += Btn_Click;
                btn.MouseEnter += Btn_MouseEnter;
                btn.MouseLeave += Btn_MouseLeave;
                this.Controls.Add(btn);
            }

            // Размер формы под сетку
            int formW = marginX * 2 + cols * (cardW + gapX) - gapX + 16;
            int formH = marginY + rows * (cardH + gapY) - gapY + 40;
            formW = Math.Max(formW, 450);
            this.ClientSize = new Size(formW, formH);

            // Сдвигаем метку ходов к правому краю
            _lblMoves.PerformLayout();
            _lblMoves.Left = this.ClientSize.Width - _lblMoves.Width - 20;

            ApplyTheme();
        }

        // Подбираем симпатичную сетку (минимум пустых ячеек)
        private (int cols, int rows) GetOptimalGrid(int totalCards)
        {
            int bestCols = 1, bestRows = totalCards, bestEmpty = int.MaxValue;
            int start = (int)Math.Ceiling(Math.Sqrt(totalCards));
            for (int cols = start; cols <= totalCards; cols++)
            {
                int rows = (int)Math.Ceiling((double)totalCards / cols);
                int empty = cols * rows - totalCards;
                if (empty < bestEmpty || (empty == bestEmpty && Math.Abs(cols - rows) < Math.Abs(bestCols - bestRows)))
                {
                    bestEmpty = empty;
                    bestCols = cols;
                    bestRows = rows;
                }
                if (empty == 0) break;
            }
            return (bestCols, bestRows);
        }

        // Подсветка при наведении на закрытую карту
        private void Btn_MouseEnter(object? sender, EventArgs e)
        {
            if (_paused) return;
            if (sender is Button btn && btn.Tag is MemoryCard card && !card.IsRevealed && !card.IsMatched)
            {
                btn.BackColor = Color.Silver;
                if (_currentTheme == AppTheme.Dark) btn.ForeColor = Color.Black;
            }
        }

        private void Btn_MouseLeave(object? sender, EventArgs e)
        {
            if (_paused) return;
            if (sender is Button btn && btn.Tag is MemoryCard card && !card.IsRevealed && !card.IsMatched)
            {
                btn.BackColor = _currentTheme == AppTheme.Light ? Color.LightGray : Color.FromArgb(60, 60, 60);
                btn.ForeColor = _currentTheme == AppTheme.Light ? SystemColors.ControlText : Color.White;
            }
        }

        // Обновление счётчиков на экране
        private void UpdateStats()
        {
            _lblTime.Text = $"Время: {_secondsElapsed} сек.";
            _lblMoves.Text = $"Ходы: {_game.Moves}";
        }

        // Секундомер
        private void GameClock_Tick(object? sender, EventArgs e)
        {
            if (_paused) return;
            _secondsElapsed++;
            UpdateStats();
        }

        // Обработка клика по карте
        private void Btn_Click(object? sender, EventArgs e)
        {
            if (_paused || _flipTimer.Enabled || sender is not Button btn) return;
            if (btn.Tag is not MemoryCard card || card.IsRevealed || card.IsMatched || btn == _first) return;

            // Открываем карту
            btn.Text = card.PairId.ToString();
            btn.BackColor = Color.White;
            btn.ForeColor = _currentTheme == AppTheme.Dark ? Color.Black : SystemColors.ControlText;
            card.IsRevealed = true;

            if (_first == null)
            {
                _first = btn;
            }
            else
            {
                _second = btn;
                _game.IncrementMoves();
                UpdateStats();

                if (_game.CheckMatch(_first.Tag as MemoryCard, _second.Tag as MemoryCard))
                {
                    // Пара найдена
                    _first.BackColor = _second.BackColor = Color.LightGreen;
                    _first.ForeColor = _second.ForeColor = Color.Black;
                    _first = _second = null;

                    if (_game.IsGameOver)
                    {
                        _gameClock.Stop();
                        MessageBox.Show($"Победа!\nВремя: {_secondsElapsed} сек.\nХодов: {_game.Moves}", "Конец игры");
                        StartGame();
                    }
                }
                else
                {
                    _flipTimer.Start(); // запускаем таймер для закрытия
                }
            }
        }

        // Закрываем несовпавшие карты после задержки
        private void FlipTimer_Tick(object? sender, EventArgs e)
        {
            _flipTimer.Stop();
            if (_paused) return;

            if (_first != null && _second != null)
            {
                _first.Text = _second.Text = "";
                _first.BackColor = _second.BackColor = _currentTheme == AppTheme.Light ? Color.LightGray : Color.FromArgb(60, 60, 60);
                _first.ForeColor = _second.ForeColor = _currentTheme == AppTheme.Light ? SystemColors.ControlText : Color.White;

                if (_first.Tag is MemoryCard c1) c1.IsRevealed = false;
                if (_second.Tag is MemoryCard c2) c2.IsRevealed = false;
            }
            _first = _second = null;
        }
    }
}