/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Empiria Data Object                     *
*  Type     : ReconciliationEntry                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Domain object that holds data for a reconciliation dataset entry.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

using Empiria.FinancialAccounting.Datasets;

using Empiria.FinancialAccounting.Reconciliation.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Data;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Domain object that holds data for a reconciliation dataset entry.</summary>
  internal class ReconciliationEntry : BaseObject {

    private ReconciliationEntry() {
      // Required by Empiria Framework
    }


    internal protected ReconciliationEntry(Dataset dataset, ReconciliationEntryDto dto) {
      Assertion.AssertObject(dataset, "dataset");
      Assertion.AssertObject(dto, "dto");

      this.Dataset = dataset;

      Load(dto);
    }


    #region Properties

    [DataField("ID_DATASET")]
    public Dataset Dataset {
      get; private set;
    }


    [DataField("LLAVE_DATO_CONCILIACION")]
    public string UniqueKey {
      get; private set;
    } = string.Empty;


    [DataField("NUMERO_MAYOR")]
    public string LedgerNumber {
      get; private set;
    } = string.Empty;


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; private set;
    } = string.Empty;


    [DataField("CLAVE_MONEDA")]
    public string CurrencyCode {
      get; private set;
    } = string.Empty;


    [DataField("CLAVE_SECTOR")]
    public string SectorCode {
      get; private set;
    } = string.Empty;


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubledgerAccountNumber {
      get; private set;
    } = string.Empty;


    [DataField("NUMERO_VOLANTE")]
    public string TransactionSlip {
      get; private set;
    } = string.Empty;


    [DataField("DATO_CONCILIACION_EXT_DATA")]
    public JsonObject ExtData {
      get; private set;
    } = new JsonObject();


    [DataField("SALDO_INICIAL_CONCILIACION")]
    public decimal InitialBalance {
      get; private set;
    }


    [DataField("CARGOS_CONCILIACION")]
    public decimal Debits {
      get; private set;
    }


    [DataField("ABONOS_CONCILIACION")]
    public decimal Credits {
      get; private set;
    }


    [DataField("SALDO_ACTUAL_CONCILIACION")]
    public decimal EndBalance {
      get; private set;
    }


    [DataField("POSICION")]
    public int Position {
      get; private set;
    }

    #endregion Properties

    #region Methods

    private void Load(ReconciliationEntryDto dto) {
      this.UniqueKey = dto.UniqueKey;
      this.LedgerNumber = dto.LedgerNumber;
      this.AccountNumber = dto.AccountNumber;
      this.SubledgerAccountNumber = dto.SubledgerAccountNumber;
      this.CurrencyCode = dto.CurrencyCode;
      this.SectorCode = dto.SectorCode;
      this.TransactionSlip = dto.TransactionSlip;
      this.ExtData = dto.ExtData;
      this.InitialBalance = dto.InitialBalance;
      this.Debits = dto.Debits;
      this.Credits = dto.Credits;
      this.EndBalance = dto.EndBalance;
      this.Position = dto.Position;
    }

    protected override void OnBeforeSave() {
      Assertion.Assert(this.IsNew,
        "El método Save() sólo puede invocarse sobre nuevas entradas de conciliación."
      );
    }

    protected override void OnSave() {
      ReconciliationData.AppendEntry(this);
    }

    #endregion Methods

  }  // class ReconciliationEntry

}  // namespace Empiria.FinancialAccounting.Reconciliation
