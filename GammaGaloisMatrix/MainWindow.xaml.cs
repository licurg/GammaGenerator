﻿using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ComponentModel;
using System.Numerics;
using System.Security.Cryptography;

namespace GammaGaloisMatrix
{
    public class GammaItem
    {
        public int id { get; set; }
        public string g1 { get; set; }
        public string g2 { get; set; }
    }
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

    public class MainVM : INotifyPropertyChanged
    {
        private List<BigInteger> D = new List<BigInteger>()
        {
            3,5,15,17,51,85,255,257,771,1285,3855,4369,13107,21845,65535,65537,196611,327685,983055,1114129,3342387,5570645,16711935,16843009,50529027,84215045,252645135,286331153,858993459,1431655765,4294967295,4294967297,12884901891,21474836485,64424509455,73014444049,219043332147,365072220245,1095216660735,1103806595329,3311419785987,5519032976645,16557098929935,18764712120593,28470681808895,56294136361779,93823560602965,281479271743489,844437815230467,1407396358717445,4222189076152335,4785147619639313,14355442858917939,23925738098196565,71777214294589695,217020518514230019,361700864190383365,723401728380766673,1085102592571150095,1229782938247303441,3689348814741910323,6148914691236517205
        };

        private List<ulong> _gamma = new List<ulong>();

        public List<GammaItem> GammaTable
        {
            get
            {
                return _gamma.Select((x, i) => new GammaItem() { id = i, g1 = GetOutputString(x), g2 = GetOutputString(1) }).ToList();
            }
            set
            {
                OnPropertyChanged("GammaTable");
            }
        }

        public List<short> FactorList
        {
            get
            {
                return new List<short>()
                {
                    8, 16, 32, 64
                };
            }
        }
        private short _factor = 8;
        public short Factor
        {
            get
            {
                return _factor;
            }
            set
            {
                _factor = value;
                OnPropertyChanged("Factor");
            }
        }

        public List<string> CSList
        {
            get
            {
                return new List<string>()
                {
                    "BIN",
                    "OCT",
                    "DEC",
                    "HEX"
                };
            }
        }

        private string _cs = "BIN";
        public string CS
        {
            get
            {
                return _cs;
            }
            set
            {
                _cs = value;
                OnPropertyChanged("CS");
                PolynomialString = GetOutputString(Polynomial);
                OEString = GetOutputString(OE);
                VIString = GetOutputString(VI);
                GammaTable = null;
            }
        }

        public string PolynomialString
        {
            get
            {
                return GetOutputString(Polynomial);
            }
            set
            {
                Polynomial = GetIntValue(value);
                OnPropertyChanged("PolynomialString");
            }
        }

        private BigInteger _polynomial = 499;
        public BigInteger Polynomial
        {
            get {
                return _polynomial;
            }
            set
            {
                _polynomial = value;
                OnPropertyChanged("Polynomial");
            }
        }

        public string OEString
        {
            get
            {
                return GetOutputString(OE);
            }
            set
            {
                OE = GetIntValue(value);
                OnPropertyChanged("OEString");
            }
        }

        private BigInteger _oe = 255;
        public BigInteger OE
        {
            get
            {
                return _oe;
            }
            set
            {
                _oe = value;
                OnPropertyChanged("OE");
            }
        }

        public string VIString
        {
            get
            {
                return GetOutputString(VI);
            }
            set
            {
                VI = GetIntValue(value);
                OnPropertyChanged("VIString");
            }
        }

        private BigInteger _vi = 1;
        public BigInteger VI
        {
            get
            {
                return _vi;
            }
            set
            {
                _vi = value;
                OnPropertyChanged("VI");
            }
        }

        private RelayCommand _GenPolynomial;
        public RelayCommand GenPolynomial
        {
            get
            {
                return _GenPolynomial ?? (_GenPolynomial = new RelayCommand(command => {
                    Task.Run(() =>
                    {
                        CheckPolynomial:
                        int weight = new Random().Next(3, Factor + 1);
                        string polynomialString = "1" + new String(new String('1', weight - 2).PadLeft(Factor - 1, '0').ToCharArray().OrderBy(x => Guid.NewGuid()).ToArray()) + "1";
                        var condition = weight > 0 ? polynomialString.Count(x => x == '1') == weight : polynomialString.Count(x => x == '1') % 2 != 0;
                        if (!condition) goto CheckPolynomial;
                        else
                        {
                            BigInteger f = BinToDec(polynomialString);
                            BigInteger a = 0b10;
                            BigInteger b = 0b100;
                            int i = 2;

                            var res = ModMult(a, b, f);
                            while (res != 1 && i <= Factor)
                            {
                                i += 1;
                                a = res;
                                b = res << 1;
                                res = ModMult(a, b, f);
                            }

                            if (i == Factor && res == 1)
                            {
                                PolynomialString = GetOutputString(f);
                            }
                            else goto CheckPolynomial;
                        }
                    });
                }, command => true));
            }
        }

        private RelayCommand _GenOE;
        public RelayCommand GenOE
        {
            get
            {
                return _GenOE ?? (_GenOE = new RelayCommand(command => {
                    Task.Run(() =>
                    {
                        BigInteger max = 6148914691236517205;
                        if (Factor == 8)
                        {
                            max = 85;
                        }
                        if (Factor == 16)
                        {
                            max = 21845;
                        }
                        if (Factor == 32)
                        {
                            max = 1431655765;
                        }

                        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                        {
                            byte[] numBytes = new byte[(Factor / 8) + 1];
                        CheckOE:
                            rng.GetBytes(numBytes);
                            numBytes[numBytes.Length - 1] = 0;

                            BigInteger w = new BigInteger(numBytes);

                            var dividers = D.TakeWhile(x => ModPower(w, x, Polynomial) != 1 && x <= max).ToList();
                            if (dividers.Count == D.IndexOf(max) && !dividers.Contains(max))
                            {
                                OEString = GetOutputString(w);
                            }
                            else
                            {
                                goto CheckOE;
                            }
                        }
                    });
                }, command => true));
            }
        }

        private RelayCommand _GenVI;
        public RelayCommand GenVI
        {
            get
            {
                return _GenVI ?? (_GenVI = new RelayCommand(command => {
                    Task.Run(() =>
                    {
                        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                        {
                            byte[] numBytes = new byte[(Factor / 8) + 1];
                            rng.GetBytes(numBytes);
                            numBytes[numBytes.Length - 1] = 0;
                            VIString = GetOutputString(new BigInteger(numBytes));
                        }
                    });
                }, command => true));
            }
        }

        private RelayCommand _Start;
        public RelayCommand Start
        {
            get
            {
                return _Start ?? (_Start = new RelayCommand(command => {
                    Task.Run(() =>
                    {
                        
                        ulong[] matrix = new ulong[Factor];
                        matrix[0] = (ulong)OE;
                        ulong max = 0xFFFFFFFFFFFFFFFF;
                        if (Factor == 32)
                        {
                            max = 0xFFFFFFFF;
                        }
                        if (Factor == 16)
                        {
                            max = 0xFFFF;
                        }
                        if (Factor == 8)
                        {
                            max = 0xFF;
                        }
                        for (int i = 1; i < Factor; i++)
                        {
                            BigInteger a = matrix[i - 1] << 1;
                            if (a > max)
                                a = a ^ Polynomial;
                            matrix[i] = (ulong)a;
                        }

                        ulong vector = (ulong)VI;
                        ulong g = 0;
                        _gamma.Clear();

                        while (g != VI)
                        {
                            g = 0;
                            
                            for (int i = 0; i < Factor; i++)
                            {
                                g ^= matrix[i] & matrix[i] * (vector & 0x1);
                                vector >>= 1;
                            }
                            vector = g;
                            _gamma.Add(vector);
                        }
                        GammaTable = null;
                    });
                }, command => true));
            }
        }

        private RelayCommand _Reset;
        public RelayCommand Reset
        {
            get
            {
                return _Reset ?? (_Reset = new RelayCommand(command => {

                }, command => true));
            }
        }

        private BigInteger GetIntValue(string value)
        {
            switch (CS)
            {
                case "BIN":
                    return BinToDec(value);
                case "OCT":
                    return value.Aggregate(new BigInteger(), (b, c) => b * 8 + c - '0');
                case "DEC":
                    return BigInteger.Parse(value);
                case "HEX":
                    return BigInteger.Parse(("0" + value), System.Globalization.NumberStyles.HexNumber);
            }
            return 0;
        }

        private string GetOutputString(BigInteger integer)
        {
            switch (CS)
            {
                case "BIN":
                    return ToBinaryString(integer);
                case "OCT":
                    return ToOctalString(integer);
                case "DEC":
                    return ToDecimalString(integer);
                case "HEX":
                    return ToHexadecimalString(integer);
            }
            return "0";
        }

        private BigInteger BinToDec(string bitString)
        {
            BigInteger integer = 0;
            foreach (char bit in bitString)
            {
                integer <<= 1;
                integer += bit == '1' ? 1 : 0;
            }
            return integer;
        }

        private string ToBinaryString(BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var byteI = bytes.Length - 1;
            var bitString = new StringBuilder(bytes.Length * 8);
            var binary = Convert.ToString(bytes[byteI], 2);
            bitString.Append(binary);
            for (byteI--; byteI >= 0; byteI--)
            {
                bitString.Append(Convert.ToString(bytes[byteI], 2).PadLeft(8, '0'));
            }
            return bitString.ToString().TrimStart('0');
        }

        private string ToOctalString(BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var byteI = bytes.Length - 1;
            var octString = new StringBuilder(((bytes.Length / 3) + 1) * 8);
            var extra = bytes.Length % 3;
            if (extra == 0)
            {
                extra = 3;
            }
            int int24 = 0;
            for (; extra != 0; extra--)
            {
                int24 <<= 8;
                int24 += bytes[byteI--];
            }
            var octal = Convert.ToString(int24, 8);
            octString.Append(octal);
            for (; byteI >= 0; byteI -= 3)
            {
                int24 = (bytes[byteI] << 16) + (bytes[byteI - 1] << 8) + bytes[byteI - 2];
                octString.Append(Convert.ToString(int24, 8).PadLeft(8, '0'));
            }
            return octString.ToString();
        }

        private string ToDecimalString(BigInteger bigint)
        {
            return bigint.ToString();
        }

        private string ToHexadecimalString(BigInteger bigint)
        {
            return bigint.ToString("X").TrimStart('0');
        }

        private BigInteger ModPower(BigInteger a, BigInteger k, BigInteger mod)
        {
            BigInteger res = 1;
            while (k > 0)
            {
                if ((k & 1) != 0)
                    res = ModMult(res, a, mod);
                a = ModMult(a, a, mod);
                k >>= 1;
            }
            return res;
        }
        private BigInteger ModMult(BigInteger a, BigInteger b, BigInteger mod)
        {
            BigInteger res = 0;
            while (a > 0 && b > 0)
            {
                if ((b & 1) != 0)
                    res ^= a;
                if (a >= BigInteger.Pow(2, Factor - 1))
                    a = (a << 1) ^ mod;
                else
                    a <<= 1;
                b >>= 1;
            }
            return res;
        }

        public MainVM()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainVM();
        }
    }
}
