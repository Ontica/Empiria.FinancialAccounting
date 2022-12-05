/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Providers                               *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Information holder                      *
*  Type     : DynamicTrialBalanceEntry                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information of a trial balance entry with their totals stored in dynamic fields.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

  /// <summary>Holds information of a trial balance entry with their totals stored in dynamic fields.</summary>
  public class DynamicTrialBalanceEntry : DynamicFields, ITrialBalanceEntryDto {

    public DynamicTrialBalanceEntry(ITrialBalanceEntryDto entry) {
      Assertion.Require(entry, nameof(entry));

      ItemType = entry.ItemType;
      AccountNumber = entry.AccountNumber;
      SectorCode = entry.SectorCode;
      SubledgerAccountNumber = entry.SubledgerAccountNumber;
    }

    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }


    public string SubledgerAccountNumber {
      get; internal set;
    }


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


  }  // class DynamicTrialBalanceEntry

} // namespace Empiria.FinancialAccounting.FinancialReports.Providers
