using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Case_tournament_WPF {

    public partial class MainWindow : Window, INotifyPropertyChanged  {

        private ModelNPV model;
        public ModelNPV Model {
            get { return model; }
            set {
                model = value;
                OnPropertyChanged("Model");
            }
        }

        public MainWindow() {
            InitializeComponent();
            model = new ModelNPV();
            DataContext = this;
            calculateBtn.Click += onCalculateBtnClicked;
        }

        public void onCalculateBtnClicked(object sender, RoutedEventArgs e) {
            Model.CalculateNPV();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ModelNPV : INotifyPropertyChanged {
        private int year = 2050;
        private float discountStake = 0.2f;
        private double npv;
        private const int startYear = 2020;
        private const int totalYears = 30;
        private readonly List<int> incomes;
        private readonly List<int> outcomes;
        private readonly List<int> netCashFlow;

        public int Year {
            get { return year; }
            set {
                year = value > 2050 ? 2050 : value < startYear ? startYear : value;
                OnPropertyChanged("Year");
            }
        }

        public float DiscountStake {
            get { return discountStake; }
            set {
                discountStake = value;
                OnPropertyChanged("DiscountStake");
            }
        }

        public double NPV {
            get { return npv; }
            set {
                npv = Math.Round(value, 2);
                OnPropertyChanged("NPV");
            }
        }

        public ModelNPV() {
            incomes = Enumerable.Repeat(1000, totalYears).ToList();
            outcomes = Enumerable.Repeat(0, totalYears).ToList();
            outcomes[2] = outcomes[3] = 500;
            netCashFlow = new List<int>();
            for (int i = 0; i < totalYears; ++i) {
                netCashFlow.Add(incomes[i] - outcomes[i]);
            }
        }
        
        public void CalculateNPV() {
            double npv = netCashFlow[0] * (1 / (1 + discountStake));

            for (int i = 1; i < year - startYear - 1; ++i) {
                npv += netCashFlow[i] * (1 / Math.Pow(1 + discountStake, i + 1));
            }
            NPV = npv;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
