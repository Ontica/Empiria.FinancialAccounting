/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Object                          *
*  Type     : InputDataset                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial accounting reconciliation input dataset.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using Empiria.Contacts;

using Empiria.FinancialAccounting.Datasets.Adapters;
using Empiria.FinancialAccounting.Datasets.Data;
using Empiria.Json;
using Empiria.StateEnums;

namespace Empiria.FinancialAccounting.Datasets {

  /// <summary>Describes a financial accounting reconciliation input dataset.</summary>
  public class InputDataset : BaseObject {

    #region Constructors and parsers

    private InputDataset() {
      // Required by Empiria Framework.
    }

    public InputDataset(StoreInputDatasetCommand command, FileInfo fileInfo) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(fileInfo, "fileInfo");

      LoadData(command);
      LoadFileData(fileInfo);
    }

    static public InputDataset Parse(string uid) {
      return BaseObject.ParseKey<InputDataset>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_TIPO_DATASET")]
    public DatasetType DatasetType {
      get;
      private set;
    }

    [DataField("TIPO_ARCHIVO")]
    private string _fileType;

    public InputDatasetType FileType {
      get {
        return this.DatasetType.GetInputDatasetType(_fileType);
      }
    }


    [DataField("FECHA_OPERACION")]
    public DateTime OperationDate {
      get;
      private set;
    }


    [DataField("FECHA_ELABORACION")]
    public DateTime ElaborationDate {
      get;
      private set;
    }


    [DataField("ID_ELABORADO_POR")]
    public Contact ElaboratedBy {
      get;
      private set;
    }


    [DataField("DATASET_EXT_DATA")]
    internal JsonObject ExtData {
      get;
      private set;
    } = JsonObject.Empty;


    internal long FileSize {
      get {
        return this.ExtData.Get("file/length", 0);
      }
      set {
        this.ExtData.Set("file/length", value);
      }
    }


    internal string FileName {
      get {
        return this.ExtData.Get("file/name", string.Empty);
      }
      set {
        this.ExtData.Set("file/name", value);
      }
    }


    internal string FileUrl {
      get {
        return "http://server/sicofin/files/archivo.xlsx";
      }
    }


    [DataField("STATUS", Default = EntityStatus.Pending)]
    public EntityStatus Status {
      get;
      private set;
    } = EntityStatus.Pending;


    internal void Delete() {
      this.Status = EntityStatus.Deleted;
    }

    #endregion Properties

    #region Methods

    private void LoadData(StoreInputDatasetCommand command) {
      this.DatasetType = DatasetType.Parse(command.ReconciliationTypeUID);
      _fileType = command.DatasetType;
      this.OperationDate = command.Date;
      this.ElaborationDate = DateTime.Today;
      this.ElaboratedBy = ExecutionServer.CurrentIdentity.User.AsContact();
    }


    private void LoadFileData(FileInfo fileInfo) {
      this.FileName = fileInfo.Name;
      this.FileSize = fileInfo.Length;
    }


    protected override void OnSave() {
      DatasetData.WriteInputDataset(this);
    }

    #endregion Methods

  }  // class InputDataset

}  // namespace Empiria.FinancialAccounting.Datasets
