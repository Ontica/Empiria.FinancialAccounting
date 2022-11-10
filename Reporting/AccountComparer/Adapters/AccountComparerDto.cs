/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : AccountComparerDto                            License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return account comparer report data.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters {

  /// <summary>Output DTO used to return account comparer report data.</summary>
  public class AccountComparerDto {


    public ReportBuilderQuery Query {
      get; internal set;
    } = new ReportBuilderQuery();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<AccountComparerEntryDto> Entries {
      get; internal set;
    } = new FixedList<AccountComparerEntryDto>();


  } // class AccountComparerDto


  /// <summary>DTO for each account comparer entry report.</summary>
  public class AccountComparerEntryDto : IReportEntryDto {


    public TrialBalanceItemType ItemType {
      get;
      internal set;
    }


    public int AccountGroupId {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public string ActiveAccountName {
      get;
      internal set;
    }


    public string ActiveAccount {
      get; internal set;
    }


    public decimal ActiveBalance {
      get; internal set;
    }


    public string PasiveAccountName {
      get;
      internal set;
    }


    public string PasiveAccount {
      get; internal set;
    }


    public decimal PasiveBalance {
      get; internal set;
    }


    public decimal BalanceDifference {
      get; internal set;
    }
    
  } // class AccountComparerEntryDto


} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters
