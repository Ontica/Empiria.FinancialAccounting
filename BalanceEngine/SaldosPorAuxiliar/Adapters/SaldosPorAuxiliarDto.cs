﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : SaldosPorAuxiliarDto                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a saldos por auxiliar report.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

using Empiria.DynamicData;
using Empiria.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Output DTO used to return a saldos por auxiliar report.</summary>
  public class SaldosPorAuxiliarDto {

    [JsonProperty]
    public TrialBalanceQuery Query {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<SaldosPorAuxiliarEntryDto> Entries {
      get; internal set;
    }


  } // SaldosPorAuxiliarDto


  public sealed class SaldosPorAuxiliarEntryDto : ITrialBalanceEntryDto {

    public TrialBalanceItemType ItemType {
      get; internal set;
    }

    public string LedgerUID {
      get; internal set;
    } = string.Empty;


    public string LedgerNumber {
      get; internal set;
    }


    public string LedgerName {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public string CurrencyName {
      get;
      internal set;
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


    public string AccountNumberForBalances {
      get; internal set;
    }


    public string AccountName {
      get; internal set;
    }


    public string SubledgerAccountNumber {
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
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }


    public decimal? CurrentBalance {
      get; internal set;
    }


    public decimal CurrentBalanceForBalances {
      get; internal set;
    }


    public decimal ExchangeRate {
      get; internal set;
    }


    public decimal SecondExchangeRate {
      get; internal set;
    }


    public decimal? AverageBalance {
      get; internal set;
    }


    public DateTime LastChangeDate {
      get; internal set;
    }


    public DateTime LastChangeDateForBalances {
      get; internal set;
    }


    public string DebtorCreditor {
      get; internal set;
    } = string.Empty;


    public bool HasAccountStatement {
      get; internal set;
    } = false;


    public bool IsParentPostingEntry {
      get; set;
    } = false;


    public bool ClickableEntry {
      get; internal set;
    } = false;


    public string ToJson() {
      return JsonObject.Parse(this)
                       .ToString();
    }

  }

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
