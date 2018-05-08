using System;
namespace GameReward
{
    public enum TransactionType
    {
        Base = 1, Internal = 2, External = 3
    }

    public enum TransactionStatus
    {
        Pending = 0, Success = 1, Error = 2
    }
    public class GrdTransaction
    {
        public long transid;
        public long transdate;
        public String tx;
        public String from;
        public String to;
        public decimal amount;
        public TransactionType transtype;
        public TransactionStatus status;
        public DateTime getTime()
        {
            DateTime date = new DateTime(1970,1,1,0,0,0);
            date.AddSeconds(transdate);
            return date;
        }
    }
}
