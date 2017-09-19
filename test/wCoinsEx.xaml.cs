using System;
using System.Windows;

namespace test
{
    public partial class wCoinsEx : Window
    {
        public Coins c;

        public wCoinsEx(Coins coins)
        {
            InitializeComponent();
            c = coins;
            lbCount.Content = c.Count == 0 ? 0 : 1;
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            int v = Convert.ToInt32(lbCount.Content) - 1;
            if (v >= 0) lbCount.Content = v;
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            int v = Convert.ToInt32(lbCount.Content) + 1;
            if (v <= c.Count) lbCount.Content = v;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int v = Convert.ToInt32(lbCount.Content);
            Data.vm.Add(new Coins(c.CoinType, v));
            Data.userWallet.GetCoins(new Coins(c.CoinType, v));
            Close();
        }
    }
}
