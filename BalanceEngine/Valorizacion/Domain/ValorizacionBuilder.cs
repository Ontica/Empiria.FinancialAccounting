/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : ValorizacionBuilder                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de valorizacion.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de valorizacion.</summary>
  internal class ValorizacionBuilder {

    private readonly TrialBalanceQuery _query;

    public ValorizacionBuilder(TrialBalanceQuery query) {

      _query = query;

    }


    #region Public methods


    internal FixedList<ValorizacionEntry> Build() {

      DateTime initialDate = _query.InitialPeriod.FromDate;
      _query.InitialPeriod.FromDate = new DateTime(
                  _query.InitialPeriod.ToDate.Year,
                  _query.InitialPeriod.ToDate.Month, 1);

      FixedList <TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      _query.InitialPeriod.FromDate = initialDate;

      return Build(baseAccountEntries);
    }


    internal FixedList<ValorizacionEntry> Build(FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new FixedList<ValorizacionEntry>();
      }

      var helper = new ValorizacionHelper(_query);

      FixedList<ValorizacionEntry> accountsByCurrency = helper.GetAccountsWithCurrencies(accountEntries,
                                                        _query.InitialPeriod.ToDate);

      FixedList<ValorizacionEntry> accountsInfoByMonth = helper.GetAccountsByFilteredMonth();

      FixedList<ValorizacionEntry> mergeAccountsByMonth = helper.MergeAccountsByMonth(
                                                          accountsByCurrency, accountsInfoByMonth);


      return mergeAccountsByMonth;
    }


    #endregion Public methods


  } // class ValorizacionBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
