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

        private bool Fit(int start, int sum, List<int> rates, ref List<Coins> values, Dictionary<int, int> coins)
        {
            int n = start + 1;
            List<Coins> baseList = new List<Coins>(values);

            for (int i = n; i < rates.Count; i++)
            {
                int coin = rates[i];

                if (coins[coin] == 0) continue;
                if (coin > sum)
                {
                    Coins prev = baseList[baseList.Count - 1];
                    if ((sum + prev.CoinType) % coin == 0 && coins[coin] >= (sum + prev.CoinType))
                    {
                        baseList[baseList.Count - 1].Count--;
                        sum += prev.CoinType;
                    }
                    else continue;
                }

                if (sum > coins[coin] * coin)
                {
                    int nv = sum - coin * coins[coin];
                    baseList = new List<Coins>(baseList);
                    baseList.Add(new Coins(coin, coins[coin]));

                    if (Fit(i, nv, rates, ref baseList, coins))
                    {
                        values = baseList;
                        return true;
                    }
                    baseList.RemoveAt(baseList.Count - 1);
                    continue;
                }

                baseList.Add(new Coins(coin, sum / coin));
                if (sum % coin == 0)
                {
                    values = baseList;
                    return true;
                }
                
                int v = sum - coin * (sum / coin);
                baseList = new List<Coins>(baseList);

                if (v == 0)
                {
                    baseList.RemoveAt(baseList.Count - 1);
                    continue;
                }

                if (Fit(i, v, rates, ref baseList, coins))
                {
                    values = baseList;
                    return true;
                }
                baseList.RemoveAt(baseList.Count - 1);
            }

            return false;
        }

        private List<Coins> GetComposition(int sum, Dictionary<int, int> coins)
        {
            List<Coins> val;
            List<int> rates = coins.Keys.ToList();
            rates.Sort((a, b) => -1 * a.CompareTo(b));

            for (int n = 0; n < rates.Count; n++)
            {
                int coin = rates[n];

                if (coin > sum || coins[coin] == 0) continue;
                if (sum > coins[coin] * coin)
                {
                    int nv = sum - coin * coins[coin];
                    val = new List<Coins>();
                    val.Add(new Coins(coin, coins[coin]));

                    if (Fit(n, nv, rates, ref val, coins)) return val;
                    continue;
                }

                val = new List<Coins>();
                val.Add(new Coins(coin, sum / coin));

                if (sum % coin == 0) return val;

                int v = sum - coin * (sum / coin);
                if (Fit(n, v, rates, ref val, coins)) return val;
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
            
            int amount = sum;
            Dictionary<int, int> change = new Dictionary<int, int>();

            List<Coins> composition = GetComposition(sum, _coins);
            if (composition == null)
            {
                MessageBox.Show("Не хватает монет");
                return;
            }

            foreach (Coins coin in composition)
            {
                amount -= coin.CoinType * coin.Count;
                if (!change.ContainsKey(coin.CoinType)) change[coin.CoinType] = coin.Count;
                else change[coin.CoinType] += coin.Count;
            }

            foreach (KeyValuePair<int, int> entry in change)
            {
                _coins[entry.Key] -= entry.Value;
                toDest.AddCoins(new Coins(entry.Key, entry.Value));
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
