/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : BalanceStorageCommand                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to interact with the balance storage.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Command payload used to interact with the balance storage.</summary>
  public class BalanceStorageCommand {

    public string AccountsChartUID {
      get; set;
    }

    public DateTime BalancesDate {
      get; set;
    }

  } // class BalanceStorageCommand

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
