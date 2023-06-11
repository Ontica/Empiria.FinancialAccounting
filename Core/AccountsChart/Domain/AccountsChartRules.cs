/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Type     : AccountsChartRules                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information for a chart of accounts rules related data.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information for a chart of accounts rules related data.</summary>
  public class AccountsChartRules {

    #region Fields

    private readonly FixedList<AreaRule> _areasRules;
    private readonly EmpiriaHashTable<FixedList<CurrencyRule>> _currenciesRules;
    private readonly FixedList<LedgerRule> _ledgersRules;
    private readonly FixedList<SectorRule> _sectorRules;

    #endregion Fields


    internal AccountsChartRules(AccountsChart accountsChart) {
      Assertion.Require(accountsChart, nameof(accountsChart));

      _areasRules = AccountsChartData.GetAccountsAreasRules(accountsChart);

      _currenciesRules = AccountsChartData.GetAccountsCurrenciesRules(accountsChart);

      _ledgersRules = AccountsChartData.GetAccountsLedgersRules(accountsChart);

      _sectorRules = AccountsChartData.GetAccountsSectorsRules(accountsChart);
    }


    public FixedList<AreaRule> GetAccountAreasRules(Account account) {
      var list = _areasRules.FindAll(x => x.StandardAccountId == account.StandardAccountId);

      list.Sort((x, y) => x.StandardAccountId.CompareTo(y.StandardAccountId));

      return list;
    }


    public FixedList<CurrencyRule> GetAccountCurrenciesRules(Account account) {
      FixedList<CurrencyRule> list;

      _currenciesRules.TryGetValue(account.StandardAccountId.ToString(), out list);

      if (list != null) {
        return list;
      } else {
        return new FixedList<CurrencyRule>();
      }
    }


    public FixedList<LedgerRule> GetAccountLedgerRules(Account account) {
      var list = _ledgersRules.FindAll(x => x.StandardAccountId == account.StandardAccountId);

      list.Sort((x, y) => x.Ledger.Number.CompareTo(y.Ledger.Number));

      return list;
    }


    public FixedList<SectorRule> GetAccountSectorRules(Account account) {
      var list = _sectorRules.FindAll(x => x.StandardAccountId == account.StandardAccountId);

      list.Sort((x, y) => x.Sector.Code.CompareTo(y.Sector.Code));

      return list;
    }

  }  // class AccountsChartRules

}  // namespace Empiria.FinancialAccounting
