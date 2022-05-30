/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll      Pattern   : Data Transfer Object                    *
*  Type     : VouchersByAccountDto                            License   : Please read LICENSE.txt file       *
*                                                                                                            *
*  Summary  : Output DTO used to return vouchers by account.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;

namespace Empiria.FinancialAccounting.Reporting.Adapters {


  public interface IVouchersByAccountEntryDto {

  }

  /// <summary>Output DTO used to return vouchers by account.</summary>
  public class AccountStatementDto {

    public BalancesQuery Command {
      get; internal set;
    } = new BalancesQuery();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<IVouchersByAccountEntryDto> Entries {
      get; internal set;
    } = new FixedList<IVouchersByAccountEntryDto>();


    public string Title {
      get; internal set;
    }

  } // class VouchersByAccountDto


  public class VouchersByAccountEntryDto : IVouchersByAccountEntryDto {

    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public int VoucherId {
      get;
      internal set;
    }


    public string LedgerUID {
      get;
      internal set;
    }


    public string LedgerNumber {
      get; internal set;
    }


    public string LedgerName {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public int StandardAccountId {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string AccountNumberForBalances {
      get; internal set;
    }


    public string AccountName {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }


    public decimal? Debit {
      get; internal set;
    }


    public decimal? Credit {
      get; internal set;
    }


    public decimal CurrentBalance {
      get;
      internal set;
    }


    public string VoucherNumber {
      get;
      internal set;
    }


    public string ElaboratedBy {
      get;
      internal set;
    }


    public string Concept {
      get;
      internal set;
    }


    public string SubledgerAccountNumber {
      get;
      internal set;
    }


    public DateTime AccountingDate {
      get; internal set;
    }


    public DateTime RecordingDate {
      get;
      internal set;
    }
    
  } // class VouchersByAccountEntryDto

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
