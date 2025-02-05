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


    public string ItemTypeName {
      get; internal set;
    }


    public string AccountType {
      get; internal set;
    } = string.Empty;


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


    public decimal ClosingExchangeRateForDollar {
      get; internal set;
    }


    public decimal ClosingExchangeRateForYen {
      get; internal set;
    }


    public decimal ClosingExchangeRateForEuro {
      get; internal set;
    }


    public decimal ClosingExchangeRateForUdi {
      get; internal set;
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


    public decimal ValorizedDailyDollarBalance {
      get; internal set;
    }


    public decimal ValorizedDailyYenBalance {
      get; internal set;
    }


    public decimal ValorizedDailyEuroBalance {
      get; internal set;
    }


    public decimal ValorizedDailyUdisBalance {
      get; internal set;
    }


    public string ERI {
      get; internal set;
    } = string.Empty;


    public string ComplementDescription {
      get; internal set;
    } = string.Empty;


    public string ComplementDetail {
      get; internal set;
    } = string.Empty;


    public string AccountLevel {
      get; internal set;
    } = string.Empty;


    public string CategoryType {
      get; internal set;
    } = string.Empty;


    public string SiglasUSD {
      get; internal set;
    }


    public string SiglasYEN {
      get; internal set;
    }


    public string SiglasEURO {
      get; internal set;
    }


    public string SiglasUDI {
      get; internal set;
    }


    public decimal ValorizedDailyDollarBalanceNeg {
      get; internal set;
    }


    public decimal DollarBalanceNeg {
      get; internal set;
    }


    public decimal ValorizedDailyYenBalanceNeg {
      get; internal set;
    }


    public decimal YenBalanceNeg {
      get; internal set;
    }


    public decimal ValorizedDailyEuroBalanceNeg {
      get; internal set;
    }


    public decimal EuroBalanceNeg {
      get; internal set;
    }


    public decimal ValorizedDailyUdisBalanceNeg {
      get; internal set;
    }


    public decimal UdisBalanceNeg {
      get; internal set;
    }
    

    string ITrialBalanceEntryDto.SubledgerAccountNumber {
      get {
        return String.Empty;
      }
    }


  }  // class BalanzaColumnasMonedaEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
