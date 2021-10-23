/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Information Holder                    *
*  Type     : ExcelVoucherEntry                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  :                                                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  internal class ExcelVoucherEntry {

    private readonly List<ToImportVoucherIssue> _entryIssues = new List<ToImportVoucherIssue>();

    internal ExcelVoucherEntry(string worksheetName, int worksheetSection, int row) {
      this.WorksheetName = worksheetName;
      this.WorksheetSection = worksheetSection;
      this.Row = row;
    }

    public string WorksheetName {
      get;

    }
    public int WorksheetSection {
      get;
    }

    public int Row {
      get;
    }


    public string VoucherUniqueID {
      get {
        return $"{this.WorksheetName}||{this.WorksheetSection.ToString("00000")}||{this.ResponsibilityAreaCode}";
      }
    }


    public string VoucherConcept {
      get; private set;
    }


    public string ImportationSet {
      get {
        return WorksheetName;
      }
    }


    public AccountsChart AccountsChart {
      get; private set;
    } = AccountsChart.Empty;


    private void SetAccountsChart() {
      if (this.BaseAccount.Length == 4) {
        this.AccountsChart = AccountsChart.Former;
      } else if (this.BaseAccount.Length == 1) {
        this.AccountsChart = AccountsChart.IFRS;
      } else {
        AddError("No puedo determinar el catálogo de cuentas a partir del archivo Excel.");
        this.AccountsChart = AccountsChart.Empty;
      }
    }

    private string BaseAccount {
      get; set;
    } = string.Empty;


    private string SubaccountWithSector {
      get; set;
    } = string.Empty;


    private string CurrencyCode {
      get; set;
    } = string.Empty;


    private string ResponsibilityAreaCode {
      get; set;
    } = string.Empty;


    private decimal Amount {
      get; set;
    }

    private string SubledgerAccount {
      get; set;
    } = string.Empty;


    private decimal ExchangeRate {
      get; set;
    }


    internal Currency GetCurrency() {
      if (this.CurrencyCode.Length == 0) {
        return Currency.Empty;
      }

      try {
        return Currency.Parse(this.CurrencyCode);
      } catch {
        AddError($"No reconozco la clave de moneda {this.CurrencyCode}.");

        return Currency.Empty;
      }
    }


    internal Sector GetSector() {
      if (this.SubaccountWithSector.Length == 0) {
        return Sector.Empty;
      }

      string sectorCode = this.SubaccountWithSector.Substring(this.SubaccountWithSector.Length - 2);

      try {
        return Sector.Parse(sectorCode);
      } catch {
        AddError($"No reconozco el sector {sectorCode}.");

        return Sector.Empty;
      }
    }


    internal Ledger GetLedger() {
      if (this.ResponsibilityAreaCode.Length == 0) {
        return Ledger.Empty;
      }
      if (this.AccountsChart.IsEmptyInstance) {
        return Ledger.Empty;
      }

      string ledgerCode = this.ResponsibilityAreaCode.Substring(1, 2);

      try {
        Ledger ledger = this.AccountsChart.TryGetLedger(ledgerCode);

        if (ledger == null) {
          AddError($"No existe una contabilidad con número '{ledgerCode}' " +
                   $"para el catálogo de cuentas {this.AccountsChart.Name}.");

        }

        return ledger ?? Ledger.Empty;

      } catch {
        AddError($"No existe una contabilidad con número '{ledgerCode}'" +
                 $"para el catálogo de cuentas {this.AccountsChart.Name}.");

        return Ledger.Empty;
      }
    }

    internal void SetConcept(string value) {
      value = EmpiriaString.TrimAll(value);

      if (String.IsNullOrWhiteSpace(value)) {
        AddError("El concepto de la póliza no puede ser una cadena vacía.");
      }
      this.VoucherConcept = value;
    }


    internal void SetBaseAccount(string value) {
      value = EmpiriaString.TrimAll(value);

      if (String.IsNullOrWhiteSpace(value)) {
        AddError("La celda no contiene el valor de la cuenta de mayor.");

      } else if (value.Length != 4 && value.Length != 1) {
        AddError($"No reconozco el formato de la cuenta de mayor {value}.");

      } else if (!EmpiriaString.IsInteger(value)) {
        AddError($"La celda B tiene caracteres que no reconozco ({value}).");

      } else {
        this.BaseAccount = value;

        SetAccountsChart();
      }
    }


    internal void SetSubaccountWithSector(string value) {
      value = EmpiriaString.TrimAll(value);

      if (String.IsNullOrWhiteSpace(value)) {
        AddError("La subcuenta en la celda B no contiene el valor de la subcuenta de mayor y el sector.");

      } else if (value.Length != "14060000000041".Length) {
        AddError($"La subcuenta en la celda B tiene un valor con una longitud que no coincide con la esperada ({value}).");

      } else if (!EmpiriaString.IsInteger(value)) {
        AddError($"La subcuenta de la celda B tiene caracteres que no reconozco ({value}).");

      } else {
        this.SubaccountWithSector = value;

      }
    }


    internal void SetDebitOrCredit(decimal debit, decimal credit) {
      if (debit < 0) {
        AddError($"El cargo en la columna E tiene un valor negativo ({debit.ToString("C2")}).");
        return;
      }
      if (credit < 0) {
        AddError($"El abono en la columna F tiene un valor negativo ({credit.ToString("C2")}).");
        return;
      }
      if (debit == 0 && credit == 0) {
        AddError($"Tanto la columna de cargos como la de abonos tienen importes iguales a cero.");
        return;
      }
      if (debit > 0 && credit > 0) {
        AddError($"Tanto la columna de cargos E ({debit.ToString("C2")}) como la de abonos F ({credit.ToString("C2")})," +
                 $"tienen importes mayores a cero.");
        return;
      }

      if (debit > 0) {
        this.Amount = debit;
        this.VoucherEntryType = VoucherEntryType.Debit;
      } else if (credit > 0) {
        this.Amount = credit;
        this.VoucherEntryType = VoucherEntryType.Credit;
      }
    }

    internal void SetCurrencyCode(string value) {
      value = EmpiriaString.TrimAll(value);

      if (String.IsNullOrWhiteSpace(value)) {
        AddError("La celda C no contiene la clave de la moneda.");
      } else if (value.Length != 2) {
        AddError($"La celda C tiene el valor '{value}', el cual no corresponde a una clave de moneda.");
      } else {
        this.CurrencyCode = value;
      }
    }


    internal void SetSubledgerAccount(string value) {
      value = EmpiriaString.TrimAll(value);

      if (String.IsNullOrWhiteSpace(value)) {
        this.SubledgerAccount = string.Empty;
      } else {
        this.SubledgerAccount = value;
      }
    }

    internal void SetResponsibilityAreaCode(string value) {
      value = EmpiriaString.TrimAll(value);

      if (String.IsNullOrWhiteSpace(value)) {
        AddError("La celda D no contiene la clave del área.");
      } else if (value.Length != 6) {
        AddError($"La celda D no contiene una clave válida de área ({value}).");
      } else {
        this.ResponsibilityAreaCode = value;
      }
    }

    internal void SetExchangeRate(decimal value) {
      if (value ==  0) {
        AddError($"La columna H tiene un tipo de cambio igual a cero.");
        return;
      }
      if (value < 0) {
        AddError($"La columna H tiene un tipo de cambio negativo ({value.ToString("C2")}).");
        return;
      }
      this.ExchangeRate = value;
    }

    internal FunctionalArea GetFunctionalArea() {
      if (this.ResponsibilityAreaCode.Length == 0) {
        return FunctionalArea.Empty;
      }
      try {
        return FunctionalArea.Parse(this.ResponsibilityAreaCode);
      } catch {
        AddError($"No puedo obtener el área funcional {this.ResponsibilityAreaCode}.");

        return FunctionalArea.Empty;
      }
    }


    internal FixedList<ToImportVoucherIssue> GetHeaderIssues() {
      return new FixedList<ToImportVoucherIssue>();
    }


    internal StandardAccount GetStandardAccount() {
      var ledger = GetLedger();

      if (ledger.IsEmptyInstance) {
        return StandardAccount.Empty;
      }

      if (this.BaseAccount.Length == 0 || this.SubaccountWithSector.Length == 0) {
        return StandardAccount.Empty;
      }

      string subaccount = this.SubaccountWithSector.Substring(0, this.SubaccountWithSector.Length - 2);

      string accountNumber = ledger.AccountsChart.FormatAccountNumber(this.BaseAccount + subaccount);

      return ledger.AccountsChart.GetStandardAccount(accountNumber);
    }


    internal SubsidiaryAccount GetSubledgerAccount() {
      if (this.SubledgerAccount.Length == 0) {
        return SubsidiaryAccount.Empty;
      }

      Ledger ledger = this.GetLedger();

      if (ledger.IsEmptyInstance) {
        return SubsidiaryAccount.Empty;
      }

      var formattedAccountNo = GetSubledgerAccountNo();

      SubsidiaryAccount subledgerAccount = ledger.TryGetSubledgerAccount(formattedAccountNo);

      if (subledgerAccount != null) {
        return subledgerAccount;
      } else {
        return SubsidiaryAccount.Empty;
      }
    }


    internal string GetSubledgerAccountNo() {
      if (this.SubledgerAccount.Length == 0) {
        return this.SubledgerAccount;
      }

      var ledger = GetLedger();

      if (ledger.IsEmptyInstance) {
        return this.SubledgerAccount;

      }
      return ledger.FormatSubledgerAccount(this.SubledgerAccount);
    }


    internal VoucherEntryType VoucherEntryType {
      get; private set;
    }


    internal decimal GetAmount() {
      return this.Amount;
    }


    internal decimal GetExchangeRate() {
      return this.ExchangeRate;
    }


    internal decimal GetBaseCurrencyAmount() {
      return this.GetAmount() * this.GetExchangeRate();
    }


    internal FixedList<ToImportVoucherIssue> GetEntryIssues() {
      return this._entryIssues.ToFixedList();
    }

    public void AddError(string msg) {
      _entryIssues.Add(new ToImportVoucherIssue(VoucherIssueType.Error,
                                                this.ImportationSet,
                                                $"Hoja {this.WorksheetName}, Renglón {this.Row}",
                                                msg));
    }


    public void AddWarning(string msg) {
      _entryIssues.Add(new ToImportVoucherIssue(VoucherIssueType.Warning,
                                                this.ImportationSet,
                                                $"Hoja {this.WorksheetName}, Renglón {this.Row}",
                                                msg));
    }


  }  // class ExcelVoucherEntry

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
