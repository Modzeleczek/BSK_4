using System.Text;

namespace Zadanie_4
{
    class SynchronousStreamCipher
    {
        public string Encrypt(string text, LinearFeedbackShiftRegister lfsr)
        {
            var bits = UnicodeStringToBitBools(text);
            for (int i = 0; i < bits.Length; ++i) // xorujemy każdy bit z wejścia z kolejnym bitem LFSRa
                bits[i] ^= lfsr.ShiftRight();
            // zamieniamy tabelę bitów na napis z zer i jedynek
            var charArray = new char[bits.Length];
            for (int i = 0; i < bits.Length; ++i)
                charArray[i] = (bits[i] == true) ? '1' : '0';
            return new string(charArray);
        }

        public string Decrypt(string text, LinearFeedbackShiftRegister lfsr)
        {
            var bits = LinearFeedbackShiftRegister.BitStringToBitBools(text);
            for (int i = 0; i < bits.Length; ++i)
                bits[i] ^= lfsr.ShiftRight();
            return BitBoolsToUnicodeString(bits);
        }

        private bool[] UnicodeStringToBitBools(string str) // przekształca tekst na tabelę bitów tworzących kolejne znaki
        {
            var bytes = Encoding.Unicode.GetBytes(str);
            var bitBools = new bool[str.Length * 16]; // każdy znak zajmuje 16 bitów (2 bajty)
            int bitI = 0;
            for (int byteI = 0; byteI < bytes.Length; ++byteI)
            {
                byte temp = bytes[byteI];
                for (int i = 7; i >= 0; --i)
                    bitBools[bitI++] = (temp & (1 << i)) != 0;
            }
            return bitBools;
        }

        private string BitBoolsToUnicodeString(bool[] bitBools)
        {
            var bytes = new byte[bitBools.Length / 8];
            int bitI = 0;
            for (int byteI = 0; byteI < bytes.Length; ++byteI)
            {
                byte temp = 0b0000_0000;
                for (int i = 7; i >= 0; --i)
                    if (bitBools[bitI++] == true)
                        temp |= (byte)(0b0000_0001 << i);
                bytes[byteI] = temp;
            }
            return Encoding.Unicode.GetString(bytes);
        }
    }
}
