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
    private readonly VoucherHelper _helper;

    #region Constructors and parsers

    private Voucher() {
      // Required by Empiria Framework.

      RefreshEntries();

      _helper = new VoucherHelper(this);
    }


    internal Voucher(VoucherFields fields) {
      Assertion.Require(fields, nameof(fields));

      LoadFields(fields);

      Number = "No actualizada";
      RecordingDate = DateTime.Today;
      IsOpened = true;

      if (ElaboratedBy.IsEmptyInstance) {
        ElaboratedBy = Participant.Current;
      }

      RefreshEntries();

      _helper = new VoucherHelper(this);
    }


    internal Voucher(VoucherFields fields,
                     IEnumerable<VoucherEntryFields> entriesFields) : this(fields) {
      Assertion.Require(entriesFields, nameof(entriesFields));

      var entries = new List<VoucherEntry>(entriesFields.Count());

      foreach (var entryFields in entriesFields) {
        var voucherEntry = new VoucherEntry(this, entryFields);

        entries.Add(voucherEntry);
      }

      _entries = new Lazy<FixedList<VoucherEntry>>(() => entries.ToFixedList());
      _helper = new VoucherHelper(this);
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

    #region Properties

    [DataField("ID_TRANSACCION")]
    public long Id {
      get; private set;
    }


    [DataField("NUMERO_TRANSACCION")]
    public string Number {
      get; private set;
    }


    [DataField("CONCEPTO_TRANSACCION")]
    public string Concept {
      get; private set;
    }


    [DataField("ID_MAYOR", ConvertFrom = typeof(long))]
    public Ledger Ledger {
      get; private set;
    }


    [DataField("ID_TIPO_TRANSACCION", ConvertFrom = typeof(long))]
    public TransactionType TransactionType {
      get; private set;
    }


    [DataField("ID_TIPO_POLIZA", ConvertFrom = typeof(long))]
    public VoucherType VoucherType {
      get; private set;
    }


    [DataField("ID_FUENTE", ConvertFrom = typeof(long))]
    public FunctionalArea FunctionalArea {
      get; private set;
    }


    [DataField("FECHA_AFECTACION", Default = "ExecutionServer.DateMaxValue")]
    public DateTime AccountingDate {
      get; private set;
    }


    [DataField("FECHA_REGISTRO", Default = "ExecutionServer.DateMaxValue")]
    public DateTime RecordingDate {
      get; private set;
    }


    public bool IsClosed {
      get {
        return !IsOpened;
      }
    }


    [DataField("ESTA_ABIERTA", ConvertFrom = typeof(int))]
    public bool IsOpened {
      get; private set;
    }


    [DataField("ID_ELABORADA_POR", ConvertFrom = typeof(long))]
    public Participant ElaboratedBy {
      get; private set;
    } = Participant.Empty;


    [DataField("ID_AUTORIZADA_POR", ConvertFrom = typeof(long))]
    public Participant AuthorizedBy {
      get; private set;
    } = Participant.Empty;


    [DataField("ID_ENVIADA_DIARIO_POR", ConvertFrom = typeof(long))]
    public Participant ClosedBy {
      get; private set;
    } = Participant.Empty;


    public FixedList<VoucherEntry> Entries {
      get {
        return _entries.Value;
      }
    }


    public bool IsEmptyInstance {
      get {
        return Id == -1;
      }
    }


    internal VoucherHelper Helper {
      get {
        return _helper;
      }
    }


    public string StatusName {
      get {
        if (IsClosed) {
          return "Enviada al diario";
        }
        if (_helper.IsSupervisor(AuthorizedBy)) {
          return "Enviada al supervisor";
        }
        return "Pendiente";
      }
    }


    internal bool WasSentToSupervisor {
      get {
        return !AuthorizedBy.Equals(Participant.Empty);
      }
    }

    #endregion Properties

    #region Methods

    internal VoucherEntry AppendAndSaveEntry(VoucherEntryFields fields) {
      Assertion.Require(fields, nameof(fields));
      Assertion.Require(IsOpened, "No se puede agregar el movimiento porque la póliza ya está cerrada.");

      fields.EnsureValidFor(this);

      var voucherEntry = new VoucherEntry(this, fields);

      voucherEntry.Save();

      RefreshEntries();

      return voucherEntry;
    }


    internal void Close() {
      Assertion.Require(IsOpened, "Esta póliza ya está cerrada.");

      if (!_helper.IsValid) {
        FixedList<string> validationResult = _helper.GetValidationResult(true);

        Assertion.RequireFail("La póliza no puede enviarse al diario porque tiene datos inconsistentes.\n\n" +
                              EmpiriaString.ToString(validationResult));
      }

      _helper.EnsureAllEntriesAreValid();

      DateTime lastRecordingDate = this.RecordingDate;

      AuthorizedBy = Participant.Current;
      ClosedBy = Participant.Current;
      RecordingDate = DateTime.Today;
      IsOpened = false;
      Number = VoucherData.GetVoucherNumberFor(this);

      try {

        VoucherData.CloseVoucher(this);

        TrialBalanceCache.Invalidate(AccountingDate);

      } catch {
        AuthorizedBy = Participant.Empty;
        ClosedBy = Participant.Empty;
        RecordingDate = lastRecordingDate;
        IsOpened = true;
        Number = "No actualizada";
        throw;
      }

      RefreshEntries();
    }


    internal void Delete() {
      Assertion.Require(IsOpened, "Esta póliza no puede eliminarse porque ya está cerrada.");

      VoucherData.DeleteVoucher(this);
    }


    internal void DeleteEntry(VoucherEntry entry) {
      Assertion.Require(entry, nameof(entry));

      Assertion.Require(IsOpened, "No se puede eliminar el movimiento porque la póliza ya está cerrada.");
      Assertion.Require(Entries.Contains(entry), "El movimiento que se desea eliminar no pertenece a esta póliza");

      entry.Delete();

      RefreshEntries();
    }


    internal VoucherEntry GetCopyOfLastEntry() {
      Assertion.Require(Entries.Count > 0, "Esta póliza aún no tiene movimientos.");

      var list = new List<VoucherEntry>(Entries);

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

      IEnumerable<Currency> currencies = Entries.Select(x => x.Currency)
                                                .Distinct()
                                                .OrderBy(x => x.Code);

      foreach (var currency in currencies) {
        var debitsEntries = Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Debit &&
                                                 x.Currency.Equals(currency));
        var creditsEntries = Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Credit &&
                                                  x.Currency.Equals(currency));

        decimal totalDebits = debitsEntries.Sum(x => x.Debit);
        decimal totalCredits = creditsEntries.Sum(x => x.Credit);

        var total = new VoucherTotal(currency, totalDebits, totalCredits);

        totalsList.Add(total);
      }

      return totalsList.ToFixedList();
    }


    private void LoadFields(VoucherFields fields) {
      Ledger = Ledger.Parse(fields.LedgerUID);
      AccountingDate = fields.AccountingDate;
      RecordingDate = fields.RecordingDate;

      if (fields.ElaboratedById == 0) {
        ElaboratedBy = Participant.Current;
      } else {
        ElaboratedBy = Participant.Parse(fields.ElaboratedById);
      }

      Concept = EmpiriaString.TrimAll(fields.Concept).ToUpperInvariant();
      VoucherType = VoucherType.Parse(fields.VoucherTypeUID);
      TransactionType = TransactionType.Parse(fields.TransactionTypeUID);
      FunctionalArea = FunctionalArea.Parse(fields.FunctionalAreaId);
    }


    protected internal void Save() {
      if (Id == 0) {
        Id = VoucherData.NextVoucherId();
      }
      VoucherData.WriteVoucher(this);
    }


    internal void RefreshEntries() {
      if (!IsEmptyInstance) {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => VoucherData.GetVoucherEntries(this));

      } else {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => new FixedList<VoucherEntry>());
      }
    }


    internal void SendToSupervisor() {
      Assertion.Require(IsOpened, "La póliza no se puede enviar a supervisión porque no está abierta.");

      Assertion.Require(_helper.IsValid, "La póliza no puede enviarse a supervisión debido a que " +
                                         "tiene datos con inconsistencias o no está balanceada.");

      //Assertion.Require(this.ElaboratedBy.Equals(Participant.Current) || IsSupervisor(Participant.Current),
      //                                   "La póliza solo puede ser enviada a supervisión " +
      //                                   "por la persona que la elaboró o por alguien con el rol supervisor.");

      AuthorizedBy = _helper.AccountingManager;

      Save();
    }


    internal void Update(VoucherFields fields) {
      Assertion.Require(fields, nameof(fields));

      Ledger = Patcher.Patch(fields.LedgerUID, Ledger);
      AccountingDate = Patcher.Patch(fields.AccountingDate, AccountingDate);
      RecordingDate = Patcher.Patch(fields.RecordingDate, RecordingDate);
      ElaboratedBy = Patcher.Patch(fields.ElaboratedById, ElaboratedBy);
      Concept = Patcher.PatchClean(fields.Concept, Concept);
      // this.TransactionType = FieldPatcher.PatchField(fields.TransactionTypeUID, this.TransactionType);
      VoucherType = Patcher.Patch(fields.VoucherTypeUID, VoucherType);
      FunctionalArea = Patcher.Patch(fields.FunctionalAreaId, FunctionalArea);
    }


    internal void UpdateConcept(string newConcept) {
      Assertion.Require(newConcept, nameof(newConcept));

      Assertion.Require(_helper.Actions.ChangeConcept,
          "Para efectuar el cambio de concepto, la póliza debe estar cerrada " +
          "y su fecha de afectación debe estar dentro de un período contable abierto.");

      Concept = EmpiriaString.Clean(newConcept);

      VoucherData.UpdateVoucherConcept(this);
    }


    internal void UpdateEntry(VoucherEntry entry, VoucherEntryFields fields) {
      Assertion.Require(entry, nameof(entry));
      Assertion.Require(IsOpened, "No se puede actualizar el movimiento porque la póliza ya está cerrada.");
      Assertion.Require(Entries.Contains(entry), "El movimiento que se desea modificar no pertenece a esta póliza");

      entry.Update(fields);

      RefreshEntries();
    }


    internal void SaveAll() {
      Save();

      foreach (var entry in this.Entries) {
        entry.Save();
      }

      RefreshEntries();
    }

    #endregion Methods

  }  // class Voucher

}  // namespace Empiria.FinancialAccounting.Vouchers
