using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Core;
using FreeUniverse.Core.Serialization;
using FreeUniverse.Arch;

namespace FreeUniverse.Common
{
    public enum CreditType
    {
        LibertyDollar = 0,
        RheinlandMark = 1,
        KusariYuan = 2,
        BretoniaPound = 3,
        UniverseCredit = 4,
        BlackMarketCoin = 5,
        RedRouble = 6,
        NomadJellies = 7,

        Invalid = 1000
    }

    public class CreditAccount
    {
        public static int MAX_CREDIT_TYPES = 8;
        private ulong[] credits { get; set; }

        [FastSerializable]
        public List<ulong> creditValues
        {
            get
            {
                List<ulong> lst = new List<ulong>();
                foreach (ulong val in credits)
                    lst.Add(val);
                return lst;
            }

            set
            {
                List<ulong> lst = value;
                int i = 0;
                foreach (ulong e in lst)
                {
                    credits[i] = e;
                    i++;
                }
            }
        }

        public void ReadHeader(INIReaderHeader header)
        {
            foreach (INIReaderParameter p in header.parameters)
            {
                if (p.Check("credit"))
                {
                    CreditType ct = StringToCreditType(p.GetString(0));

                    if (ct == CreditType.Invalid) continue;

                    credits[(int)ct] = p.GetUInt64(1);
                }
            }
        }

        public CreditAccount()
        {
            credits = new ulong[MAX_CREDIT_TYPES];
        }

        public void Add(CreditType type, ulong value)
        {
            if (value <= 0)
                return;

            credits[(int)type] += value;
        }

        public bool Draw(CreditType type, ulong value)
        {
            if (credits[(int)type] < value)
                return false;

            credits[(int)type] -= value;

            return true;
        }

        public ulong Get(CreditType type)
        {
            return credits[(int)type];
        }

        public static CreditType StringToCreditType(string str)
        {
            if (str.CompareTo("liberty_dollar") == 0) return CreditType.LibertyDollar;
            if (str.CompareTo("black_market_coin") == 0) return CreditType.BlackMarketCoin;
            if (str.CompareTo("universe_credit") == 0) return CreditType.UniverseCredit;
            if (str.CompareTo("bretonia_pound") == 0) return CreditType.BretoniaPound;
            if (str.CompareTo("kusari_yuan") == 0) return CreditType.KusariYuan;
            if (str.CompareTo("nomad_jellies") == 0) return CreditType.NomadJellies;
            if (str.CompareTo("red_rouble") == 0) return CreditType.RedRouble;
            if (str.CompareTo("rheinland_mark") == 0) return CreditType.RheinlandMark;
            return CreditType.Invalid;
        }
    }
}
