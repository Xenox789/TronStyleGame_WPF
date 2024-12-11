using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Tron.Model;
using Tron.Persistence;
using Tron_WPF.View;
using Tron_WPF.ViewModel;

namespace Tron_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TronGameModel _model = null!;
        private MainWindowViewModel _viewModel = null!;
        private MainWindow _view = null!;
        private DispatcherTimer _timer = null!;

         


        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _model = new TronGameModel(new TronFileDataAccess());
            _model.GameOver += new EventHandler<TronEventArgs>(Model_GameOver);
            _model.NewGame();

            _viewModel = new MainWindowViewModel(_model);
            _viewModel.NewGame += new EventHandler(VM_NewGame);
            _viewModel.ExitGame += new EventHandler(VM_ExitGame);
            _viewModel.LoadGame += new EventHandler(VM_LoadGame);
            _viewModel.SaveGame += new EventHandler(VM_SaveGame);
            


            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.KeyDown += KeyEventHandler;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
            _view.Show();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(800);
            _timer.Tick += new EventHandler(Timer_tick);
            _timer.Start();
        }

        private void Timer_tick(object? sender, EventArgs e) 
        {
            _model.AdvanceGame();
            _timer.Interval -= TimeSpan.FromMilliseconds(4);
        }

        private void Model_GameOver(object? sender, TronEventArgs e)
        {
            _timer.Stop(); 
            switch (e.Winner)
            {
                case 1:
                    MessageBox.Show("Game Over! Blue Won!", "Tron", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    break;
                case 2:
                    MessageBox.Show("Game Over! Red Won!", "Tron", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    break;
                default: throw new Exception("Bad Data");
            }
        }

        private void KeyEventHandler(object? sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                if (_timer.IsEnabled)
                {
                    _timer.Stop();
                } else { _timer.Start(); }
            }
            if (e.Key == Key.A)
            {
                _model.Table.Blue.LeftTurn();
            }
            if (e.Key == Key.D)
            {
                _model.Table.Blue.RightTurn();
            }
            if (e.Key == Key.Left)
            {
                _model.Table.Red.LeftTurn();
            }
            if (e.Key == Key.Right)
            {
                _model.Table.Red.RightTurn();
            }
        }

        private void View_Closing(object? sender, CancelEventArgs e)
        {
            bool restartTimer = _timer.IsEnabled;
            _timer.Stop();
            if (MessageBox.Show("Do you want to quit?", "Tron", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
                if(restartTimer)
                {
                    _timer.Start();
                }
            }
        }

        private void VM_NewGame(object? sender, EventArgs e)
        {
            _model.NewGame();
            _timer.Start();
        }

        private void VM_ExitGame(object? sender, EventArgs e)
        {
            _view.Close();
        }

        private async void VM_LoadGame(object? sender, EventArgs e)
        {
            bool restartTimer = _timer.IsEnabled;
            _timer.Stop();
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Tron Loading";
                ofd.Filter = "Tron table | *.tbl";
                if (ofd.ShowDialog() == true)
                {

                    await _model.LoadGameAsync(ofd.FileName);

                    _timer.Start();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error in loading", "Tron", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (restartTimer)
            {
                _timer.Start();
            }
        }

        private async void VM_SaveGame(object? sender, EventArgs e)
        {
            bool restartTimer = _timer.IsEnabled;
            _timer.Stop();

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Tron Save";
                sfd.Filter = "Tron table | *.tbl";
                if(sfd.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveGameAsync(sfd.FileName);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Game save failed!", "Tron", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Game save failed!", "Tron", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if(restartTimer)
            {
                _timer.Start();
            }
        }
    }
  
}
