﻿/* Empiria Financial *****************************************************************************************
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

  /// <summary>Contains edition and opeeration flags for an accounting voucher.</summary>
  public class VoucherActions {

    internal VoucherActions() {
      // no-op
    }

    public bool ChangeConcept {
      get; internal set;
    }

    public bool CloneVoucher {
      get; internal set;
    }

    public bool DeleteVoucher {
      get; internal set;
    }

    public bool EditVoucher {
      get; internal set;
    }


    public bool ReviewVoucher {
      get; internal set;
    }


    public bool SendToLedger {
      get; internal set;
    }


    public bool SendToSupervisor {
      get; internal set;
    }

  }  // VoucherActions



  /// <summary>Represents an accounting voucher.</summary>
  internal class Voucher {

    private Lazy<FixedList<VoucherEntry>> _entries;

    #region Constructors and parsers

    private Voucher() {
      // Required by Empiria Framework.
      RefreshEntries();
    }

    internal Voucher(VoucherFields fields) {
      Assertion.Require(fields, nameof(fields));

      this.LoadFields(fields);

      this.Number = "No actualizada";
      this.RecordingDate = DateTime.Today;
      this.IsOpened = true;

      if (this.ElaboratedBy.IsEmptyInstance) {
        this.ElaboratedBy = Participant.Current;
      }

      RefreshEntries();
    }


    internal Voucher(VoucherFields fields,
                     IEnumerable<VoucherEntryFields> entriesFields) : this(fields) {
      Assertion.Require(entriesFields,nameof(entriesFields));

      var entries = new List<VoucherEntry>(entriesFields.Count());

      foreach (var entryFields in entriesFields) {
        var voucherEntry = new VoucherEntry(this, entryFields);

        entries.Add(voucherEntry);
      }

      _entries = new Lazy<FixedList<VoucherEntry>>(() => entries.ToFixedList());
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


    public bool IsClosed {
      get {
        return !IsOpened;
      }
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


    public bool SentToSupervisor {
      get {
        return !AuthorizedBy.Equals(Participant.Empty);
      }
    }


    public string StatusName {
      get {
        if (IsClosed) {
          return "Enviada al diario";
        }
        if (IsSupervisor(this.AuthorizedBy)) {
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


    public VoucherActions Actions {
      get {
        if (this.IsClosed) {
          return new VoucherActions {
            ChangeConcept = this.IsAccountingDateOpened,
            CloneVoucher = true
          };
        }

        bool isAssignedToCurrentUser = this.ElaboratedBy.Equals(Participant.Current);
        bool wasSentToAnotherUser = !this.AuthorizedBy.IsEmptyInstance &&
                                    !this.AuthorizedBy.Equals(Participant.Current);

        if (!this.IsValid()) {
          return new VoucherActions {
            DeleteVoucher = true,
            EditVoucher = true,
            ReviewVoucher = true
          };
        }

        if (this.CanBeClosedBy(Participant.Current)) {
          return new VoucherActions {
            CloneVoucher = true,
            DeleteVoucher = true,
            EditVoucher = true,
            SendToLedger = true
          };

        } else if (!wasSentToAnotherUser) {
          return new VoucherActions {
            CloneVoucher = true,
            DeleteVoucher = true,
            EditVoucher = true,
            SendToSupervisor = true
          };

        } else {  // wasSentToAnotherUser
          return new VoucherActions {
            CloneVoucher = true,
          };
        }
      }
    }

    #endregion Public properties

    #region Methods

    internal VoucherEntry AppendAndSaveEntry(VoucherEntryFields fields) {
      Assertion.Require(fields, nameof(fields));
      Assertion.Require(this.IsOpened, "No se puede agregar el movimiento porque la póliza ya está cerrada.");

      fields.EnsureValidFor(this);

      var voucherEntry= new VoucherEntry(this, fields);

      voucherEntry.Save();

      this.RefreshEntries();

      return voucherEntry;
    }


    internal bool CanBeClosedBy(Participant participant) {
      if (IsClosed) {
        return false;
      }

      if (IsSupervisor(participant)) {
        return true;
      }

      if (HasProtectedAccounts() && !CanCloseWithProtectedAccounts()) {
        return false;
      }

      if (this.IsAccountingDateOpened) {
        return true;
      }

      return false;
    }


    internal bool CanCloseWithProtectedAccounts() {
      return ExecutionServer.CurrentPrincipal.IsInRole("administrador-operativo") ||
             ExecutionServer.CurrentPrincipal.HasPermission("registro-manual-cuentas-protegidas");
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
      Assertion.Require(entry, nameof(entry));

      Assertion.Require(this.IsOpened, "No se puede eliminar el movimiento porque la póliza ya está cerrada.");
      Assertion.Require(this.Entries.Contains(entry), "El movimiento que se desea eliminar no pertenece a esta póliza");

      entry.Delete();

      this.RefreshEntries();
    }


    internal bool HasProtectedAccounts() {
      return this.Entries.Contains(x => x.LedgerAccount.Number.StartsWith("4.02.03.01"));
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
      if (this.IsClosed) {
        return true;
      }

      return (this.ValidationResult(false).Count == 0);
    }


    private void LoadFields(VoucherFields fields) {
      this.Ledger = Ledger.Parse(fields.LedgerUID);
      this.AccountingDate = fields.AccountingDate;
      this.RecordingDate = fields.RecordingDate;

      if (fields.ElaboratedById == 0) {
        this.ElaboratedBy = Participant.Current;
      } else {
        this.ElaboratedBy = Participant.Parse(fields.ElaboratedById);
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


    public bool CurrentUserIsSupervisor() {
      var workgroup = VoucherWorkgroup.Parse("Vouchers.Authorization.Group");

      return workgroup.Members.Contains(x => x.Id == ExecutionServer.CurrentUserId);
    }


    private bool IsSupervisor(Participant participant) {
      var workgroup = VoucherWorkgroup.Parse("Vouchers.Authorization.Group");

      return workgroup.Members.Contains(participant);
    }


    private Participant GetAccountingManager() {
      var workgroup = VoucherWorkgroup.Parse("Accounting.Manager");

      return workgroup.Members[0];
    }


    private void RefreshEntries() {
      if (!this.IsEmptyInstance) {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => VoucherData.GetVoucherEntries(this));

      } else {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => new FixedList<VoucherEntry>());
      }
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


    internal FixedList<SubledgerAccount> SearchSubledgerAccountsForEdition(LedgerAccount account, string keywords) {
      Assertion.Require(this.IsOpened, "No hay cuentas auxiliares para edición porque la póliza ya está cerrada.");

      Assertion.Require(account.Ledger.Equals(this.Ledger), "Account does not belong to voucher ledger.");

      var historic = account.GetHistoric(this.AccountingDate);

      Assertion.Require(historic.Role == AccountRole.Control || historic.Role == AccountRole.Sectorizada,
                       "The account role is not 'Control', so there are not subledger accounts to return.");

      return VoucherData.SearchSubledgerAccountsForVoucherEdition(this, keywords);
    }


    internal void SendToSupervisor() {
      Assertion.Require(this.IsOpened, "La póliza no se puede enviar a supervisión porque no está abierta.");

      Assertion.Require(this.IsValid(), "La póliza no puede enviarse a supervisión debido a que " +
                                        "tiene datos con inconsistencias o no está balanceada.");

      //Assertion.Require(this.ElaboratedBy.Equals(Participant.Current) || IsSupervisor(Participant.Current),
      //                                   "La póliza solo puede ser enviada a supervisión " +
      //                                   "por la persona que la elaboró o por alguien con el rol supervisor.");

      this.AuthorizedBy = GetAccountingManager();

      this.Save();
    }


    internal void Update(VoucherFields fields) {
      Assertion.Require(fields, nameof(fields));

      this.Ledger = Patcher.Patch(fields.LedgerUID, this.Ledger);
      this.AccountingDate = Patcher.Patch(fields.AccountingDate, this.AccountingDate);
      this.RecordingDate = Patcher.Patch(fields.RecordingDate, this.RecordingDate);
      this.ElaboratedBy = Patcher.Patch(fields.ElaboratedById, this.ElaboratedBy);
      this.Concept = Patcher.PatchClean(fields.Concept, this.Concept);
      // this.TransactionType = FieldPatcher.PatchField(fields.TransactionTypeUID, this.TransactionType);
      this.VoucherType = Patcher.Patch(fields.VoucherTypeUID, this.VoucherType);
      this.FunctionalArea = Patcher.Patch(fields.FunctionalAreaId, this.FunctionalArea);
    }


    internal void UpdateConcept(string newConcept) {
      Assertion.Require(newConcept, nameof(newConcept));

      Assertion.Require(this.Actions.ChangeConcept,
          "Para efectuar el cambio de concepto, la póliza debe estar cerrada " +
          "y su fecha de afectación debe estar dentro de un período contable abierto.");

      this.Concept = EmpiriaString.TrimAll(newConcept);

      VoucherData.UpdateVoucherConcept(this);
    }


    internal void UpdateEntry(VoucherEntry entry, VoucherEntryFields fields) {
      Assertion.Require(entry, nameof(entry));
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

      if (HasProtectedAccounts() && !CanCloseWithProtectedAccounts()) {
        exception = "La persona usuaria no tiene permisos para enviar al diario pólizas con cuentas protegidas.";
      }

      var validator = new VoucherValidator(this.Ledger, this.AccountingDate);

      var list = validator.Validate(this.Entries, fullValidation).ToList();

      if (exception.Length != 0) {
        list.Insert(0, exception);
      }

      return list.ToFixedList();
    }


    internal void SaveAll() {
      this.Save();

      foreach (var entry in this.Entries) {
        entry.Save();
      }

      this.RefreshEntries();
    }

    #endregion Methods

  }  // class Voucher

}  // namespace Empiria.FinancialAccounting.Vouchers
