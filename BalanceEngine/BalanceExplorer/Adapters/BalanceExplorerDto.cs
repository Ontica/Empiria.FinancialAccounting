/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : BalanceExplorerDto                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return balances for the balances explorer.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters {

    /// <summary>Output DTO used to return balances for the balances explorer.</summary>
  public class BalanceExplorerDto {

    [JsonProperty]
    public BalanceExplorerQuery Query {
      get; internal set;
    } = new BalanceExplorerQuery();


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    [JsonProperty]
    public FixedList<BalanceExplorerEntryDto> Entries {
      get; internal set;
    } = new FixedList<BalanceExplorerEntryDto>();

  } // class BalanceExplorerDto


  /// <summary>Output DTO used to return balance explorer entries.</summary>
  public class BalanceExplorerEntryDto {

    public TrialBalanceItemType ItemType {
      get; internal set;
    } = TrialBalanceItemType.Entry;


    public string LedgerUID {
      get;
      internal set;
    }

    public string LedgerNumber {
      get; internal set;
    } = string.Empty;


    public string LedgerName {
      get; internal set;
    } = string.Empty;


    public string CurrencyCode {
      get; internal set;
    } = string.Empty;


    public string CurrencyName {
      get; internal set;
    } = string.Empty;


    public string AccountNumber {
      get; internal set;
    } = string.Empty;


    public string AccountNumberForBalances {
      get; internal set;
    }

    public string AccountName {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


    public string subledgerAccountName {
      get; internal set;
    } = string.Empty;


    public string SectorCode {
      get; internal set;
    } = string.Empty;


    public decimal InitialBalance {
      get; internal set;
    }


    public decimal? CurrentBalance {
      get; internal set;
    }


    public decimal CurrentBalanceForBalances {
      get;
      internal set;
    }


    public DateTime LastChangeDate {
      get; internal set;
    } = DateTime.Now;


    public DateTime LastChangeDateForBalances {
      get;
      internal set;
    } = DateTime.Now;


    public string DebtorCreditor {
      get; internal set;
    } = string.Empty;


    public bool HasAccountStatement {
      get; internal set;
    }


    public bool ClickableEntry {
      get; internal set;
    }

  } // class BalanceExplorerEntryDto

} // Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters
