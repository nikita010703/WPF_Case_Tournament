using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Office.Interop.Excel;

namespace Case_tournament_WPF {

    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged  {

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
        private readonly List<double> incomes;
        private readonly List<double> outcomes;
        private readonly List<double> netCashFlow;

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
                discountStake = !(value < 0) ? value : 0;
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
            incomes = new List<double>();
            outcomes = new List<double>();
            netCashFlow = new List<double>();
        }
        
        public void CalculateNPV() {
            string file = Directory.GetCurrentDirectory() + "\\Source.xlsx";
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook wb = excel.Workbooks.Open(file);
            Worksheet ws = wb.Worksheets[2];

            Range income = ws.Range["B2:B32"];
            foreach (double cell in income.Value) {
                incomes.Add((int)cell);
            }

            Range outcome = ws.Range["C2:C32"];
            foreach (double cell in outcome.Value) {
                //int tmp;
                outcomes.Add((int)cell);
            }

            for (int i = 0; i < totalYears; ++i) {
                netCashFlow.Add(incomes[i] - outcomes[i]);
            }

            double npv = netCashFlow[0] * (1 / (1 + discountStake));
            for (int i = 1; i < year - startYear - 1; ++i) {
                npv += netCashFlow[i] * (1 / Math.Pow(1 + discountStake, i + 1));
            }
            NPV = npv;

            wb.Close();
            excel.Quit();

            Marshal.ReleaseComObject(outcome);
            Marshal.ReleaseComObject(income);
            Marshal.ReleaseComObject(ws);
            Marshal.ReleaseComObject(wb);
            Marshal.ReleaseComObject(excel);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
