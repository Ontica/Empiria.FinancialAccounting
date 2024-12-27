/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : BalanzaDiferenciaDiariaMonedaDto           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a Balanza diferencia diaria por moneda.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.DynamicData;
using Empiria.Json;
using Newtonsoft.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return a Balanza diferencia diaria por moneda.</summary>
  public class BalanzaDiferenciaDiariaMonedaDto {

    [JsonProperty]
    public TrialBalanceQuery Query {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<BalanzaDiferenciaDiariaMonedaEntryDto> Entries {
      get; internal set;
    }

  } // class BalanzaDiferenciaDiariaMonedaDto


  public class BalanzaDiferenciaDiariaMonedaEntryDto : ITrialBalanceEntryDto {

    public string AccountNumber {
      get; internal set;
    }


    public string AccountName {
      get; internal set;
    }


    public TrialBalanceItemType ItemType {
      get; internal set;
    } = TrialBalanceItemType.Entry;


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


    public string SectorCode {
      get; internal set;
    }


    public DateTime FromDate {
      get; internal set;
    }


    public DateTime ToDate {
      get; internal set;
    }


    public decimal DomesticDailyBalance {
      get; internal set;
    }


    public decimal DollarDailyBalance {
      get; internal set;
    }


    public decimal YenDailyBalance {
      get; internal set;
    }


    public decimal EuroDailyBalance {
      get; internal set;
    }


    public decimal UdisDailyBalance {
      get; internal set;
    }


    public decimal DomesticBalance {
      get; internal set;
    }


    public decimal DollarBalance {
      get; internal set;
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

    
    public decimal ExchangeRateForDollar {
      get;
      internal set;
    }


    public decimal ExchangeRateForYen {
      get;
      internal set;
    }


    public decimal ExchangeRateForEuro {
      get;
      internal set;
    }


    public decimal ExchangeRateForUdi {
      get;
      internal set;
    }


    public decimal ValorizedDollarBalance {
      get;
      internal set;
    }


    public decimal ValorizedYenBalance {
      get;
      internal set;
    }


    public decimal ValorizedEuroBalance {
      get;
      internal set;
    }


    public decimal ValorizedUdisBalance {
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


  }  // class BalanzaColumnasMonedaEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
