/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : BalanzaColumnasMonedaDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a Balanza en columnas por moneda.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Newtonsoft.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Output DTO used to return a Balanza en columnas por moneda.</summary>
  public class BalanzaColumnasMonedaDto {


    [JsonProperty]
    public TrialBalanceQuery Query {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<BalanzaColumnasMonedaEntryDto> Entries {
      get; internal set;
    }


  } // class BalanzaColumnasMonedaDto


  public class BalanzaColumnasMonedaEntryDto : ITrialBalanceEntryDto {

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
      get; internal set;
    }

    public decimal DollarBalance {
      get;
      internal set;
    }

    public decimal YenBalance {
      get; internal set;
    }

    public decimal EuroBalance {
      get; internal set;
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

  }  // class BalanzaColumnasMonedaEntryDto


} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
