/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Query payload                           *
*  Type     : SaldosEncerradosQuery                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Query payload used to build locked up balances.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Query payload used to build locked up balances.</summary>
  public class SaldosEncerradosQuery {

    public string AccountsChartUID {
      get; set;
    }


    public DateTime FromDate {
      get; set;
    }


    public DateTime ToDate {
      get; set;
    }

  } // class SaldosEncerradosQuery

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
