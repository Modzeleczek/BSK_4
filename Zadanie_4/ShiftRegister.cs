namespace Zadanie_4
{
    public class ShiftRegister
    {
        public int BitLength { get; protected set; }
        public int ByteLength { get; protected set; }
        protected byte[] State;

        public ShiftRegister(int bitLength)
        {
            BitLength = bitLength;
            ByteLength = bitLength / 8 + (bitLength % 8 != 0 ? 1 : 0); // ceil(bitLength / 8)
            State = new byte[ByteLength];
        }

        protected ShiftRegister() { }

        // index - indeks bitu przy numeracji od prawej strony, począwszy od 0
        public void SetBit(int index)
        {
            // optymalizacje: index >> 3; index & 7
            int byteI = index / 8, bitInByteI = index % 8;
            State[byteI] |= (byte)(1 << bitInByteI);
        }

        public void ClearBit(int index)
        {
            int byteI = index / 8, bitInByteI = index % 8;
            State[byteI] &= (byte)(~(1 << bitInByteI));
        }

        public bool ReadBit(int index)
        {
            int byteI = index / 8, bitInByteI = index % 8;
            return (State[byteI] & (1 << bitInByteI)) != 0;
        }

        // zakładamy, że newState ma tyle samo B co State
        public void Set(byte[] newState)
        {
            // newState może zawierać np. 24 b (3 B), a State tylko BitLength (np. 21) b (3 B); kopiujemy pełne bajty, ale przy zmianie stanu używamy tylko BitLength b
            for (int i = 0; i < ByteLength; ++i)
                State[i] = newState[i];
        }

        // przeciążenie z tabelą booli
        public void Set(bool[] newState)
        {
            int limit = newState.Length;
            if (limit > BitLength) // jeżeli newState jest dłuższy niż LFSR, to obcinamy newState
                limit = BitLength;
            for (int i = 0; i < limit; ++i)
            {
                if (newState[i] == true)
                    SetBit(i);
                else
                    ClearBit(i);
            }
            for (int i = newState.Length; i < BitLength; ++i) // jeżeli newState jest krótszy niż LFSR, to dopełniamy LFSR zerami
                ClearBit(i);
        }

        // zakładamy, że state ma tyle samo B co State
        public void Read(byte[] state)
        {
            for (int i = 0; i < ByteLength; ++i)
                state[i] = State[i];
        }

        // zwraca bool reprezentujący 1 bit
        public virtual bool ShiftRight()
        {
            // np. jeżeli mamy rejestr o długości 21 bitów, to rezerwujemy 3 bajty, ale 3 najstarszych b trzeciego B nie używamy
            // bajt
            // |State[2]        |State[1]         |State[0]
            // Q
            // |        2   4   | 6   8   10  12  | 14  16  18  20
            // |      1   3   5 |   7   9   11  13|   15  17  19  21
            // bit
            // |        19  17    15  13  11  9     7   5   3   1
            // |      20  18  16    14  12  10  8     6   4   2   0
            // |0 0 0|0 1 0 1 1 | 1 0 0 0 1 1 1 0 | 0 0 0 1 0 1 1 0
            //  ^ ^ ^ nieużywane bity
            //  ^ MSb     LSb ^ | ^ MSb     LSb ^ | ^ MSb     LSb ^
            // 0. bit 0. bajtu jest pierwszym bitem z prawej
            bool ret = ReadBit(0);
            for (int i = 0; i < ByteLength - 1; ++i)
            {
                byte temp = (byte)((State[i + 1] & 0b0000_0001) << 7); // pobieramy LSb i + 1 bajtu; przesuwamy najmłodszy bit powstałego tymczasowego bajtu (potencjalną 1) o 7 miejsc w lewo, aby dołączyć ją do bajtu i
                State[i] = (byte)((State[i] >> 1) | temp); // przesuwamy bajt i o 1 miejsce w prawo; po przesunięciu MSb bajtu 1 jest 0; jeżeli z bajtu i + 1 odczytaliśmy 1, to na MSb bajtu 1 ustawiamy 1; jeżeli z bajtu i + 1 odczytaliśmy 0, to na MSb bajtu i pozostaje 0 (0 | 0 = 0)
            }
            State[ByteLength - 1] >>= 1; // przesuwamy ostatni bajt o 1 miejsce w prawo
            return ret;
        }

        public override string ToString()
        {
            var chars = new char[BitLength];
            int charI = BitLength - 1;
            // więcej obliczeń, bo dzielenie i modulo co iterację
            /*for (int i = 0; i < BitLength; ++i)
            {
                int byteI = i / 8, bitInByteI = i % 8;
                chars[charI--] = (State[byteI] & (1 << bitInByteI)) != 0 ? '1' : '0';
            }*/
            for (int B = 0; B < ByteLength - 1; ++B) // do przedostatniego bajtu
                for (int b = 0; b < 8; ++b)
                    chars[charI--] = (State[B] & (1 << b)) != 0 ? '1' : '0';
            int remainder = BitLength % 8;
            for (int b = 0; b < remainder; ++b) // ostatni bajt
                chars[charI--] = (State[ByteLength - 1] & (1 << b)) != 0 ? '1' : '0';
            return new string(chars);
        }
    }
}
