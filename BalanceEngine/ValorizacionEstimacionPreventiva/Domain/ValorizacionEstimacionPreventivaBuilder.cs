/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : ValorizacionEstimacionPreventivaBuilder    License   : Please read LICENSE.txt file            *
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
  internal class ValorizacionEstimacionPreventivaBuilder {

    private readonly TrialBalanceQuery _query;

    public ValorizacionEstimacionPreventivaBuilder(TrialBalanceQuery query) {

      _query = query;

    }


    #region Public methods


    internal FixedList<ValorizacionEstimacionPreventivaEntry> Build() {

      DateTime initialDate = _query.InitialPeriod.FromDate;

      _query.InitialPeriod.FromDate = new DateTime(
                  _query.InitialPeriod.ToDate.Year,
                  _query.InitialPeriod.ToDate.Month, 1);

      FixedList <TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      _query.InitialPeriod.FromDate = initialDate;

      return Build(baseAccountEntries);
    }


    internal FixedList<ValorizacionEstimacionPreventivaEntry> Build(FixedList<TrialBalanceEntry> accountEntries) {

      if (accountEntries.Count == 0) {
        return new FixedList<ValorizacionEstimacionPreventivaEntry>();
      }

      var helper = new ValorizacionEstimacionPreventivaHelper(_query);

      FixedList<ValorizacionEstimacionPreventivaEntry> accountsByCurrency = helper.GetAccountsBalances(
                                          accountEntries, _query.InitialPeriod.ToDate);

      FixedList<ValorizacionEstimacionPreventivaEntry> accountsInfoByMonth = GetAccountsByFilteredMonths();
      
      FixedList <ValorizacionEstimacionPreventivaEntry> mergeAccountsByMonth = helper.MergeAccountsByMonth(
                                                          accountsByCurrency, accountsInfoByMonth);

      return mergeAccountsByMonth;
    }


    #endregion Public methods


    #region Private methods


    private FixedList<ValorizacionEstimacionPreventivaEntry> GetAccountsByFilteredMonths() {
      
      if (_query.InitialPeriod.ToDate.Month == 1) {

        return new FixedList<ValorizacionEstimacionPreventivaEntry>();

      } else {

        var helper = new ValorizacionEstimacionPreventivaHelper(_query);
        return helper.GetAccountsByFilteredMonth();

      }

    }


    #endregion Private methods

  } // class ValorizacionEstimacionPreventivaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
