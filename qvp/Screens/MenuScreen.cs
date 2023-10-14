using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using qvp.Players;
using qvp.StateManagement;

namespace qvp.Screens
{
    // Base class for screens that contain a menu of options. The user can
    // move up and down to select an entry, or cancel to back out of the screen.
    public class MenuScreen : GameScreen
    {
        private ContentManager _content;
        Texture2D _menuTitle;
        Texture2D _menuTitleQ;
        Texture2D _menuTitleP;
        SoundEffect _readyUpSound;
        SoundEffect _qButtonSound;
        SoundEffect _pButtonSound;

        private readonly List<MenuEntry> _menuEntries = new List<MenuEntry>();
        protected IList<MenuEntry> MenuEntries => _menuEntries;

        private int _selectedEntry;

        private InputAction _menuLeft;
        private InputAction _menuRight;
        private InputAction _menuLeftSelect;
        private InputAction _menuRightSelect;

        private MenuPlayer _playerQ;
        private MenuPlayer _playerP;

        private StatusBar _bgQ;
        private StatusBar _bgP;

        public override void Activate()
        {
            base.Activate();

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _menuLeft = new InputAction(
                new[] { Keys.Q });
            _menuRight = new InputAction(
                new[] { Keys.P });
            _menuLeftSelect = new InputAction(
               new[] { Keys.Q }, false);
            _menuRightSelect = new InputAction(
               new[] { Keys.P }, false);

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _menuTitle = _content.Load<Texture2D>("title");
            _menuTitleQ = _content.Load<Texture2D>("titleQ");
            _menuTitleP = _content.Load<Texture2D>("titleP");
            _readyUpSound = _content.Load<SoundEffect>("pickupCoin");
            _qButtonSound = _content.Load<SoundEffect>("powerUp");
            _pButtonSound = _content.Load<SoundEffect>("powerUp (1)");

            var playGameMenuEntry = new MenuEntry(_content.Load<Texture2D>("playIcon"));
            var exitMenuEntry = new MenuEntry(_content.Load<Texture2D>("exitIcon"));
            var creditsMenuEntry = new MenuEntry(_content.Load<Texture2D>("creditsIcon"));

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            creditsMenuEntry.Selected += CreditsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);
            MenuEntries.Add(creditsMenuEntry);

            _playerQ = new MenuPlayer(new Vector2(120, 12800), _content.Load<Texture2D>("q_mainmenu"));
            _playerP = new MenuPlayer(new Vector2(120, 12800), _content.Load<Texture2D>("p_mainmenu"));

            _bgQ = new StatusBar(_content.Load<Texture2D>("bg_mainmenu_blue"), _readyUpSound);
            _bgP = new StatusBar(_content.Load<Texture2D>("bg_mainmenu_pink"), _readyUpSound);
        }

        // Responds to user input, changing the selected entry and accepting or cancelling the menu.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (_menuLeft.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _selectedEntry--;
                _qButtonSound.Play();

                if (_selectedEntry < 0)
                    _selectedEntry = _menuEntries.Count - 1;
            }

            if (_menuRight.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _selectedEntry++;
                _pButtonSound.Play();

                if (_selectedEntry >= _menuEntries.Count)
                    _selectedEntry = 0;
            }
            if(_menuLeftSelect.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _playerQ.IsPressing = true;
                _bgQ.Update(true);
            }
            else
            {
                _playerQ.IsPressing = false;
            }
            if (_menuRightSelect.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _playerP.IsPressing = true;
                _bgP.Update(true);
            }
            else
            {
                _playerP.IsPressing = false;
            }

            if (_bgQ.IsFull && _bgP.IsFull)
            {
                OnSelectEntry(_selectedEntry, playerIndex);
            }

            _bgQ.Update(false);
            _bgP.Update(false);
        }

        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            _menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            List<GameScreen> games = new List<GameScreen> { new StandoffGameScreen(), new SpaceGameScreen(), new TrackGameScreen(), new BoxingScreen(), new StaringScreen(), new BalloonGameScreen(), new DartsScreen(), new BombGameScreen() };
            // List<GameScreen> games = new List<GameScreen> { new BalloonGameScreen() };
            Random random = new Random();

            for (int i = games.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                GameScreen temp = games[i];
                games[i] = games[j];
                games[j] = temp;
            }

            ScreenManager.Games = games;
            ScreenManager.GameIndex = -1;
            ScreenManager.QPoints = 0;
            ScreenManager.PPoints = 0;
            LoadingScreen.Load(ScreenManager, true, new TransitionScreen());
        }

        private void CreditsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var creditsMessageBox = new MessageBoxScreen("Credits:\n\ngame created by: Kyle Reading\n\nmusic: 'Game Over' by Danijel Zambo\n@ uppbeat.io\n\nthis font: November\nby Brandon Schoepf @ tepidmonkey.com\n\npress (ESC) to exit");
            ScreenManager.AddScreen(creditsMessageBox);
        }

        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            var exitMessageBox = new MessageBoxScreen("Exit?\n\n(Space) to exit\n(ESC) to cancel");
            ScreenManager.AddScreen(exitMessageBox);

            exitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
        }

        // Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            _bgQ.Draw(spriteBatch);
            _bgP.Draw(spriteBatch);
            _playerP.Draw(spriteBatch);
            _playerQ.Draw(spriteBatch);

            for (int i = 0; i < _menuEntries.Count; i++)
            {
                var menuEntry = _menuEntries[i];
                bool isSelected = IsActive && i == _selectedEntry;
                menuEntry.Draw(this, isSelected, gameTime);
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_menuTitle, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            if (_bgQ.IsFull)
            {
                spriteBatch.Draw(_menuTitleQ, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            }
            if (_bgP.IsFull) spriteBatch.Draw(_menuTitleP, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
