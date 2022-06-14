/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaComparativa                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza valorizada comparativa.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanza valorizada comparativa.</summary>
  internal class BalanzaComparativaBuilder {

    private readonly TrialBalanceQuery _query;

    public BalanzaComparativaBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal FixedList<BalanzaComparativaEntry> Build() {

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      return Build(baseAccountEntries);
    }


    internal FixedList<BalanzaComparativaEntry> Build(FixedList<TrialBalanceEntry> baseAccountEntries) {

      _query.FinalPeriod.IsSecondPeriod = true;

      var helper = new BalanzaComparativaHelper(_query);

      FixedList<TrialBalanceEntry> entries = helper.AccountEntriesValorized(baseAccountEntries);

      var balanceHelper = new TrialBalanceHelper(_query);

      balanceHelper.SetSummaryToParentEntries(entries);

      entries = helper.GetExchangeRateByPeriod(entries, _query.FinalPeriod);

      balanceHelper.RoundDecimals(entries);

      helper.GetAverageBalance(entries.ToList());

      List<BalanzaComparativaEntry> comparativeBalance =
                                         helper.MergePeriodsIntoComparativeBalance(entries);

      return comparativeBalance.ToFixedList();

      //var returnBalance = new FixedList<ITrialBalanceEntry>(
      //                        comparativeBalance.Select(x => (ITrialBalanceEntry) x));

      //return new TrialBalance(_query, returnBalance);
    }

  }  // class BalanzaComparativa

}  // namespace Empiria.FinancialAccounting.BalanceEngine
