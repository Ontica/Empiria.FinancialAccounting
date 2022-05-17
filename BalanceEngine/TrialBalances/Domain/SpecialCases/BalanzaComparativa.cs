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

    private readonly TrialBalanceCommand _command;

    public BalanzaComparativa(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var balanceHelper = new TrialBalanceHelper(_command);
      var helper = new TrialBalanceComparativeHelper(_command);

      _command.FinalPeriod.IsSecondPeriod = true;

      FixedList<TrialBalanceEntry> entries = balanceHelper.GetPostingEntries();

      entries = balanceHelper.GetSummaryToParentEntries(entries);

      entries = balanceHelper.ValuateToExchangeRate(entries, _command.FinalPeriod);

      entries = balanceHelper.RoundTrialBalanceEntries(entries);

      helper.GetAverageBalance(entries.ToList());

      List<TrialBalanceComparativeEntry> comparativeBalance = 
                                         helper.MergePeriodsIntoComparativeBalance(entries);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              comparativeBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }

  }  // class BalanzaComparativa

}  // namespace Empiria.FinancialAccounting.BalanceEngine
