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

using Newtonsoft.Json;

using Empiria.Json;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return the 'Analitico de cuentas' report.</summary>
  public class AnaliticoDeCuentasDto {

    [JsonProperty]
    public TrialBalanceQuery Query {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    [JsonProperty]
    public FixedList<AnaliticoDeCuentasEntryDto> Entries {
      get; internal set;
    }

  }  // class AnaliticoDeCuentasDto



  /// <summary>Output DTO used to return the entries of an analytic balance with separated domestic
  /// and foreign currencies totals.</summary>
  public sealed class AnaliticoDeCuentasEntryDto : ITrialBalanceEntryDto,
                                                   IEquatable<AnaliticoDeCuentasEntryDto> {

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
    public string AccountName {
      get; internal set;
    }


    [JsonProperty]
    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


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
    public string AccountMark {
      get; internal set;
    }


    [JsonProperty]
    public string SectorCode {
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
    public decimal AverageBalance {
      get; internal set;
    }


    [JsonProperty]
    public DateTime LastChangeDate {
      get; set;
    }

    public override bool Equals(object obj) => this.Equals(obj as AnaliticoDeCuentasEntryDto);

    public bool Equals(AnaliticoDeCuentasEntryDto entry) {
      if (entry == null) {
        return false;
      }

      return this.GetHashCode() == entry.GetHashCode();
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
        // AccountMark,
        SectorCode,
        StandardAccountId,
        SubledgerAccountId,
        SubledgerAccountNumber,
        DebtorCreditor,
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
