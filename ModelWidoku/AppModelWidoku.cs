using System;

namespace DekoderPESEL.ModelWidoku
{
    using Model;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;

    public class AppModelWidoku : INotifyPropertyChanged
    {
        private AppModel model = new AppModel();

        #region Properties
        public string Pesel
        {
            get
            {
                return model.Pesel;
            }
            set
            {
                model.Pesel = value;
                OnPropertyChanged(nameof(Pesel));
            }
        }

        public string DataUrodzenia
        {
            get
            {
                return model.DataUrodzenia;
            }
            set
            {
                model.DataUrodzenia = value;
                OnPropertyChanged(nameof(DataUrodzenia));
            }
        }

        public string Płeć
        {
            get
            {
                return model.Płeć;
            }
            set
            {
                model.Płeć = value;
                OnPropertyChanged(nameof(Płeć));
            }
        }

        public string NrSeryjny
        {
            get
            {
                return model.NumerSeryjny;
            }
            set
            {
                model.NumerSeryjny = value;
                OnPropertyChanged(nameof(NrSeryjny));
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        #region RelayCommand
        private ICommand _dekoduj;
        private ICommand _resetuj;

        public ICommand Dekoduj
        {
            get
            {
                if (_dekoduj == null)
                    _dekoduj = new RelayCommand(
                        (object o) =>
                        {                            
                            DataUrodzenia = DekodujDatęUr(Pesel);
                            Płeć = JakaPłeć(Pesel);
                            NrSeryjny = Pesel[6].ToString() + Pesel[7].ToString() + Pesel[8].ToString() + Pesel[9].ToString() + Pesel[10].ToString();
                        },
                        (object o) =>
                        {
                            return CzyPeselJestPoprawny(Pesel);
                        }
                        );
                return _dekoduj;
            }
        }

        public ICommand Resetuj
        {
            get
            {
                if (_resetuj == null)
                    _resetuj = new RelayCommand(
                        (object o) =>
                        {
                            Pesel = "";
                            DataUrodzenia = "";
                            Płeć = "";
                            NrSeryjny = "";
                        },
                        (object o) =>
                        {
                            return !string.IsNullOrEmpty(Pesel) || !string.IsNullOrEmpty(DataUrodzenia) || !string.IsNullOrEmpty(Płeć) || !string.IsNullOrEmpty(NrSeryjny);                            
                        }
                        );
                return _resetuj;
            }
        }

        #endregion

        #region metody pomocnicze
        // sprawdzenie poprawności nr PESEL na podstawie cyfry kontrolnej
        private bool CzyPeselJestPoprawny(string pesel)
        {
            int[] p = new int[11];
            int sumaKontrolna;
            int cyfraKontrolna;

            if (string.IsNullOrEmpty(pesel))
                return false;
            else
            {
                foreach (var item in pesel)
                {
                    if (!Char.IsDigit(item))
                        return false;
                }

                for (int i = 0; i < pesel.Length; i++)
                {
                    p[i] = int.Parse(pesel[i].ToString());
                }
            }                        

            cyfraKontrolna = p[10];
            sumaKontrolna = 9 * p[0] + 7 * p[1] + 3 * p[2] + 1 * p[3] + 9 * p[4] + 7 * p[5] + 3 * p[6] + 1 * p[7] + 9 * p[8] + 7 * p[9];

            if (sumaKontrolna % 10 == cyfraKontrolna)
                return true;
            else
                return false;
        }

        //wyznaczenie daty ur na podstawie PESEL
        public string DekodujDatęUr(string pesel)
        {
            int[] dataPesel = new int[6];
            int stulecie, rok, nrMiesiąca, dzień;
            string miesiąc;
            List<string> listaMiesięcy = new List<string>(
            new string[] { "styczeń", "luty", "marzec", "kwiecień", "maj", "czerwiec", "lipiec", "sierpień", "wrzesień", "październik", "listopad", "grudzień" });

            if (!CzyPeselJestPoprawny(pesel))
                return "";
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    dataPesel[i] = int.Parse(pesel[i].ToString());
                }
            }

            if (dataPesel[2] == 0 || dataPesel[2] == 1)
                stulecie = 1900;
            else if (dataPesel[2] == 2 || dataPesel[2] == 3)
                stulecie = 2000;
            else if (dataPesel[2] == 4 || dataPesel[2] == 5)
                stulecie = 2100;
            else if (dataPesel[2] == 6 || dataPesel[2] == 7)
                stulecie = 2200;
            else
                stulecie = 1800;

            rok = stulecie + dataPesel[0] * 10 + dataPesel[1];

            if ((dataPesel[2] == 1 || dataPesel[2] == 3 || dataPesel[2] == 5 || dataPesel[2] == 7 || dataPesel[2] == 9) && dataPesel[3] == 1)
                nrMiesiąca = 11;
            else if ((dataPesel[2] == 1 || dataPesel[2] == 3 || dataPesel[2] == 5 || dataPesel[2] == 7 || dataPesel[2] == 9) && dataPesel[3] == 2)
                nrMiesiąca = 12;
            else if (dataPesel[3] == 0)
                nrMiesiąca = 10;
            else nrMiesiąca = dataPesel[3];

            miesiąc = listaMiesięcy[nrMiesiąca - 1];

            dzień = dataPesel[4] * 10 + dataPesel[5];

            return dzień + " " + miesiąc + " " + rok + " r.";
        }

        //ustalenie płci na podstawie PESEL
        public string JakaPłeć(string pesel)
        {
            if (pesel[9].Equals('0') || pesel[9].Equals('2') || pesel[9].Equals('4') || pesel[9].Equals('6') || pesel[9].Equals('8'))
                return "kobieta";
            return "mężczyzna";
        }
        #endregion
    }
}
