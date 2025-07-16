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

      if (baseAccountEntries.Count == 0) {
        return new FixedList<BalanzaComparativaEntry>();
      }

      _query.FinalPeriod.IsSecondPeriod = true;

      var helper = new BalanzaComparativaHelper(_query);

      FixedList<TrialBalanceEntry> accountEntries = helper.AccountEntriesValorized(baseAccountEntries);

      var balanceHelper = new TrialBalanceHelper(_query);

      balanceHelper.SetSummaryToParentEntriesV2(accountEntries);

      helper.ValuateEntriesToExchangeRate(accountEntries, _query.FinalPeriod);

      balanceHelper.RoundDecimals(accountEntries);

      helper.GetAverageBalance(accountEntries);

      FixedList<BalanzaComparativaEntry> comparativeBalance =
                                    helper.MergePeriodsIntoComparativeBalance(accountEntries);

      return comparativeBalance;
    }

  }  // class BalanzaComparativa

}  // namespace Empiria.FinancialAccounting.BalanceEngine
