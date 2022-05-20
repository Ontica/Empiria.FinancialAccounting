/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorAuxiliar                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por auxiliar.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Helpers;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por auxiliar.</summary>
  internal class SaldosPorAuxiliar {

    private readonly TrialBalanceCommand _command;

    public SaldosPorAuxiliar(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var balanceHelper = new TrialBalanceHelper(_command);
      var helper = new BalanceBySubledgerAccountHelper(_command);

      List<TrialBalanceEntry> trialBalance = balanceHelper.GetPostingEntries().ToList();

      balanceHelper.SetSummaryToParentEntries(trialBalance);

      EmpiriaHashTable<TrialBalanceEntry> summaryEntries = helper.BalancesBySubledgerAccounts(trialBalance);

      List<TrialBalanceEntry> orderedTrialBalance = helper.OrderByAccountNumber(summaryEntries);

      trialBalance = helper.CombineTotalAndSummaryEntries(orderedTrialBalance, trialBalance);

      trialBalance = helper.GenerateAverageBalance(trialBalance);

      balanceHelper.RestrictLevels(trialBalance);

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    internal TrialBalance BuildForBalancesGeneration() {
      var helper = new TrialBalanceHelper(_command);

      _command.WithSubledgerAccount = true;

      FixedList<TrialBalanceEntry> trialBalance = helper.GetPostingEntries();

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


  }  // class SaldosPorAuxiliar

}  // namespace Empiria.FinancialAccounting.BalanceEngine
