using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tron.Model;

namespace Tron_WPF.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        public ObservableCollection<TronField> Fields { get; set; }
        public TronGameModel _model;




        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }

        public DelegateCommand ExitGameCommand { get; private set; }


        public int GridSize
        {
            get; private set;
        }
        public Boolean IsGameSmall
        {
            get { return _model.BoardSize == BoardSize.Small; }
            set
            {
                if (_model.BoardSize == BoardSize.Small)
                    return;
                _model.BoardSize = BoardSize.Small;
                OnPropertyChanged(nameof(IsGameSmall));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameLarge));
            }
        }

        public Boolean IsGameMedium
        {
            get { return _model.BoardSize == BoardSize.Medium; }
            set
            {
                if (_model.BoardSize == BoardSize.Medium)
                    return;
                _model.BoardSize = BoardSize.Medium;
                OnPropertyChanged(nameof(IsGameSmall));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameLarge));
            }
        }

        public Boolean IsGameLarge
        {
            get { return _model.BoardSize == BoardSize.Large; }
            set
            {
                if (_model.BoardSize == BoardSize.Large)
                    return;

                _model.BoardSize = BoardSize.Large;
                OnPropertyChanged(nameof(IsGameSmall));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameLarge));
            }
        }




        public event EventHandler? NewGame;
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;
        public event EventHandler? ExitGame;

        public MainWindowViewModel(TronGameModel model)
        {
            _model = model;
            _model.GameAdvanced += new EventHandler<StepEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<TronEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<GameEventArgs>(Model_GameCreated);

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitGameCommand = new DelegateCommand(param => OnExitGame());


            GridSize = _model.Table.GridSize;

            Fields = new ObservableCollection<TronField>();
            for (int i = 0; i < _model.Table.GridSize; i++)
            {
                for (int j = 0; j < _model.Table.GridSize; j++)
                {
                    Fields.Add(new TronField
                    {
                        CellValue = _model.Table.Grid[i, j],
                        X = j,
                        Y = i,
                    }
                    );
                }
            }

            RefreshFields();
        }

        private void RefreshFields()
        {
            foreach (var field in Fields)
            {
                field.CellValue = _model.Table.Grid[field.Y, field.X];
                OnPropertyChanged(nameof(field.CellValue));
            }
        }


        private void Model_GameOver(object? sender, TronEventArgs e)
        {
            foreach (TronField field in Fields)
            {
                field.CellValue = e.Winner;
                OnPropertyChanged(nameof(field.CellValue));
            }
        }

        private void Model_GameAdvanced(object? sender, StepEventArgs e)
        {
            RefreshFields();
        }

        private void Model_GameCreated(object? sender, GameEventArgs e)
        {
            GridSize = _model.Table.GridSize;
            Fields.Clear();
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Fields.Add(new TronField
                    {
                        CellValue = 0,
                        X = j,
                        Y = i,
                    }
                    );
                }
            }
            OnPropertyChanged(nameof(GridSize));
            RefreshFields();
        }



        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);          
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }


        
    }
}
