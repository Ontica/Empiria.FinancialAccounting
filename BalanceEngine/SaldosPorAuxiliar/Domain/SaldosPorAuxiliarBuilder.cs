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
  internal class SaldosPorAuxiliarBuilder {

    private readonly TrialBalanceQuery _query;

    internal SaldosPorAuxiliarBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Build() {
      var trialBalanceHelper = new TrialBalanceHelper(_query);
      var helper = new SaldosPorAuxiliarHelper(_query);

      List<TrialBalanceEntry> accountEntries = trialBalanceHelper.GetPostingEntries().ToList();

      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      EmpiriaHashTable<TrialBalanceEntry> summaryEntries = helper.BalancesBySubledgerAccounts(accountEntries);

      List<TrialBalanceEntry> orderedTrialBalance = helper.OrderByAccountNumber(summaryEntries);

      accountEntries = helper.CombineTotalAndSummaryEntries(orderedTrialBalance, accountEntries);

      accountEntries = helper.GenerateAverageBalance(accountEntries);

      trialBalanceHelper.RestrictLevels(accountEntries);

      var returnBalance = new FixedList<ITrialBalanceEntry>(accountEntries.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


    internal TrialBalance BuildForBalancesGeneration() {
      var helper = new TrialBalanceHelper(_query);

      _query.WithSubledgerAccount = true;

      FixedList<TrialBalanceEntry> trialBalance = helper.GetPostingEntries();

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


  }  // class SaldosPorAuxiliar

}  // namespace Empiria.FinancialAccounting.BalanceEngine
