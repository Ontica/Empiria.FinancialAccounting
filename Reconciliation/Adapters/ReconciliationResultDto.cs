/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Data Transfer Object                    *
*  Type     : ReconciliationResultDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with a reconciliation result.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>Output DTO with a reconciliation result.</summary>
  public class ReconciliationResultDto {

    internal ReconciliationResultDto() {
      // no-op
    }

    public ReconciliationCommand Command {
      get; internal set;
    }


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    public FixedList<ReconciliationResultEntry> Entries {
      get; internal set;
    }

  }  // class ReconciliationResultDto


  /// <summary>DTO that describes a reconciliation result entry.</summary>
  public class ReconciliationResultEntryDto {

    internal ReconciliationResultEntryDto() {
      // no-op
    }

    public string AccountNumber {
      get; internal set;
    }


    public string SubledgerAccountNumber {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }


    public decimal OperationalTotal {
      get; internal set;
    }


    public decimal AccountingTotal {
      get; internal set;
    }


    public decimal Difference {
      get {
        return this.OperationalTotal - AccountingTotal;
      }
    }

  }  // class ReconciliationResultEntryDto

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
