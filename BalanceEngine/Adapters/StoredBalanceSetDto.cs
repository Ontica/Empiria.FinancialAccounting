/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : StoredBalanceSetDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO for a set of stored account balances or account aggrupation balances.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>DTO for a set of stored account balances or account aggrupation balances.</summary>
  public class StoredBalanceSetDto {

    internal StoredBalanceSetDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public NamedEntityDto AccountsChart {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public DateTime BalancesDate {
      get; internal set;
    }


    public  bool Calculated {
      get; internal set;
    }


    public DateTime CalculationTime {
      get; internal set;
    }


    public FixedList<StoredBalanceDto> Balances {
      get; internal set;
    }

  }  // class StoredBalanceSetDto

}  // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
