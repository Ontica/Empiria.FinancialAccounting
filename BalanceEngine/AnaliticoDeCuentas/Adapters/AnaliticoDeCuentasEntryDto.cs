/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : AnalyticBalanceEntryDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of a analytic balance with separated domestic            *
*             and foreign currencies totals                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Json;
using Newtonsoft.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {


  /// <summary>Output DTO used to return the 'Analitico de cuentas' report.</summary>
  public class AnaliticoDeCuentasDto {

    public TrialBalanceCommand Command {
      get; set;
    } = new TrialBalanceCommand();


    public FixedList<DataTableColumn> Columns {
      get; set;
    } = new FixedList<DataTableColumn>();


    public FixedList<AnaliticoDeCuentasEntryDto> Entries {
      get; set;
    } = new FixedList<AnaliticoDeCuentasEntryDto>();

  }



  /// <summary>Output DTO used to return the entries of an analytic balance with separated domestic
  /// and foreign currencies totals.</summary>
  public class AnaliticoDeCuentasEntryDto : ITrialBalanceEntryDto {

    [JsonProperty]
    public TrialBalanceItemType ItemType {
      get; internal set;
    }


    [JsonProperty]
    public string LedgerUID {
      get; internal set;
    }


    [JsonProperty]
    public string LedgerNumber {
      get; internal set;
    }


    [JsonProperty]
    public string LedgerName {
      get; internal set;
    }


    [JsonProperty]
    public string CurrencyCode {
      get; internal set;
    }


    [JsonProperty]
    public int StandardAccountId {
      get; internal set;
    }


    [JsonProperty]
    public int SubledgerAccountId {
      get; internal set;
    }


    [JsonProperty]
    public string AccountNumber {
      get; internal set;
    }


    [JsonProperty]
    public string AccountNumberForBalances {
      get; internal set;
    }


    [JsonProperty]
    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


    [JsonProperty]
    public string AccountName {
      get; internal set;
    }


    [JsonProperty]
    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


    [JsonProperty]
    public AccountRole AccountRole {
      get; internal set;
    }


    [JsonProperty]
    public int AccountLevel {
      get; internal set;
    }


    [JsonProperty]
    public string SectorCode {
      get; internal set;
    }

    [JsonProperty]
    public decimal InitialBalance {
      get; internal set;
    }


    [JsonProperty]
    public decimal Debit {
      get; internal set;
    }


    [JsonProperty]
    public decimal Credit {
      get; internal set;
    }


    [JsonProperty]
    public decimal DomesticBalance {
      get; internal set;
    }


    [JsonProperty]
    public decimal ForeignBalance {
      get; internal set;
    }


    [JsonProperty]
    public decimal TotalBalance {
      get; internal set;
    }


    [JsonProperty]
    public decimal ExchangeRate {
      get; internal set;
    }


    [JsonProperty]
    public decimal AverageBalance {
      get; internal set;
    }


    [JsonProperty]
    public DateTime LastChangeDate {
      get; set;
    }


    [JsonProperty]
    public bool HasAccountStatement {
      get; internal set;
    } = false;


    [JsonProperty]
    public bool ClickableEntry {
      get; internal set;
    } = false;


    [JsonProperty]
    public bool IsParentPostingEntry {
      get; internal set;
    }

    public bool Equals(AnaliticoDeCuentasEntryDto entry) {
      return entry.GetHashCode() == this.GetHashCode();
    }

    public override bool Equals(object obj) {
      return obj is AnaliticoDeCuentasEntryDto entry &&
             this.Equals(entry);
    }


    public string ToJson() {
      return JsonObject.Parse(this)
                       .ToString();
    }


    public string Hash() {
      return EmpiriaString.Combine(
        ItemType,
        LedgerNumber,
        AccountNumber,
        CurrencyCode,
        SectorCode,
        StandardAccountId,
        SubledgerAccountNumber,
        DomesticBalance.ToString("C2"),
        ForeignBalance.ToString("C2"),
        TotalBalance.ToString("C2")
        // LastChangeDate
      );
    }


    public override int GetHashCode() {
      return this.Hash()
                 .GetHashCode();
    }

  }  // class AnalyticBalanceEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
