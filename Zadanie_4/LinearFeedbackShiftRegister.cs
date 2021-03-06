using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zadanie_4
{
    public class LinearFeedbackShiftRegister : ShiftRegister
    {
        private int[] Exponents;

        // exponents - wykładniki potęg wielomianu; powinny być podawane w numeracji od lewej, począwszy od 1
        public LinearFeedbackShiftRegister(int[] exponents) : base()
        {
            ConstructFromExponentArray(exponents);
        }

        private void ConstructFromExponentArray(int[] exponents)
        {
            Exponents = RemoveInvalidAndDuplicates(exponents);
            if (Exponents.Length == 0)
                throw new ArgumentException("No valid (positive) exponents were specified.");
            BitLength = FindMax(Exponents);
            // tabela wykładników zawiera indeksy Q_i bitów (przerzutników) numerowane od lewej strony, począwszy od 1; przeliczamy indeks na numerowany od prawej strony, począwszy od 0
            for (int i = 0; i < Exponents.Length; ++i)
                Exponents[i] = BitLength - Exponents[i];
            ByteLength = BitLength / 8 + (BitLength % 8 != 0 ? 1 : 0); // ceil(bitLength / 8)
            State = new byte[ByteLength];
        }

        private int[] RemoveInvalidAndDuplicates(int[] array)
        {
            // usuwamy potencjalne duplikaty wykładników
            var set = new HashSet<int>();
            foreach (var e in array)
                if (e > 0) // usuwamy potencjalne 0 i ujemne wykładniki
                    set.Add(e);
            var filtered = new int[set.Count];
            int i = 0;
            foreach (var e in set)
                filtered[i++] = e;
            return filtered;
        }

        private int FindMax(int[] array)
        {
            int max = Exponents[0];
            for (int i = 1; i < Exponents.Length; ++i)
                if (Exponents[i] > max)
                    max = Exponents[i];
            return max;
        }

        public override bool ShiftRight()
        {
            bool newLastBit = false; // nowy ostatni bit obliczony poprzez xor bitów, których numery są w tabeli wykładników
            foreach (var e in Exponents)
                newLastBit ^= ReadBit(e);
            var ret = base.ShiftRight(); // przesuwamy rejestr w prawo
            // aktualizujemy ostatni bit
            if (newLastBit == false) ClearBit(BitLength - 1);
            else SetBit(BitLength - 1);
            return ret;
        }

        public static bool[] BitStringToBitBools(string str)
        {
            var list = new LinkedList<bool>();
            foreach (var c in str)
            {
                if (c == '0')
                    list.AddLast(false);
                else if (c == '1')
                    list.AddLast(true);
            }
            return list.ToArray();
        }

        // konstruktor parsujący wielomian i seed w postaci tekstowej
        public LinearFeedbackShiftRegister(string polynomialStr, string seedStr)
        {
            ConstructFromExponentArray(ParsePolynomialString(polynomialStr));
            var seedBools = BitStringToBitBools(seedStr);
            Reverse(seedBools); // LFSR wpisuje seed od prawej do lewej, więc odwracamy nasz seed zapisany od lewej do prawej
            Set(seedBools); // jeżeli seed będzie za długi, to zostanie obcięty z lewej; jeżeli będzie za krótki, to LFSR zostanie dopełniony zerami
        }

        private int[] ParsePolynomialString(string str)
        {
            var builder = new StringBuilder();
            foreach (var c in str)
                if (c == ';' || c == '-' || (c >= '0' && c <= '9'))
                    builder.Append(c);
            var split = builder.ToString().Split(';');
            var list = new LinkedList<int>();
            foreach (var s in split)
                if (int.TryParse(s, out int number))
                    list.AddLast(number);
            return list.ToArray();
        }

        private void Reverse<T>(T[] array)
        {
            for (int i = 0; i < array.Length / 2; ++i)
            {
                T temp = array[array.Length - 1 - i];
                array[array.Length - 1 - i] = array[i];
                array[i] = temp;
            }
        }
    }
}
