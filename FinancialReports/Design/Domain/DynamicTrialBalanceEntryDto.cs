/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : DynamicReportEntryTotals                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Generates a dynamic fields report entry to be used as a FinancialReportEntry.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.FinancialReports {

  public class DynamicTrialBalanceEntryDto : DynamicFields, ITrialBalanceEntryDto {

    public DynamicTrialBalanceEntryDto(ITrialBalanceEntryDto entry) {
      Assertion.Require(entry, nameof(entry));

      this.ItemType = entry.ItemType;
      this.AccountNumber = entry.AccountNumber;
      this.SectorCode = entry.SectorCode;
      this.SubledgerAccountNumber = entry.SubledgerAccountNumber;
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


  }  // class DynamicTrialBalanceEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
