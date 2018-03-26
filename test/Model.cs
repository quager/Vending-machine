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
            if (!_coins.ContainsKey(coins.CoinType))
            {
                _coins[coins.CoinType] = coins.Count;
                List.Add(coins);
            }
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

        private List<Coins> GetComposition(int sum, Dictionary<int, int> coins)
        {
            int n = 0;
            int current_sum = 0;
            List<Coins> res = new List<Coins>();
            Dictionary<int, int> current_coins = new Dictionary<int, int>();
            List<int> coins_set = new List<int>();
            List<int> rates = coins.Keys.ToList();
            rates.Sort((a, b) => -1 * a.CompareTo(b));

            while (n < rates.Count)
            {
                int coin = rates[n];

                if (current_coins.ContainsKey(coin) && current_coins[coin] == _coins[coin])
                {
                    n++;
                    continue;
                }

                if (coin > sum || coins[coin] == 0)
                {
                    n++;
                    if (n == rates.Count)
                    {
                        if (coins_set.Count == 0) break;
                        n--;
                        int iprev = coins_set.Count - 1;
                        int prev = coins_set[iprev];
                        current_sum -= prev;
                        coins_set.RemoveAt(iprev);
                        current_coins[prev]--;

                        while (rates[n] != prev)
                        {
                            n--;
                            if (n < 0) return null;
                        }
                        n++;
                        continue;
                    }
                    continue;
                }

                current_sum += coin;
                coins_set.Add(coin);
                if (!current_coins.ContainsKey(coin)) current_coins[coin] = 1;
                else current_coins[coin]++;

                if (current_sum == sum)
                {
                    foreach (KeyValuePair<int, int> entry in current_coins)
                        res.Add(new Coins(entry.Key, entry.Value));

                    return res;
                }

                if (current_sum > sum)
                {
                    current_sum -= coin;
                    coins_set.RemoveAt(coins_set.Count - 1);
                    current_coins[coin]--;
                    
                    if (n == rates.Count - 1)
                    {
                        int prev = rates[n - 1];
                        current_sum -= prev;
                        int iprev = coins_set.Count - 1;

                        while (coins_set[iprev] != prev)
                        {
                            current_coins[coins_set[iprev]]--;
                            coins_set.RemoveAt(iprev);
                            iprev--;
                            if (iprev < 0) return null;
                        }

                        current_coins[prev]--;
                        coins_set.RemoveAt(iprev);
                        continue;
                    }
                    else n++;
                }
            }

            return null;
        }

        public void GetCoins(ref int sum, Wallet toDest)
        {
            if (sum == 0)
            {
                MessageBox.Show("Остатка нет");
                return;
            }
            if (sum > _account)
            {
                MessageBox.Show("Не хватает монет");
                return;
            }
            
            Dictionary<int, int> change = new Dictionary<int, int>();

            List<Coins> composition = GetComposition(sum, _coins);
            if (composition == null)
            {
                MessageBox.Show("Не хватает монет");
                return;
            }

            foreach (Coins coin in composition)
            {
                _coins[coin.CoinType] -= coin.Count;
                toDest.AddCoins(new Coins(coin.CoinType, coin.Count));
            }
            Account -= sum;
            sum = 0;

            UpdateList();
        }

        private void UpdateList()
        {
            for (int i = 0; i < List.Count; i++)
                List[i] = new Coins(List[i].CoinType, _coins[List[i].CoinType]);
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
            Account.GetCoins(ref _income, Data.userWallet);
            Income = _income;
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
