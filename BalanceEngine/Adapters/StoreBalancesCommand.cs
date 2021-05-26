/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : StoreBalancesCommand                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to store balances or aggrupation balances.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Command payload used to store balances or aggrupation balances.</summary>
  public class StoreBalancesCommand {

    public string AccountsChartUID {
      get; set;
    }


    public DateTime BalancesDate {
      get; set;
    }

  } // class StoreBalancesCommand


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
