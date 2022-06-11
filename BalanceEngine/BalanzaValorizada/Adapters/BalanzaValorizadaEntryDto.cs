/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : ValuedTrialBalanceDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return valued trial balances.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return valued trial balances.</summary>
  public class BalanzaValorizadaEntryDto : ITrialBalanceEntryDto {

    public string CurrencyCode {
      get; internal set;
    }

    public string CurrencyName {
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

    public decimal? TotalBalance {
      get;  internal set;
    }

    public decimal ExchangeRate {
      get; internal set;
    }

    public decimal? ValuedExchangeRate {
      get;
      internal set;
    }

    public decimal TotalEquivalence {
      get;  internal set;
    }

    public string GroupName {
      get; internal set;
    }

    public string GroupNumber {
      get; internal set;
    }

    public TrialBalanceItemType ItemType {
      get;
      internal set;
    }

    public bool HasAccountStatement {
      get; internal set;
    } = false;


    public bool ClickableEntry {
      get; internal set;
    } = false;


    string ITrialBalanceEntryDto.SubledgerAccountNumber {
      get {
        return String.Empty;
      }
    }

  } // class ValuedTrialBalanceDto



  public class TrialBalanceByCurrencyDto : ITrialBalanceEntryDto {

    public string CurrencyCode {
      get; internal set;
    }

    public string CurrencyName {
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

    public decimal DomesticBalance {
      get;  internal set;
    }

    public decimal DollarBalance {
      get;
      internal set;
    }

    public decimal YenBalance {
      get; internal set;
    }

    public decimal EuroBalance {
      get;  internal set;
    }

    public decimal UdisBalance {
      get; internal set;
    }

    public string GroupName {
      get; internal set;
    }

    public string GroupNumber {
      get; internal set;
    }

    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public bool HasAccountStatement {
      get; internal set;
    } = false;


    public bool ClickableEntry {
      get; internal set;
    } = false;


    string ITrialBalanceEntryDto.SubledgerAccountNumber {
      get {
        return String.Empty;
      }
    }

  }  // class TrialBalanceByCurrencyDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
