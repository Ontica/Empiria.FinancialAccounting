/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll      Pattern   : Empiria Plain Object                    *
*  Type     : VouchersByAccountEntry                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for vouchers by account.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Domain {

  public interface IVouchersByAccountEntry {

  }

  /// <summary>Represents an entry for vouchers by account.</summary>
  public class AccountStatementEntry : IVouchersByAccountEntry {


    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get; internal set;
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    public Currency Currency {
      get; internal set;
    }


    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public int StandardAccountId {
      get; internal set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get; internal set;
    }


    [DataField("ID_TRANSACCION", ConvertFrom = typeof(decimal))]
    public int VoucherId {
      get; internal set;
    }


    [DataField("ID_ELABORADA_POR", ConvertFrom = typeof(long))]
    public Participant ElaboratedBy {
      get; internal set;
    } = Participant.Empty;


    [DataField("ID_AUTORIZADA_POR", ConvertFrom = typeof(long))]
    public Participant AuthorizedBy {
      get; internal set;
    } = Participant.Empty;


    [DataField("ID_MOVIMIENTO", ConvertFrom = typeof(decimal))]
    public int VoucherEntryId {
      get; internal set;
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; internal set;
    }


    [DataField("NOMBRE_CUENTA_ESTANDAR")]
    public string AccountName {
      get; internal set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubledgerAccountNumber {
      get; internal set;
    }


    [DataField("NOMBRE_CUENTA_AUXILIAR")]
    public string SubledgerAccountName {
        get; internal set;
    }


    [DataField("NUMERO_TRANSACCION")]
    public string VoucherNumber {
      get; internal set;
    }


    [DataField("NUMERO_VERIFICACION")]
    public string VerificationNumber {
      get; internal set;
    }


    // Todo: Change string type to DebtorCreditor enumeration type (already exists)
    [DataField("NATURALEZA")]
    public string DebtorCreditor {
      get; internal set;
    }


    [DataField("FECHA_AFECTACION", Default = "ExecutionServer.DateMaxValue")]
    public DateTime AccountingDate {
      get; internal set;
    }


    [DataField("FECHA_REGISTRO", Default = "ExecutionServer.DateMaxValue")]
    public DateTime RecordingDate {
      get; internal set;
    }


    [DataField("CONCEPTO_TRANSACCION")]
    public string Concept {
      get; internal set;
    }


    [DataField("DEBE")]
    public decimal Debit {
      get; internal set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get; internal set;
    }


    public decimal CurrentBalance {
      get; internal set;
    }


    public decimal ExchangeRate {
      get; internal set;
    } = 1;


    public bool IsCurrentBalance {
      get; internal set;
    } = false;


    public bool HasParentPostingEntry {
      get; internal set;
    } = false;


    public bool IsParentPostingEntry {
      get; internal set;
    } = false;


    public TrialBalanceItemType ItemType {
      get; internal set;
    } = TrialBalanceItemType.Entry;


    internal static AccountStatementEntry MapToAccountStatementEntry(AccountStatementEntry entry) {
      return new AccountStatementEntry {
        ItemType = entry.ItemType,
        Ledger = entry.Ledger,
        Currency = entry.Currency,
        StandardAccountId = entry.StandardAccountId,
        Sector = entry.Sector,
        VoucherId = entry.VoucherId,
        ElaboratedBy = entry.ElaboratedBy,
        AuthorizedBy = entry.AuthorizedBy,
        VoucherEntryId = entry.VoucherEntryId,
        AccountNumber = entry.AccountNumber,
        AccountName = entry.AccountName,
        SubledgerAccountNumber = entry.SubledgerAccountNumber,
        VoucherNumber = entry.VoucherNumber,
        Debit = entry.Debit,
        Credit = entry.Credit,
        CurrentBalance = entry.CurrentBalance,
        DebtorCreditor = entry.DebtorCreditor,
        AccountingDate = entry.AccountingDate,
        RecordingDate = entry.RecordingDate,
        Concept = EmpiriaString.Clean(entry.Concept),
        IsCurrentBalance = entry.IsCurrentBalance,
        HasParentPostingEntry = entry.HasParentPostingEntry,
        IsParentPostingEntry = entry.IsParentPostingEntry,
      };
    }


    internal void MultiplyBy(decimal value) {
      this.Debit *= value;
      this.Credit *= value;
      this.CurrentBalance *= value;
      this.ExchangeRate = value;
    }


    internal void RoundBalances() {
      this.Debit = Math.Round(this.Debit, 2);
      this.Credit = Math.Round(this.Credit, 2);
      this.CurrentBalance = Math.Round(this.CurrentBalance, 2);
    }


    internal static AccountStatementEntry SetTotalAccountBalance(decimal balance) {

      var returnedEntry = new AccountStatementEntry {
        Ledger = Ledger.Empty,
        Currency = Currency.Empty,
        StandardAccountId = StandardAccount.Empty.Id,
        Sector = Sector.Empty,
        SubledgerAccountNumber = "",
        VoucherNumber = "",
        Concept = "",
        CurrentBalance = balance,
        ItemType = TrialBalanceItemType.Total
      };



      return returnedEntry;
    }


    internal void Sum(AccountStatementEntry entry) {
      this.Debit += entry.Debit;
      this.Credit += entry.Credit;
      this.CurrentBalance += entry.CurrentBalance;
    }


  } // class VouchersByAccountEntry


} // namespace Empiria.FinancialAccounting.Reporting.AccountStatements.Domain
