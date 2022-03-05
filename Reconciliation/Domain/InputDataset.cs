/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Empiria Object                          *
*  Type     : InputDataset                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial accounting reconciliation input dataset.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using Empiria.Contacts;
using Empiria.FinancialAccounting.Reconciliation.Adapters;
using Empiria.FinancialAccounting.Reconciliation.Data;
using Empiria.Json;
using Empiria.StateEnums;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Describes a financial accounting reconciliation input dataset.</summary>
  internal class InputDataset : BaseObject {

    #region Constructors and parsers

    private InputDataset() {
      // Required by Empiria Framework.
    }

    internal InputDataset(StoreInputDatasetCommand command, FileInfo fileInfo) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(fileInfo, "fileInfo");

      LoadData(command);
    }

    static public InputDataset Parse(string uid) {
      return BaseObject.ParseKey<InputDataset>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_TIPO_CONCILIACION")]
    public ReconciliationType ReconciliationType {
      get;
      private set;
    }


    [DataField("TIPO_DATASET")]
    private string _datasetType;


    public InputDatasetType DatasetType {
      get {
        return this.ReconciliationType.GetInputDatasetType(_datasetType);
      }
    }


    [DataField("NOMBRE_DATASET")]
    public string Name {
      get;
      private set;
    }


    [DataField("FECHA_CONCILIACION")]
    public DateTime ReconciliationDate {
      get;
      private set;
    }


    [DataField("FECHA_ELABORACION")]
    public DateTime ElaborationDate {
      get;
      private set;
    }


    [DataField("ELABORADO_POR_ID")]
    public Contact ElaboratedBy {
      get;
      private set;
    }


    [DataField("DATASET_EXT_DATA")]
    internal JsonObject ExtData {
      get;
      private set;
    } = JsonObject.Empty;


    internal int FileSize {
      get {
        return 0;
      }
    }


    [DataField("STATUS", Default = EntityStatus.Pending)]
    public EntityStatus Status {
      get;
      private set;
    } = EntityStatus.Pending;


    internal void Delete() {
      this.Status = EntityStatus.Deleted;

      this.Save();
    }

    #endregion Properties

    #region Methods

    private void LoadData(StoreInputDatasetCommand command) {
      this.ReconciliationType = ReconciliationType.Parse(command.ReconciliationTypeUID);
      _datasetType = command.DatasetType;
      this.ReconciliationDate = command.Date;
      this.ElaborationDate = DateTime.Today;
      this.ElaboratedBy = ExecutionServer.CurrentIdentity.User.AsContact();
      this.Name = $"{ReconciliationType.Name}. Fecha: {ReconciliationDate.ToString("yyyy/MM/dd")}. {DatasetType}.";
    }


    protected override void OnSave() {
      ReconciliationData.WriteInputDataset(this);
    }

    #endregion Methods

  }  // class InputDataset

}  // namespace Empiria.FinancialAccounting.Reconciliation
