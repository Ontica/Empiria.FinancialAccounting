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

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanza valorizada comparativa.</summary>
  internal class BalanzaComparativa {

    private readonly TrialBalanceQuery _query;

    public BalanzaComparativa(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Build() {
      var balanceHelper = new TrialBalanceHelper(_query);
      var helper = new TrialBalanceComparativeHelper(_query);

      _query.FinalPeriod.IsSecondPeriod = true;

      FixedList<TrialBalanceEntry> entries = balanceHelper.GetPostingEntries();

      balanceHelper.SetSummaryToParentEntries(entries);

      entries = balanceHelper.ValuateToExchangeRate(entries, _query.FinalPeriod);

      balanceHelper.RoundDecimals(entries);

      helper.GetAverageBalance(entries.ToList());

      List<TrialBalanceComparativeEntry> comparativeBalance =
                                         helper.MergePeriodsIntoComparativeBalance(entries);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              comparativeBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }

  }  // class BalanzaComparativa

}  // namespace Empiria.FinancialAccounting.BalanceEngine
