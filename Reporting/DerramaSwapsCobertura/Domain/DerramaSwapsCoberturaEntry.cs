/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Empiria Plain Object                    *
*  Type     : DerramaSwapsCoberturaEntry                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for 'Derrama de intereses de swaps de cobertura'.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura {

  /// <summary>Represents an entry for 'Derrama de intereses de swaps de cobertura'.</summary>
  public class DerramaSwapsCoberturaEntry {

    public string SubledgerAccount {
      get; internal set;
    }

    public string SubledgerAccountName {
      get; internal set;
    }

    public string Classification {
      get; internal set;
    }

    public decimal IncomeAccountTotal {
      get; internal set;
    }

    public decimal ExpensesAccountTotal {
      get; internal set;
    }

    public decimal Total {
      get {
        return IncomeAccountTotal + ExpensesAccountTotal;
      }
    }

    internal void Sum(DerramaSwapsCoberturaEntry entry) {
      IncomeAccountTotal += entry.IncomeAccountTotal;
      ExpensesAccountTotal += entry.ExpensesAccountTotal;
    }

  }  // class DerramaSwapsCoberturaEntry

}  // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura
