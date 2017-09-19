using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace test
{
    public class Coins : INotifyPropertyChanged
    {
        public Coins(int value, int count)
        {
            _value = value;
            Count = count;
        }

        private int _value;
        private int _count;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        public int CoinType { get { return _value; } }

        public override string ToString()
        {
            return string.Format("{0} руб = {1} штук", _value, _count);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Wallet : INotifyPropertyChanged
    {
        public Wallet() { }
        
        private int _account = 0;
        private Dictionary<int, int> _coins = new Dictionary<int, int>();
        private ObservableCollection<Coins> _list = new ObservableCollection<Coins>();

        public int Account
        {
            get { return _account; }
            private set
            {
                _account = value;
                OnPropertyChanged("Account");
            }
        }

        public ObservableCollection<Coins> List
        {
            get { return _list; }
            private set
            {
                _list = value;
                OnPropertyChanged("List");
            }
        }

        public void AddCoins(Coins coins)
        {
            Account += coins.CoinType * coins.Count;
            if (!_coins.ContainsKey(coins.CoinType)) _coins[coins.CoinType] = coins.Count;
            else _coins[coins.CoinType] += coins.Count;
            UpdateList();
        }

        public void GetCoins(Coins coins)
        {
            if (_coins.ContainsKey(coins.CoinType))
            {
                _coins[coins.CoinType] -= coins.Count;
                Account -= coins.CoinType * coins.Count;
                UpdateList();
            }
        }

        public void GetCoins(int sum, Wallet toDest)
        {
            if (sum <= _account)
            {
                int count;
                int amount = sum;
                List<Coins> change = new List<Coins>();
                int c = _coins.Keys.Max();
                while (amount % c != 0)
                {
                    count = amount / c;
                    if (count > 0)
                    {
                        if (_coins[c] >= count)
                        {
                            amount -= c * count;
                            change.Add(new Coins(c, count));
                            //_coins[c] -= count;
                            //toDest.AddCoins(new Coins(c, count));
                        }
                        else
                        {
                            amount -= _coins[c] * c;
                            change.Add(new Coins(c, _coins[c]));
                            //toDest.AddCoins(new Coins(c, _coins[c]));
                            //_coins[c] = 0;
                        }
                    }

                    if (c == _coins.Keys.Min()) break;
                    c = _coins.Keys.Where(x => x < c).Max();
                }
                count = amount / c;
                if (count > 0)
                {
                    if (_coins[c] >= count)
                    {
                        amount -= c * count;
                        change.Add(new Coins(c, count));
                        //_coins[c] -= count;
                        //toDest.AddCoins(new Coins(c, count));
                    }
                    else
                    {
                        amount -= _coins[c] * c;
                        change.Add(new Coins(c, _coins[c]));
                        //toDest.AddCoins(new Coins(c, _coins[c]));
                        //_coins[c] = 0;
                    }
                }

                if (amount > 0) MessageBox.Show("Не хватает монет");
                else
                {
                    foreach (Coins m in change)
                    {
                        _coins[m.CoinType] -= m.Count;
                        toDest.AddCoins(m);
                    }
                    Account -= sum;
                }
                UpdateList();
            }
        }

        private void UpdateList()
        {
            List.Clear();
            foreach (KeyValuePair<int, int> entry in _coins)
                List.Add(new Coins(entry.Key, entry.Value));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Items : INotifyPropertyChanged
    {
        public Items(string itemType, int price, int count)
        {
            Price = price;
            Type = itemType;
            Count = count;
        }

        private int _count;
        private int _price;
        private string _type;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        public int Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1} руб, {2} порций", _type, _price, _count);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class VM : INotifyPropertyChanged
    {
        public VM()
        {
            PriceList = new ObservableCollection<Items>();
        }

        private int _income = 0;
        private Wallet _account = new Wallet();

        public void AddToAccount(Coins coins)
        {
            Account.AddCoins(coins);
        }

        public void Add(Coins coins)
        {
            Income += coins.CoinType*coins.Count;
            Account.AddCoins(coins);
        }

        public void GetChange()
        {
            Account.GetCoins(Income, Data.userWallet);
            Income = 0;
        }

        public ObservableCollection<Items> PriceList { get; set; }

        public int Income
        {
            get { return _income; }
            set
            {
                _income = value;
                OnPropertyChanged("Income");
            }
        }

        public Wallet Account
        {
            get { return _account; }
            set { _account = value; }
        }

        public void BuyItem(Items item)
        {
            try
            {
                if (item.Price <= Income)
                {
                    int index = PriceList.IndexOf(item);
                    PriceList[index] = new Items(item.Type, item.Price, item.Count - 1);
                    Income -= item.Price;
                    MessageBox.Show("Спасибо!");
                }
                else MessageBox.Show("Недостаточно средств");
            }
            catch { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class Data
    {
        public static VM vm = new VM();
        public static Wallet userWallet = new Wallet();
        //public static ObservableCollection<Items> PriceList = new ObservableCollection<Items>();
    }
}
