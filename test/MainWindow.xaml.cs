﻿using System.Windows;
using System.Windows.Controls;

namespace test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            lwUserWallet.ItemsSource = Data.userWallet.List;
            lwVMWallet.ItemsSource = Data.vm.Account.List;
            lwPriceList.ItemsSource = Data.vm.PriceList;
            tbAccount.DataContext = Data.vm;

            Data.userWallet.AddCoins(new Coins(1, 5));
            Data.userWallet.AddCoins(new Coins(2, 5));
            Data.userWallet.AddCoins(new Coins(5, 5));
            Data.userWallet.AddCoins(new Coins(10, 5));

            Data.vm.AddToAccount(new Coins(1, 1));
            Data.vm.AddToAccount(new Coins(2, 1));
            Data.vm.AddToAccount(new Coins(5, 1));
            Data.vm.AddToAccount(new Coins(10,0));

            Data.vm.PriceList.Add(new Items("Чай", 13, 10));
            Data.vm.PriceList.Add(new Items("Кофе", 18, 20));
            Data.vm.PriceList.Add(new Items("Кофе с молоком", 21, 20));
            Data.vm.PriceList.Add(new Items("Сок", 35, 15));
        }

        private void AddCoins_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Coins coins = button.DataContext as Coins;
            wCoinsEx w = new wCoinsEx(coins);
            w.ShowDialog();
        }

        private void GetItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Items item = button.DataContext as Items;
            Data.vm.BuyItem(item);
        }

        private void Coins_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            Data.vm.GetChange();
        }

        private void lwPriceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem lwi = sender as ListViewItem;
            Items item = lwi.DataContext as Items;
            Data.vm.BuyItem(item);
        }

        private void lwUserWallet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem lwi = sender as ListViewItem;
            Coins coins = lwi.DataContext as Coins;
            wCoinsEx w = new wCoinsEx(coins);
            w.ShowDialog();
        }
    }
}
