using System;
using System.Windows;
using System.Windows.Input;
using Wpf_localiser.pages;

namespace Wpf_localiser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new cartes());
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BtnNavCartes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new cartes());
        }

        private void BtnNavAlerter_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new alerter());
        }

        private void BtnNavAccueil_Click(object sender, RoutedEventArgs e) { }
        private void BtnNavMesures_Click(object sender, RoutedEventArgs e) { }
        private void BtnNavGraphiques_Click(object sender, RoutedEventArgs e) { }
        private void BtnNavPollens_Click(object sender, RoutedEventArgs e) { }
        private void BtnNavParametres_Click(object sender, RoutedEventArgs e) { }
        private void BtnNavSeConnecter_Click(object sender, RoutedEventArgs e) { }
    }
}