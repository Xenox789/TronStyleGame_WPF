using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tron.Persistence;

namespace Tron.Model
{
    public enum BoardSize
    {
        Small, Medium, Large
    }
    public class TronGameModel
    {
        private ITronDataAccess _dataAccess;
        private TronTable _table;
        private int? _winner;
        private BoardSize _bs;

        public bool _isGameOver;
        public BoardSize BoardSize
        {
            get { return _bs; }
            set { _bs = value; }
        }
        public TronTable Table { get { return _table; } }


        public event EventHandler<StepEventArgs>? GameAdvanced;
        public event EventHandler<TronEventArgs>? GameOver;
        public event EventHandler<GameEventArgs>? GameCreated;

        

        public TronGameModel(ITronDataAccess dataAccess)
        {

            _dataAccess = dataAccess;
            _table = new TronTable(24);
            _bs = BoardSize.Medium;
            _isGameOver = false;
        }

        public void NewGame()
        {
            switch(_bs)
            {
                case BoardSize.Small:
                    _table = new TronTable(12);
                    break;
                case BoardSize.Medium:
                    _table = new TronTable(24);
                    break;
                case BoardSize.Large:
                    _table = new TronTable(36);
                    break;
            }
            OnGameCreated();
            _isGameOver = false;
            
        }
        public void AdvanceGame()
        {
            if (!_isGameOver)
            {
                _table.MovePlayers();
                OnGameAdvanced();

                if (_table.Hit() != 0)
                {
                    _isGameOver = true;
                    _winner = _table.Hit();
                    OnGameOver();
                }
                else
                {
                    _table.setPlayers();
                }
            }
        }


        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException();
            _table = await _dataAccess.LoadAsync(path);
        }

        public async Task SaveGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException();
            await _dataAccess.SaveAsync(path, _table);
        }

        private void OnGameAdvanced()
        {
            GameAdvanced?.Invoke(this, new StepEventArgs(_table.Blue.X,_table.Blue.Y, _table.Red.X, _table.Red.Y));
        }

        private void OnGameOver()
        {
                GameOver?.Invoke(this, new TronEventArgs(_winner));
        }

        private void OnGameCreated()
        {
            GameCreated?.Invoke(this, new GameEventArgs(_table.GridSize));
        }




    }
}
