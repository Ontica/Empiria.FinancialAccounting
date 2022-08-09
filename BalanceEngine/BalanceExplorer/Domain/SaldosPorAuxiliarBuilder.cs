/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : SaldosPorAuxiliarBuilder                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por auxiliar para el explorador de saldos.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer {

  /// <summary>Genera los datos para el reporte de saldos por auxiliar para el explorador de saldos.</summary>
  internal class SaldosPorAuxiliarBuilder {

    private readonly BalanceExplorerQuery _query;

    public SaldosPorAuxiliarBuilder(BalanceExplorerQuery query) {
      _query = query;
    }


    internal BalanceExplorerResult Build() {
      var helper = new BalanceExplorerHelper(_query);

      FixedList<BalanceExplorerEntry> balanceEntries = helper.GetBalanceExplorerEntries();

      if (balanceEntries.Count == 0) {
        return new BalanceExplorerResult(_query, new FixedList<BalanceExplorerEntry>());
      }

      balanceEntries = helper.GetSummaryToParentEntries(balanceEntries);

      FixedList<BalanceExplorerEntry> subledgerAccounts = helper.GetSubledgerAccounts(balanceEntries);

      List<BalanceExplorerEntry> orderingBalance = helper.OrderBySubledgerAccounts(subledgerAccounts);

      FixedList<BalanceExplorerEntry> returnedEntries = helper.CombineSubledgerAccountsWithBalanceEntries(
                                                            orderingBalance, balanceEntries);

      var balancesToReturn = new FixedList<BalanceExplorerEntry>(returnedEntries);

      return new BalanceExplorerResult(_query, balancesToReturn);
    }


  } // class SaldosPorAuxiliarBuilder

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer
