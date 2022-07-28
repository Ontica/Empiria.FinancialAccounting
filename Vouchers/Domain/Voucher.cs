/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Aggregate root                          *
*  Type     : Voucher                                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an accounting voucher.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Represents an accounting voucher.</summary>
  internal class Voucher {

    private Lazy<FixedList<VoucherEntry>> _entries;

    #region Constructors and parsers

    private Voucher() {
      // Required by Empiria Framework.
      RefreshEntries();
    }

    internal Voucher(VoucherFields fields) {
      Assertion.Require(fields, "fields");

      this.LoadFields(fields);

      this.Number = "No actualizada";
      this.RecordingDate = DateTime.Today;
      this.IsOpened = true;

      if (this.ElaboratedBy.IsEmptyInstance) {
        this.ElaboratedBy = Participant.Current;
      }

      RefreshEntries();
    }


    static public Voucher Parse(long id) {
      return VoucherData.GetVoucher(id);
    }

    internal static Voucher TryParse(Ledger ledger, string voucherNumber) {
      return VoucherData.TryGetVoucher(ledger, voucherNumber);
    }

    static public FixedList<Voucher> GetList(string filter, string sort, int pageSize) {
      return VoucherData.GetVouchers(filter, sort, pageSize);
    }


    static public Voucher Empty => Parse(-1);

    #endregion Constructors and parsers

    #region Public properties


    [DataField("ID_TRANSACCION")]
    public long Id {
      get;
      private set;
    }


    [DataField("NUMERO_TRANSACCION")]
    public string Number {
      get;
      private set;
    }


    [DataField("CONCEPTO_TRANSACCION")]
    public string Concept {
      get;
      private set;
    }


    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get;
      private set;
    }


    [DataField("ID_TIPO_TRANSACCION", ConvertFrom = typeof(long))]
    public TransactionType TransactionType {
      get;
      private set;
    }


    [DataField("ID_TIPO_POLIZA", ConvertFrom = typeof(long))]
    public VoucherType VoucherType {
      get;
      private set;
    }


    [DataField("ID_FUENTE", ConvertFrom = typeof(long))]
    public FunctionalArea FunctionalArea {
      get;
      private set;
    }


    [DataField("FECHA_AFECTACION", Default = "ExecutionServer.DateMaxValue")]
    public DateTime AccountingDate {
      get;
      private set;
    }


    [DataField("FECHA_REGISTRO", Default = "ExecutionServer.DateMaxValue")]
    public DateTime RecordingDate {
      get;
      private set;
    }


    [DataField("ESTA_ABIERTA", ConvertFrom = typeof(int))]
    public bool IsOpened {
      get;
      private set;
    }


    [DataField("ID_ELABORADA_POR", ConvertFrom = typeof(long))]
    public Participant ElaboratedBy {
      get;
      private set;
    } = Participant.Empty;


    [DataField("ID_AUTORIZADA_POR", ConvertFrom = typeof(long))]
    public Participant AuthorizedBy {
      get;
      private set;
    } = Participant.Empty;


    [DataField("ID_ENVIADA_DIARIO_POR", ConvertFrom = typeof(long))]
    public Participant ClosedBy {
      get;
      private set;
    } = Participant.Empty;


    public FixedList<VoucherEntry> Entries {
      get {
        return _entries.Value;
      }
    }


    public bool IsEmptyInstance {
      get {
        return this.Id == -1;
      }
    }


    public string StatusName {
      get {
        if (!IsOpened) {
          return "Enviada al diario";
        }
        if (this.AuthorizedBy.Equals(GetSupervisor())) {
          return "Enviada al supervisor";
        }
        return "Pendiente";
      }
    }

    internal bool IsAccountingDateOpened {
      get {
        return this.Ledger.IsAccountingDateOpened(this.AccountingDate);
      }
    }


    #endregion Public properties

    #region Methods

    internal VoucherEntry AppendEntry(VoucherEntryFields fields) {
      Assertion.Require(fields, "fields");
      Assertion.Require(this.IsOpened, "No se puede agregar el movimiento porque la póliza ya está cerrada.");

      fields.EnsureValidFor(this);

      var voucherEntry = new VoucherEntry(this, fields);

      voucherEntry.Save();

      this.RefreshEntries();

      return voucherEntry;
    }


    internal bool CanBeClosedBy(Participant participant) {
      if (!IsOpened) {
        return false;
      }

      //if (!(this.ElaboratedBy.Equals(participant) || this.AuthorizedBy.Equals(participant))) {
      //  return false;
      //}

      Participant supervisor = this.GetSupervisor();

      if (participant.Equals(supervisor)) {
        return true;
      }

      if (this.IsAccountingDateOpened) {
        return true;
      }

      return false;
    }


    internal void Close() {
      Assertion.Require(this.IsOpened, "Esta póliza ya está cerrada.");

      if (!this.IsValid()) {
        FixedList<string> validationResult = this.ValidationResult(true);

        Assertion.RequireFail("La póliza no puede enviarse al diario porque tiene datos inconsistentes.\n\n" +
                              EmpiriaString.ToString(validationResult));
      }

      RequireAllEntriesAreValidBeforeClose();

      DateTime lastRecordingDate = this.RecordingDate;

      this.AuthorizedBy = Participant.Current;
      this.ClosedBy = Participant.Current;
      this.RecordingDate = DateTime.Today;
      this.IsOpened = false;
      this.Number = VoucherData.GetVoucherNumberFor(this);

      try {

        VoucherData.CloseVoucher(this);

        TrialBalanceCache.Invalidate(this.AccountingDate);

      } catch {
        this.AuthorizedBy = Participant.Empty;
        this.ClosedBy = Participant.Empty;
        this.RecordingDate = lastRecordingDate;
        this.IsOpened = true;
        this.Number = "No actualizada";
        throw;
      }

      this.RefreshEntries();
    }


    internal void Delete() {
      Assertion.Require(this.IsOpened, "Esta póliza no puede eliminarse porque ya está cerrada.");

      VoucherData.DeleteVoucher(this);
    }


    internal void DeleteEntry(VoucherEntry entry) {
      Assertion.Require(entry, "entry");

      Assertion.Require(this.IsOpened, "No se puede eliminar el movimiento porque la póliza ya está cerrada.");
      Assertion.Require(this.Entries.Contains(entry), "El movimiento que se desea eliminar no pertenece a esta póliza");

      entry.Delete();

      this.RefreshEntries();
    }


    private void RequireAllEntriesAreValidBeforeClose() {
      this.RefreshEntries();

      Assertion.Require(this.Entries.All(x => x.LedgerAccount.Ledger.Equals(this.Ledger)),
          $"Cuando menos un movimiento tiene una cuenta que pertenece a otro mayor. (Póliza {this.Id}).");

      Assertion.Require(this.Entries.All(x => x.Amount > 0),
          $"Cuando menos un movimiento tiene un importe negativo o igual a cero. (Póliza {this.Id}).");

      Assertion.Require(this.Entries.All(x => x.BaseCurrencyAmount > 0),
          $"Cuando menos un movimiento tiene un importe negativo o igual a cero" +
          $"para la moneda origen (Póliza {this.Id}).");

      Assertion.Require(this.Entries.All(x => x.SubledgerAccount.IsEmptyInstance ||
                        (x.SubledgerAccount.BelongsTo(this.Ledger) && !x.SubledgerAccount.Suspended)),
          $"Cuando menos un movimiento tiene un auxiliar que pertenece " +
          $"a otra contabilidad o está suspendido. (Póliza {this.Id}).");
    }


    internal VoucherEntry GetCopyOfLastEntry() {
      Assertion.Require(this.Entries.Count > 0, "Esta póliza aún no tiene movimientos.");

      var list = new List<VoucherEntry>(this.Entries);

      VoucherEntry lastEntry = list.OrderByDescending(x => x.Id)
                                   .First();

      return lastEntry.CreateCopy();
    }


    internal VoucherEntry GetEntry(long voucherEntryId) {
      var entry = Entries.Find(x => x.Id == voucherEntryId);

      Assertion.Require(entry, $"La póliza no tiene registrado un movimiento con id {voucherEntryId}.");

      return entry;
    }


    internal FixedList<VoucherTotal> GetTotals() {
      var totalsList = new List<VoucherTotal>();

      IEnumerable<Currency> currencies = this.Entries.Select(x => x.Currency)
                                                     .Distinct()
                                                     .OrderBy(x => x.Code);

      foreach (var currency in currencies) {
        var debitsEntries = this.Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Debit &&
                                                      x.Currency.Equals(currency));
        var creditsEntries = this.Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Credit &&
                                                       x.Currency.Equals(currency));

        decimal totalDebits = debitsEntries.Sum(x => x.Debit);
        decimal totalCredits = creditsEntries.Sum(x => x.Credit);

        var total = new VoucherTotal(currency, totalDebits, totalCredits);

        totalsList.Add(total);
      }

      return totalsList.ToFixedList();
    }


    public bool IsValid() {
      if (!this.IsOpened) {
        return true;
      }

      return (this.ValidationResult(false).Count == 0);
    }


    private void LoadFields(VoucherFields fields) {
      this.Ledger = Ledger.Parse(fields.LedgerUID);
      this.AccountingDate = fields.AccountingDate;
      this.RecordingDate = fields.RecordingDate;

      if (fields.ElaboratedByUID.Length == 0) {
        this.ElaboratedBy = Participant.Current;
      } else {
        this.ElaboratedBy = Participant.Parse(fields.ElaboratedByUID);
      }

      this.Concept = EmpiriaString.TrimAll(fields.Concept).ToUpperInvariant();
      this.VoucherType = VoucherType.Parse(fields.VoucherTypeUID);
      this.TransactionType = TransactionType.Parse(fields.TransactionTypeUID);
      this.FunctionalArea = FunctionalArea.Parse(fields.FunctionalAreaId);
    }


    protected internal void Save() {
      if (this.Id == 0) {
        this.Id = VoucherData.NextVoucherId();
      }
      VoucherData.WriteVoucher(this);
    }


    private Participant GetSupervisor() {
      return Participant.Parse(135);
    }


    private void RefreshEntries() {
      if (!this.IsEmptyInstance) {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => VoucherData.GetVoucherEntries(this));

      } else {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => new FixedList<VoucherEntry>());
      }
    }

    // Ya tenemos el primero asunto de la cuenta 01.09.05.02.02.04 ahora con auxiliares y sin auxiliares hasta el 24 de enero
    internal FixedList<SubledgerAccount> SearchSubledgerAccountsForEdition(LedgerAccount account, string keywords) {
      Assertion.Require(this.IsOpened, "No hay cuentas auxiliares para edición porque la póliza ya está cerrada.");

      Assertion.Require(account.Ledger.Equals(this.Ledger), "Account does not belong to voucher ledger.");

      var historic = account.GetHistoric(this.AccountingDate);

      Assertion.Require(historic.Role == AccountRole.Control || historic.Role == AccountRole.Sectorizada,
                       "The account role is not 'Control', so there are not subledger accounts to return.");

      return VoucherData.SearchSubledgerAccountsForVoucherEdition(this, keywords);
    }


    internal void SendToSupervisor() {
      Assertion.Require(this.IsOpened, "La póliza no se puede enviar al supervisor porque no está abierta.");

      Assertion.Require(this.IsValid(), "La póliza no puede enviarse al supervisor porque " +
                                        "tiene datos con inconsistencias o no está balanceada.");

      this.AuthorizedBy = GetSupervisor();

      this.Save();
    }


    internal void Update(VoucherFields fields) {
      Assertion.Require(fields, "fields");

      this.Ledger = FieldPatcher.PatchField(fields.LedgerUID, this.Ledger);
      this.AccountingDate = FieldPatcher.PatchField(fields.AccountingDate, this.AccountingDate);
      this.RecordingDate = FieldPatcher.PatchField(fields.RecordingDate, this.RecordingDate);
      this.ElaboratedBy = FieldPatcher.PatchField(fields.ElaboratedByUID, this.ElaboratedBy);
      this.Concept = FieldPatcher.PatchField(EmpiriaString.TrimAll(fields.Concept), this.Concept);
      // this.TransactionType = FieldPatcher.PatchField(fields.TransactionTypeUID, this.TransactionType);
      this.VoucherType = FieldPatcher.PatchField(fields.VoucherTypeUID, this.VoucherType);
      this.FunctionalArea = FieldPatcher.PatchField(fields.FunctionalAreaId, this.FunctionalArea);
    }


    internal void UpdateEntry(VoucherEntry entry, VoucherEntryFields fields) {
      Assertion.Require(entry, "entry");
      Assertion.Require(this.IsOpened, "No se puede actualizar el movimiento porque la póliza ya está cerrada.");
      Assertion.Require(this.Entries.Contains(entry), "El movimiento que se desea modificar no pertenece a esta póliza");

      entry.Update(fields);

      this.RefreshEntries();
    }


    internal FixedList<string> ValidationResult(bool fullValidation) {
      this.RefreshEntries();

      string exception = string.Empty;

      string UID_POLIZA_CARGA_SALDOS_INICIALES = "c94bfd2b-84a7-4807-99fc-d4cb23cd43e3";
      string UID_POLIZA_EFECTOS_INICIALES_ADOPCION_NORMA = "e05e39ed-e744-43e1-b7b1-8e7b6c4e9895";

      if (AccountingDate.Date == new DateTime(2022, 1, 1) && VoucherType.UID != UID_POLIZA_CARGA_SALDOS_INICIALES) {
        exception = "El primero de enero de 2022 sólo permite el registro de pólizas de 'Carga de saldos iniciales'.";
      }

      if (AccountingDate.Date != new DateTime(2022, 1, 1) && VoucherType.UID == UID_POLIZA_CARGA_SALDOS_INICIALES) {
        exception = "Las pólizas de 'Carga de saldos iniciales' deben registrarse con fecha primero de enero de 2022.";
      }


      if (AccountingDate.Date == new DateTime(2022, 1, 2) && VoucherType.UID != UID_POLIZA_EFECTOS_INICIALES_ADOPCION_NORMA) {
        exception = "El 2 de enero de 2022 sólo permite el registro de pólizas de 'Efectos iniciales de adopción de Norma'.";
      }

      if (AccountingDate.Date != new DateTime(2022, 1, 2) && VoucherType.UID == UID_POLIZA_EFECTOS_INICIALES_ADOPCION_NORMA) {
        exception = "Las pólizas de 'Efectos iniciales de adopción de Norma' deben registrarse con fecha 2 de enero de 2022.";
      }

      var validator = new VoucherValidator(this.Ledger, this.AccountingDate);

      var list = validator.Validate(this.Entries, fullValidation).ToList();

      if (exception.Length != 0) {
        list.Insert(0, exception);
      }

      return list.ToFixedList();
    }


    #endregion Methods

  }  // class Voucher

}  // namespace Empiria.FinancialAccounting.Vouchers
