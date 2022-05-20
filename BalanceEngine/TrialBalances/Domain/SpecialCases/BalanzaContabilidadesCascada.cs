/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaContabilidadesCascada               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza con contabilidades en cascada.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de balanza con contabilidades en cascada.</summary>
  internal class BalanzaContabilidadesCascada {

    private readonly TrialBalanceCommand _command;

    public BalanzaContabilidadesCascada(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var balanceHelper = new TrialBalanceHelper(_command);
      var helper = new BalanceCascadeAccountingHelper(_command);

      FixedList<TrialBalanceEntry> postingEntries = balanceHelper.GetPostingEntries();

      postingEntries = balanceHelper.GetSummaryToParentEntries(postingEntries);

      List<TrialBalanceEntry> balanceEntries = new List<TrialBalanceEntry>(postingEntries);

      List<TrialBalanceEntry> orderingEntries = helper.OrderingLedgersByAccount(balanceEntries);

      FixedList<TrialBalanceEntry> summaryByAccountEntries = 
                                    helper.GenerateTotalSummaryByGroup(orderingEntries);

      balanceEntries = helper.CombineGroupAndBalanceEntries(balanceEntries,
                                                          summaryByAccountEntries);

      List<TrialBalanceEntry> summaryDebtorCreditorEntries =
                                helper.GetSummaryByDebtorCreditor(orderingEntries);

      balanceEntries = helper.CombineTotalDebtorCreditorAndEntries(balanceEntries,
                                                                    summaryDebtorCreditorEntries);

      FixedList<TrialBalanceEntry> totalByCurrencies = helper.GenerateTotalSummaryByCurrency(
                                                            summaryDebtorCreditorEntries);

      balanceEntries = helper.CombineTotalByCurrencyAndBalanceEntries(balanceEntries, totalByCurrencies);

      balanceEntries = helper.GenerateTotalReport(balanceEntries.ToList());

      balanceEntries = helper.GetAverageBalance(balanceEntries);

      balanceHelper.RestrictLevels(balanceEntries);

      var returnBalance = new FixedList<ITrialBalanceEntry>(
                                balanceEntries.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }



    #region Helper methods

    

    #endregion Helper methods

  }  // class BalanzaContabilidadesCascada

}  // namespace Empiria.FinancialAccounting.BalanceEngine
