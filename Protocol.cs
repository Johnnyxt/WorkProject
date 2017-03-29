using System;

namespace JW8307A
{
    internal class Protocol
    {
        private enum Protcol_Parser
        {
            T_Check_SOP = 0,
            T_Check_Length,
            T_Check_Cmd,
            T_Check_Data,
            T_Check_CheckSum,
            T_Check_EOP,
            T_SUCCESS,
        };

        private const int MAX_SENDBUFF = 1024;
        private const Byte SOP = 0x7b;
        private const Byte EOP = 0x7d;

        private volatile Byte Snd_Len = 0;
        private volatile Byte Cmd = 0;
        private volatile Byte[] txdata = new Byte[MAX_SENDBUFF];
        private volatile Byte[] pdata = new Byte[MAX_SENDBUFF];

        private const Byte MAXSIZE = 255;
        private const Byte MINSIZE = 3;
        private volatile Byte DataIndex;
        private volatile Byte CheckSum;
        private volatile Byte P_DataLen = 0;
        private volatile Byte P_Cmd = 0;

        private Protcol_Parser pp = Protcol_Parser.T_Check_SOP;

        private Byte Calculation_Sum_Check(Byte[] data, int num)
        {
            Byte tmp = 0;

            for (int i = 0; i < num; i++)
            {
                tmp += data[i];
            }

            tmp = (Byte)(~tmp + 0x01);

            return tmp;
        }

        private Protcol_Parser Check_SOP(Byte input)
        {
            if (input == SOP)
            {
                CheckSum = input;
                P_DataLen = 0;
                DataIndex = 0;
                return Protcol_Parser.T_Check_Length;
            }

            return Protcol_Parser.T_Check_SOP;
        }

        private Protcol_Parser Check_Length(byte input)
        {
            if ((input >= MINSIZE))
            {
                P_DataLen = (Byte)(input - MINSIZE);
                return Protcol_Parser.T_Check_Cmd;
            }

            return Protcol_Parser.T_Check_SOP;
        }

        private Protcol_Parser Check_Cmd(Byte input)
        {
            P_Cmd = input;

            if (P_DataLen > 0)
            {
                return Protcol_Parser.T_Check_Data;
            }
            if (P_DataLen == 0)
            {
                return Protcol_Parser.T_Check_CheckSum;
            }

            return Protcol_Parser.T_Check_SOP;
        }

        private Protcol_Parser Check_Data(Byte input)
        {
            pdata[DataIndex++] = input;

            if (DataIndex < P_DataLen)
            {
                return Protcol_Parser.T_Check_Data;
            }

            return Protcol_Parser.T_Check_CheckSum;
        }

        private Protcol_Parser Check_CheckSum()
        {
            if (CheckSum == 0)
            {
                return Protcol_Parser.T_Check_EOP;
            }
            return Protcol_Parser.T_Check_EOP;
            //return (CheckSum == 0) ? Protcol_Parser.T_Check_EOP : Protcol_Parser.T_Check_SOP;
        }

        private Protcol_Parser Check_EOP(Byte input)
        {
            return input == EOP ? Protcol_Parser.T_SUCCESS : Protcol_Parser.T_Check_SOP;
        }

        public int Protcol_Parser_P(Byte input)
        {
            CheckSum += input;

            switch (pp)
            {
                case Protcol_Parser.T_Check_SOP:
                    pp = Check_SOP(input);
                    break;

                case Protcol_Parser.T_Check_Length:
                    pp = Check_Length(input);
                    break;

                case Protcol_Parser.T_Check_Cmd:
                    pp = Check_Cmd(input);
                    break;

                case Protcol_Parser.T_Check_Data:
                    pp = Check_Data(input);
                    break;

                case Protcol_Parser.T_Check_CheckSum:
                    pp = Check_CheckSum();
                    break;

                case Protcol_Parser.T_Check_EOP:
                    pp = Check_EOP(input);
                    if (pp == Protcol_Parser.T_SUCCESS)
                    {
                        pp = Protcol_Parser.T_Check_SOP;

                        return P_Cmd;
                    }
                    break;

                default:
                    pp = Protcol_Parser.T_Check_SOP;
                    break;
            }

            return 0x00;
        }

        public void Cmd_Action(int cmd)
        {
            txdata[0] = SOP;
            txdata[2] = Cmd;

            Snd_Len += 3;
            txdata[1] = Snd_Len;
            txdata[Snd_Len] = Calculation_Sum_Check(txdata, Snd_Len);
            Snd_Len++;
            txdata[Snd_Len++] = EOP;
        }

        public int Protocol_wr(Byte[] tx, int cmd)
        {
            Cmd = (Byte)cmd;
            Cmd_Action(Cmd);
            int len = Snd_Len;

            for (int i = 0; i < len; i++)
            {
                tx[i] = txdata[i];
            }
            Snd_Len = 0;

            return len;
        }

        public int Protocol_wr(Byte[] tx, int cmd, Byte[] data, int cnt)
        {
            Cmd = (Byte)cmd;
            Snd_Len = 0;

            for (int i = 0; i < cnt; i++)
            {
                txdata[3 + Snd_Len++] = data[i];
            }

            Cmd_Action(Cmd);
            int len = Snd_Len;

            for (int i = 0; i < len; i++)
            {
                tx[i] = txdata[i];
            }
            Snd_Len = 0;

            return len;
        }

        //public int Protocol_Convert(Byte[] dst)
        //{
        //    for (int i = 0; i < DataIndex; i++)
        //    {
        //        dst[i] = pdata[i];
        //    }

        //    return DataIndex;
        //}

        public byte[] Protocol_Convert()
        {
            byte[] data = new byte[DataIndex];

            for (int i = 0; i < DataIndex; i++)
            {
                data[i] = pdata[i];
            }

            return data;
        }
    }
}