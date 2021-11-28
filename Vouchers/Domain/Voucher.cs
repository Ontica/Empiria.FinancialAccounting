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
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Represents an accounting voucher.</summary>
  public class Voucher : BaseObject {

    private Lazy<FixedList<VoucherEntry>> _entries;

    #region Constructors and parsers

    private Voucher() {
      // Required by Empiria Framework.
    }

    internal Voucher(VoucherFields fields) {
      Assertion.AssertObject(fields, "fields");

      this.LoadFields(fields);
      this.Number = "No actualizada";
      this.RecordingDate = DateTime.Today;
      this.IsOpened = true;
      this.ElaboratedBy = Participant.Current;
    }


    static public Voucher Parse(int id) {
      return BaseObject.ParseId<Voucher>(id);
    }

    static public Voucher Parse(string uid) {
      return BaseObject.ParseKey<Voucher>(uid);
    }

    static public FixedList<Voucher> GetList(string filter, string sort, int pageSize) {
      return VoucherData.GetVouchers(filter, sort, pageSize);
    }

    static public Voucher Empty => BaseObject.ParseEmpty<Voucher>();

    protected override void OnInitialize() {
      base.OnLoad();
      RefreshEntries();
    }

    #endregion Constructors and parsers

    #region Public properties


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

    #endregion Public properties

    #region Methods

    internal VoucherEntry AppendEntry(VoucherEntryFields fields) {
      Assertion.AssertObject(fields, "fields");
      Assertion.Assert(this.IsOpened, "No se puede agregar el movimiento porque la póliza ya está cerrada.");

      fields.EnsureValidFor(this);

      var voucherEntry = new VoucherEntry(fields);

      voucherEntry.Save();

      this.RefreshEntries();

      return voucherEntry;
    }


    internal bool CanBeClosedBy(Participant participant) {
      if (this.Ledger.IsAccountingDateOpened(this.AccountingDate)) {
        return true;
      }
      return false;
    }


    internal void Close() {
      Assertion.Assert(this.IsOpened, "Esta póliza ya está cerrada.");

      if (!this.IsValid()) {
        var msg = "La póliza no puede enviarse al diario porque tiene datos inconsistentes.\n\n";

        FixedList<string> validationResult = this.ValidationResult();

        foreach (var error in validationResult) {
          msg += error + "\n";
        }
        Assertion.AssertFail(msg);
      }

      if (!this.CanBeClosedBy(Participant.Current)) {
        Assertion.AssertFail("La póliza no puede enviarse directamente al diario porque tiene fecha valor.");
      }

      DateTime lastRecordingDate = this.RecordingDate;

      this.AuthorizedBy = Participant.Current;
      this.ClosedBy = Participant.Current;
      this.RecordingDate = DateTime.Today;
      this.IsOpened = false;
      this.Number = VoucherData.GetVoucherNumberFor(this);

      try {
        VoucherData.CloseVoucher(this);
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
      Assertion.Assert(this.IsOpened, "Esta póliza no puede eliminarse porque ya está cerrada.");

      VoucherData.DeleteVoucher(this);
    }


    internal void DeleteEntry(VoucherEntry entry) {
      Assertion.AssertObject(entry, "entry");
      Assertion.Assert(this.IsOpened, "No se puede eliminar el movimiento porque la póliza ya está cerrada.");
      Assertion.Assert(this.Entries.Contains(entry), "El movimiento que se desea eliminar no pertenece a esta póliza");

      entry.Delete();

      this.RefreshEntries();
    }


    internal VoucherEntry GetCopyOfLastEntry() {
      Assertion.Assert(this.Entries.Count > 0, "Esta póliza aún no tiene movimientos.");

      var list = new List<VoucherEntry>(this.Entries);

      VoucherEntry lastEntry = list.OrderByDescending(x => x.Id)
                                   .First();

      return lastEntry.CreateCopy();
    }


    internal VoucherEntry GetEntry(int voucherEntryId) {
      var entry = Entries.Find(x => x.Id == voucherEntryId);

      Assertion.AssertObject(entry, $"La póliza no tiene registrado un movimiento con id {voucherEntryId}");

      return entry;
    }


    internal FixedList<VoucherTotal> GetTotals() {
      var totalsList = new List<VoucherTotal>();

      IEnumerable<Currency> currencies = this.Entries.Select(x => x.Currency)
                                                     .Distinct()
                                                     .OrderBy(x => x.Code);

      foreach (var currency in currencies) {
        var debitsEntries = this.Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Debit && x.Currency.Equals(currency));
        var creditsEntries = this.Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Credit && x.Currency.Equals(currency));

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

      this.RefreshEntries();

      var validator = new VoucherValidator(this.Ledger, this.AccountingDate);

      return validator.Validate(this.Entries).Count == 0;
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


    protected override void OnSave() {
      VoucherData.WriteVoucher(this);
    }

    private void RefreshEntries() {
      if (!this.IsEmptyInstance) {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => VoucherData.GetVoucherEntries(this));

      } else {
        _entries = new Lazy<FixedList<VoucherEntry>>(() => new FixedList<VoucherEntry>());
      }
    }


    internal FixedList<SubledgerAccount> SearchSubledgerAccountsForEdition(LedgerAccount account, string keywords) {
      Assertion.Assert(this.IsOpened, "No hay cuentas auxiliares para edición porque la póliza ya está cerrada.");

      Assertion.Assert(account.Ledger.Equals(this.Ledger), "Account do not belong to voucher ledger.");

      Assertion.Assert(account.Role == AccountRole.Control || account.Role == AccountRole.Sectorizada,
                       "The account role is not control. There are not subledger accounts");

      return VoucherData.SearchSubledgerAccountsForVoucherEdition(this, keywords);
    }


    internal void Update(VoucherFields fields) {
      Assertion.AssertObject(fields, "fields");

      this.Ledger = PatchField(fields.LedgerUID, this.Ledger);
      this.AccountingDate = PatchField(fields.AccountingDate, this.AccountingDate);
      this.RecordingDate = PatchField(fields.RecordingDate, this.RecordingDate);
      this.ElaboratedBy = PatchField(fields.ElaboratedByUID, this.ElaboratedBy);
      this.Concept = PatchField(EmpiriaString.TrimAll(fields.Concept), this.Concept);
      this.TransactionType = PatchField(fields.TransactionTypeUID, this.TransactionType);
      this.VoucherType = PatchField(fields.VoucherTypeUID, this.VoucherType);
      this.FunctionalArea = PatchField(fields.FunctionalAreaId, this.FunctionalArea);
    }


    internal void UpdateEntry(VoucherEntry entry, VoucherEntryFields fields) {
      Assertion.AssertObject(entry, "entry");
      Assertion.Assert(this.IsOpened, "No se puede actualizar el movimiento porque la póliza ya está cerrada.");
      Assertion.Assert(this.Entries.Contains(entry), "El movimiento que se desea modificar no pertenece a esta póliza");

      entry.Update(fields);

      this.RefreshEntries();
    }


    internal FixedList<string> ValidationResult() {
      this.RefreshEntries();

      var validator = new VoucherValidator(this.Ledger, this.AccountingDate);

      return validator.Validate(this.Entries);
    }


    #endregion Methods

  }  // class Voucher

}  // namespace Empiria.FinancialAccounting.Vouchers
