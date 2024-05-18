/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : BalanzaComparativaDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a Balanza comparativa.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

using Empiria.DynamicData;
using Empiria.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Output DTO used to return a Balanza comparativa.</summary>
  public class BalanzaComparativaDto {


    [JsonProperty]
    public TrialBalanceQuery Query {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<BalanzaComparativaEntryDto> Entries {
      get; internal set;
    }


  } // class BalanzaComparativaDto


  /// <summary>Output DTO used to return comparative trial balances. </summary>
  public class BalanzaComparativaEntryDto : ITrialBalanceEntryDto {
    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    public string LedgerUID {
      get; internal set;
    }

    public string LedgerNumber {
      get; internal set;
    }

    public string LedgerName {
      get;
      internal set;
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


    public string AccountParent {
      get;
      internal set;
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

    public DebtorCreditorType DebtorCreditor {
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

    public string SubledgerAccountNumber {
      get;
      internal set;
    }
    public string SubledgerAccountName {
      get;
      internal set;
    }

    public decimal FirstTotalBalance {
      get;
      internal set;
    }

    public decimal FirstExchangeRate {
      get;
      internal set;
    } = 1;

    public decimal FirstValorization {
      get;
      internal set;
    }

    public decimal Debit {
      get;
      internal set;
    } = 0;

    public decimal Credit {
      get;
      internal set;
    } = 0;

    public decimal SecondTotalBalance {
      get;
      internal set;
    }

    public decimal SecondExchangeRate {
      get;
      internal set;
    } = 1;

    public decimal SecondValorization {
      get;
      internal set;
    }

    public decimal Variation {
      get;
      internal set;
    }

    public decimal VariationByER {
      get;
      internal set;
    }

    public decimal RealVariation {
      get;
      internal set;
    }

    public string GroupName {
      get; internal set;
    }

    public string GroupNumber {
      get; internal set;
    }

    public decimal AverageBalance {
      get; internal set;
    }

    public DateTime LastChangeDate {
      get; internal set;
    }


    public bool HasAccountStatement {
      get; internal set;
    } = false;


    public bool ClickableEntry {
      get; internal set;
    } = false;


    public string ToJson() {
      return JsonObject.Parse(this)
                       .ToString();
    }

  } // class TrialBalanceComparativeDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
