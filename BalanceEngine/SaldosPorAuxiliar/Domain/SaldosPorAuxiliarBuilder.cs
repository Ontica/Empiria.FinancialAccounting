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
      var saldosHelper = new SaldosPorAuxiliarHelper(_query);

      FixedList<TrialBalanceEntry> accountEntries = trialBalanceHelper.GetAccountEntries();

      if (accountEntries.Count == 0) {
        return new TrialBalance(_query, new FixedList<ITrialBalanceEntry>());
      }

      trialBalanceHelper.SetSummaryToParentEntries(accountEntries);

      EmpiriaHashTable<TrialBalanceEntry> parentAccounts =
                                          saldosHelper.GetBalancesBySubledgerAccounts(accountEntries);

      List<TrialBalanceEntry> orderedParentAccounts =
                              saldosHelper.OrderingAndAssingnSubledgerAccountInfoToParent(parentAccounts);

      List<TrialBalanceEntry> returnedAccountEntries = saldosHelper.GetTotalsAndCombineWithAccountEntries(
                                                       orderedParentAccounts, accountEntries);

      saldosHelper.GenerateAverageBalance(returnedAccountEntries);

      trialBalanceHelper.RestrictLevels(returnedAccountEntries);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                              returnedAccountEntries.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


    internal TrialBalance BuildForBalancesGeneration() {
      var helper = new TrialBalanceHelper(_query);

      _query.WithSubledgerAccount = true;

      FixedList<TrialBalanceEntry> trialBalance = helper.GetAccountEntries();

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_query, returnBalance);
    }


  }  // class SaldosPorAuxiliar

}  // namespace Empiria.FinancialAccounting.BalanceEngine
