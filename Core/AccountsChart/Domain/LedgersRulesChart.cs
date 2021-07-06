/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Type     : LedgersRulesChart                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds currencies rules for all accounts.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds ledgers rules for all accounts.</summary>
  static public class LedgersRulesChart {

    #region Fields

    static private FixedList<LedgerRule> _ledgersRules = AccountsChartData.GetAccountsLedgersRules();

    #endregion Fields

    #region Methods

    static internal FixedList<LedgerRule> GetAccountLedgerRules(Account account) {
      var list = _ledgersRules.FindAll(x => x.StandardAccountId == account.StandardAccountId);

      list.Sort((x, y) => x.Ledger.Number.CompareTo(y.Ledger.Number));

      return list;
    }

    #endregion Methods

  }  // LedgersRulesChart

}  // namespace Empiria.FinancialAccounting
