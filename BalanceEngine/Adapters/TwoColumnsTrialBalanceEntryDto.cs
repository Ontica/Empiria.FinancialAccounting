/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : TrialBalanceDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of a trial balance with separated domestic               *
*             and foreign currencies totals                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return the entries of a trial balance with separated domestic
  /// and foreign currencies totals.</summary>
  public class TwoColumnsTrialBalanceEntryDto : ITrialBalanceEntryDto {

    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public string LedgerUID {
      get; internal set;
    }


    public string LedgerNumber {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public int StandardAccountId {
      get; internal set;
    }


    public int SubledgerAccountId {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string StandardAccountNumber {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


    public string AccountName {
      get; internal set;
    }


    public AccountRole AccountRole {
      get; internal set;
    }


    public int AccountLevel {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }


    public decimal InitialBalance {
      get;
      internal set;
    }

    public decimal Debit {
      get;
      internal set;
    }
    public decimal Credit {
      get;
      internal set;
    }

    public decimal DomesticBalance {
      get; internal set;
    }


    public decimal ForeignBalance {
      get; internal set;
    }


    public decimal TotalBalance {
      get; internal set;
    }


    public decimal ExchangeRate {
      get; internal set;
    }


    public decimal AverageBalance {
      get; internal set;
    }


    public DateTime LastChangeDate {
      get; set;
    }
    
  }  // class TwoColumnsTrialBalanceEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
