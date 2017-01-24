using System.Collections.Generic;

namespace ReactIb.DataTypes
{
    public class ManagedAccountsData
    {
        public IEnumerable<string> Accounts { get; }

        public ManagedAccountsData(IEnumerable<string> accounts)
        {
            Accounts = accounts;
        }
    }
}