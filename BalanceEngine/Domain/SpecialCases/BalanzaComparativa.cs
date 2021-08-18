/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaComparativa                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza valorizada comparativa.                               *
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
      var helper = new TrialBalanceHelper(_command);
      var comparativeHelper = new TrialBalanceComparativeHelper(_command);

      List<TrialBalanceComparativeEntry> trialBalanceComparative = new List<TrialBalanceComparativeEntry>();

      FixedList<TrialBalanceEntry> trialBalanceFirstPeriod = helper.GetPostingEntries();
      trialBalanceFirstPeriod = helper.GenerateAverageDailyBalance(trialBalanceFirstPeriod.ToList(),
                                                              _command.InitialPeriod).ToFixedList();

      FixedList<TrialBalanceEntry> trialBalanceSecondPeriod = helper.GetPostingEntries(true);
      trialBalanceSecondPeriod = helper.GenerateAverageDailyBalance(trialBalanceSecondPeriod.ToList(),
                                                               _command.FinalPeriod).ToFixedList();

      trialBalanceComparative = comparativeHelper.MergePeriodsIntoComparativeBalance(
                                                    trialBalanceFirstPeriod, trialBalanceSecondPeriod);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalanceComparative.Select(x => (ITrialBalanceEntry) x));
      return new TrialBalance(_command, returnBalance);
    }
  }
}
