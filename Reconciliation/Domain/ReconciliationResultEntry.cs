/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Information Holder                      *
*  Type     : ReconciliationResultEntry                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a result entry of a reconciliation process.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Holds a result entry of a reconciliation process.</summary>
  public class ReconciliationResultEntry {

    internal string UniqueKey {
      get; set;
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

  }  // class ReconciliationResultEntry

}  //namespace Empiria.FinancialAccounting.Reconciliation
