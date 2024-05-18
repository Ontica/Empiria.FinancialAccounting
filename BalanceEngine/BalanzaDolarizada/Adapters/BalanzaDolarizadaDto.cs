/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : BalanzaDolarizadaDto                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a Balanza dolarizada.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

using Empiria.DynamicData;
using Empiria.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Output DTO used to return a Balanza dolarizada.</summary>
  public class BalanzaDolarizadaDto {


    [JsonProperty]
    public TrialBalanceQuery Query {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<BalanzaDolarizadaEntryDto> Entries {
      get; internal set;
    }


  } // class BalanzaDolarizadaDto


  public class BalanzaDolarizadaEntryDto : ITrialBalanceEntryDto {

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
      get; internal set;
    }

    public decimal ExchangeRate {
      get; internal set;
    }

    public decimal? ValuedExchangeRate {
      get;
      internal set;
    }

    public decimal TotalEquivalence {
      get; internal set;
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


    public string ToJson() {
      return JsonObject.Parse(this)
                       .ToString();
    }

  } // class BalanzaDolarizadaEntryDto


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
