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
    public enum TypeMatrix : short
    {
        Galua,
        Fibonacci,
        GaluaT,
        FibonacciT,
        ReverseGalua,
        ReverseFibonacci,
        ReverseGaluaGaluaT,
        ReverseFibonacciT
    }
    public class RandomSequence
    {
        public long Vector { get; private set; }
        public long Polinom { get; }
        public int Power { get; }

        public List<ulong> Sequence = new List<ulong>();
        private List<Action> listActions;

        public RandomSequence(long polinom, long vector)
        {
            if (PowerPolinom(polinom) - PowerPolinom(vector) < 1)
                throw new ArgumentException(nameof(vector), "Vector сan't be greater than or equal to polinom.");
            Polinom = polinom;
            Vector = vector;
            Power = PowerPolinom(polinom);
            listActions = new List<Action>
            {
                Galua,
                Fibonacci,
                GaluaTransposed,
                FibonacciTransposed,
                ReverseGalua,
                ReverseFibonacci,
                ReverseGaluaTransposed,
                ReverseFibonacciTransposed
            };
        }
        public void Generate(TypeMatrix tm)
        {
            Sequence.Clear();
            listActions[(short)tm]?.Invoke();
        }
        public void Galua()
        {
            long polinom = Trim(Polinom);
            for (int i = 0; i < Math.Pow(2, Power) - 1; i++)
            {
                Vector <<= 1;
                long leftBit = GetBit(Vector, Power);
                long _vector = ZeroingBit(Vector, Power);
                Vector = ((leftBit * polinom << 1) ^ _vector) | leftBit;
                Sequence.Add((ulong)Vector);
            }
        }
        public void Fibonacci()
        {
            long polinom = Convert.ToInt64(new string(Convert.ToString(Polinom, 2).Reverse().ToArray()), 2) >> 1;
            for (int i = 0; i < Math.Pow(2, Power) - 1; i++)
            {
                long bit = BitHelp.BitInt(polinom & Vector) & 1;
                Vector <<= 1;
                Vector = ZeroingBit(Vector, Power) | bit;
                Sequence.Add((ulong)Vector);
            }
        }
        public void GaluaTransposed()
        {
            long polinom = Polinom;
            for (int i = 0; i < Math.Pow(2, Power) - 1; i++)
            {
                long bit = BitHelp.BitInt(polinom & Vector) & 1;
                Vector >>= 1;
                Vector = Vector | (bit << (Power - 1));
                Sequence.Add((ulong)Vector);
            }
        }
        public void FibonacciTransposed()
        {
            long polinom = Trim(Convert.ToInt64(new string(Convert.ToString(Polinom, 2).Reverse().ToArray()), 2));
            for (int i = 0; i < Math.Pow(2, Power) - 1; i++)
            {
                long _vector = Vector >> 1;
                long Bit = Vector & 1;
                Vector = ((Bit * polinom) ^ _vector) ^ (Bit << (Power - 1));
                Sequence.Add((ulong)Vector);
            }
        }
        public void ReverseGalua()
        {
            long polinom = Trim(Polinom);
            for (int i = 0; i < Math.Pow(2, Power) - 1; i++)
            {
                long _vector = Vector >> 1;
                long Bit = Vector & 1;
                Vector = ((Bit * polinom) ^ _vector) ^ (Bit << (Power - 1));
                Sequence.Add((ulong)Vector);
            }
        }
        public void ReverseFibonacci()
        {
            long polinom = Convert.ToInt64(new string(Convert.ToString(Polinom, 2).Reverse().ToArray()), 2);
            for (int i = 0; i < Math.Pow(2, Power) - 1; i++)
            {
                long bit = BitHelp.BitInt(polinom & Vector) & 1;
                Vector >>= 1;
                Vector = Vector | (bit << (Power - 1));
                Sequence.Add((ulong)Vector);
            }
        }
        public void ReverseGaluaTransposed()
        {

            long polinom = Convert.ToInt64(new string(Convert.ToString(Polinom, 2).ToArray()), 2) >> 1;
            for (int i = 0; i < Math.Pow(2, Power) - 1; i++)
            {
                long bit = BitHelp.BitInt(polinom & Vector) & 1;
                Vector <<= 1;
                Vector = ZeroingBit(Vector, Power) | bit;
                Sequence.Add((ulong)Vector);
            }
        }
        public void ReverseFibonacciTransposed()
        {
            long polinom = Trim(Convert.ToInt64(new string(Convert.ToString(Polinom, 2).Reverse().ToArray()), 2));
            for (int i = 0; i < Math.Pow(2, Power) - 1; i++)
            {
                Vector <<= 1;
                long leftBit = GetBit(Vector, Power);
                long _vector = ZeroingBit(Vector, Power);
                Vector = ((leftBit * polinom << 1) ^ _vector) | leftBit;
                Sequence.Add((ulong)Vector);
            }
        }
        private long TrimLeft(long value)
        {
            return SetBit(value, PowerPolinom(value));
        }
        private long Trim(long polinom)
        {
            polinom >>= 1;
            polinom = SetBit(polinom, PowerPolinom(polinom));
            return polinom;
        }
        private long SetBit(long value, int bit)
        {
            return value ^ (1L << bit);
        }
        private long GetBit(long value, int bit)
        {
            return value >> bit & 1;
        }
        private long ZeroingBit(long value, int bit)
        {
            return value & ~(1L << bit);
        }
        private int PowerPolinom(long polinom) => BitHelp.BitOll(polinom) - 1;


    }
    public static class BitHelp
    {
        public static int BitOll(long value)
        {
            int res = 0;
            while (value != 0)
            {
                res++;
                value >>= 1;
            }
            return res;
        }
        public static int BitInt(long value)
        {
            int res = 0;
            while (value != 0)
            {
                if ((value & 1) == 1)
                    res++;
                value >>= 1;
            }
            return res;
        }
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
        private List<ulong> _gamma1 = new List<ulong>();
        private Dictionary<int, long[,]> TestPolinom = new Dictionary<int, long[,]>()
        {
            //{ 12, new string[] { "111" } },
            //{ 12, new long[][] { { 7 }, { 4 } } }
            { 12, new long[,] { { 7 }, { 4 } }}
            //{ 12, new string[,] {{"string1.1", "string1.2"}, {"string2.1", "string2.2"}} },
            //{ 13, new string[,] {{"string1.1", "string1.2"}, {"string2.1", "string2.2"}} } 
        };
        public List<GammaItem> GammaTable
        {
            get
            {
                return _gamma.Select((x, i) => new GammaItem() { id = i, g1 = GetOutputString(x), g2 = GetOutputString(_gamma1[i]) }).ToList();
            }
            set
            {
                OnPropertyChanged("GammaTable");
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
                if (value < 2)
                    _factor = 2;
                else if (value > 16)
                    _factor = 16;
                else
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

        private BigInteger _polynomial = 285;
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

        private BigInteger _oe = 2;
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

        private int Matrix = 0;

        private List<ulong> TransposeMatrixR(List<ulong> matrix)
        {
            string[] tMatrix = new string[matrix.Count];
            for (int i = (matrix.Count - 1); i >= 0; i--)
            {
                string bits = Convert.ToString((long)matrix[i], 2).PadLeft(matrix.Count, '0');
                for (int n = 0; n < matrix.Count; n++)
                {
                    tMatrix[n] += bits[(matrix.Count - 1) - n];
                }
            }
            return tMatrix.ToList().Select(x => Convert.ToUInt64(x, 2)).ToList();
        }
        private List<ulong> TransposeMatrix(List<ulong> A)
        {
            string[] tMatrix = new string[A.Count];
            for (int i = 0; i < A.Count; i++)
            {
                string bits = Convert.ToString((long)A[i], 2).PadLeft(A.Count, '0');
                for (int n = 0; n < A.Count; n++)
                {
                    tMatrix[n] += bits[n];
                }
            }
            return tMatrix.ToList().Select(x => Convert.ToUInt64(x, 2)).ToList();
        }

        private RelayCommand _GenPolynomial;
        public RelayCommand GenPolynomial
        {
            get
            {
                return _GenPolynomial ?? (_GenPolynomial = new RelayCommand(command => {
                    Task.Run(() =>
                    {
                        long min = (long)Math.Pow(2, (int)Factor) + 1;
                        long max = min * 2;
                        int Bits = BitHelp.BitOll(min);
                        Bits = BitHelp.BitOll((long)(Math.Pow(2, BitHelp.BitOll(min) - 1))) -2;
                        TestPolinom.TryGetValue(Factor, out long[,] TestArray);
                        if(TestArray == null)
                            TestArray = new long[,] { };

                        while (true) 
                        {
                            long i = LongRandom(min, max);
                                if ((i & 1) == 0)
                                    i ^= 1;
                            if ((BitHelp.BitInt(i) & 1) == 1 )
                            {
                                int Res = 2;
                                long _Res = 2;
                                _Res = Div2(_Res, _Res << 1, i, Bits);
                                while (_Res > 1 && Res < Factor)
                                {
                                    Res += 1;
                                    _Res = Div2(_Res, _Res << 1, i, Bits);
                                }
                                if (_Res == 1 && Res == Factor)
                                {
                                    for(int a = Factor/2; a > 0; a--)
                                    {

                                    }
                                    int count = 1;
                                    BigInteger res = OE;
                                    do
                                    {
                                        res = ModPower(OE, count, i);
                                        count++;
                                    }
                                    while (res != 1);
                                    if (count == (int)Math.Pow(2, Factor))
                                    {
                                        PolynomialString = GetOutputString(i);
                                        break;
                                    }
                                }
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
                            int byteLength = (Factor < 8) ? 2 : ((Factor / 8) + 1);
                            byte[] numBytes = new byte[byteLength];
                            rng.GetBytes(numBytes);
                            numBytes[numBytes.Length - 1] = 0;
                            BigInteger vi = new BigInteger(numBytes) % new BigInteger(Math.Pow(2, Factor));
                            VIString = GetOutputString(vi < 1 ? 1 : vi);
                        }
                    });
                }, command => true));
            }
        }

        private RelayCommand _SetMatrix;
        public RelayCommand SetMatrix
        {
            get
            {
                return _SetMatrix ?? (_SetMatrix = new RelayCommand(command => {
                    Task.Run(() =>
                    {
                        switch ((string)command) {
                            case "G":
                                Matrix = 0;
                                break;
                            case "F":
                                Matrix = 1;
                                break;
                            case "G*":
                                Matrix = 2;
                                break;
                            case "F*":
                                Matrix = 3;
                                break;
                            case "Ḡ":
                                Matrix = 4;
                                break;
                            case "F̄":
                                Matrix = 5;
                                break;
                            case "Ḡ*":
                                Matrix = 6;
                                break;
                            case "F̄*":
                                Matrix = 7;
                                break;
                        }
                    });
                }, command => true));
            }
        }

        private RelayCommand _IncrFactor;
        public RelayCommand IncrFactor
        {
            get
            {
                return _IncrFactor ?? (_IncrFactor = new RelayCommand(command => {
                    Task.Run(() =>
                    {
                        Factor++;
                    });
                }, command => true));
            }
        }

        private RelayCommand _DecrFactor;
        public RelayCommand DecrFactor
        {
            get
            {
                return _DecrFactor ?? (_DecrFactor = new RelayCommand(command => {
                    Task.Run(() =>
                    {
                        Factor--;
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
                        if (PolynomialString == "")
                        {
                            MessageBox.Show("Не задано значение полинома!", "Что-то не так!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (OEString == "")
                        {
                            MessageBox.Show("Не задано значение ОЭ!", "Что-то не так!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (VIString == "")
                        {
                            MessageBox.Show("Не задано значение ВИ!", "Что-то не так!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        ulong[] matrix = new ulong[Factor];
                        if (
                            Matrix == 4 ||
                            Matrix == 5 ||
                            Matrix == 6 ||
                            Matrix == 7)
                        {
                            matrix[0] = (ulong)ModPower(OE, new BigInteger(Math.Pow(2, Factor) - 2), Polynomial);
                        }
                        else matrix[0] = (ulong)OE;
                        int max = (int)(Math.Pow(2, Factor) - 1);
                        for (int i = 1; i < Factor; i++)
                        {
                            BigInteger a = matrix[i - 1] << 1;
                            if (a > max)
                                a = a ^ Polynomial;
                            matrix[i] = (ulong)a;
                        }

                        if (Matrix == 1 || Matrix == 5)
                        {
                            matrix = TransposeMatrix(matrix.ToList()).ToArray();
                        }
                        else if (Matrix == 2 || Matrix == 6)
                        {
                            matrix = TransposeMatrixR(matrix.ToList()).ToArray();
                        }
                        else if (Matrix == 3 || Matrix == 7)
                        {
                            matrix = TransposeMatrixR(TransposeMatrix(matrix.ToList())).ToArray();
                        }
                        

                        ulong vector = (ulong)VI;
                        ulong g = 0;
                        _gamma.Clear();
                        _gamma.Add((ulong)VI);

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

                        RandomSequence randomSequence = new RandomSequence((long)Polynomial, (long)VI);
                        randomSequence.Generate((TypeMatrix)Matrix);

                        _gamma1.Clear();
                        _gamma1.Add((ulong)VI);
                        _gamma1.AddRange(randomSequence.Sequence);
                        
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
        Random rnd = new Random();
        public long LongRandom(long min, long max)
        {
            byte[] buf = new byte[8];
            rnd.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }
        private bool Criterion2(long P, long[,] testArray)
        {          
            for (int i = 0; i < testArray.GetLength(1); i++)
                if (!DivFalseBool(P, testArray[i,0], testArray[1,i]))
                    return false;
            return true;
        }
        private bool DivFalseBool(long a, long b, long c)
        {
            while (a > c)
            {
                a ^= b;
                if (a == 0)
                    return false;
                while ((a & 1) == 0)
                {
                    a >>= 1;
                }
            }
            return true;
        }
        private long Div2(long a, long b, long i, int Bits)
        {
            long p = 0;
            while (b > 0)
            {
                p ^= -(b & 1) & a;
                a = (a << 1) ^ (i & -((a >> Bits) & 1));
                b >>= 1;
            }
            return p;
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

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }
    }
}
