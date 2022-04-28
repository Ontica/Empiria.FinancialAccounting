/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria Object                          *
*  Type     : Dataset                                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a financial accounting dataset.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.Datasets.Adapters;
using Empiria.FinancialAccounting.Datasets.Data;

namespace Empiria.FinancialAccounting.Datasets {

  /// <summary>Describes a financial accounting dataset.</summary>
  public class Dataset : BaseObject {

    #region Constructors and parsers

    private Dataset() {
      // Required by Empiria Framework.
    }


    public Dataset(DatasetsCommand command, FileInfo fileInfo) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(fileInfo, "fileInfo");

      LoadData(command);
      LoadFileData(fileInfo);
    }


    static public Dataset Parse(string uid) {
      return BaseObject.ParseKey<Dataset>(uid);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_TIPO_DATASET")]
    public DatasetFamily DatasetFamily {
      get;
      private set;
    }

    [DataField("TIPO_ARCHIVO")]
    private string _fileType;

    public DatasetKind FileType {
      get {
        return this.DatasetFamily.GetDatasetKind(_fileType);
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

    private void LoadData(DatasetsCommand command) {
      this.DatasetFamily = DatasetFamily.Parse(command.DatasetFamilyUID);
      _fileType = command.DatasetKind;
      this.OperationDate = command.Date;
      this.ElaborationDate = DateTime.Today;
      this.ElaboratedBy = ExecutionServer.CurrentIdentity.User.AsContact();
    }


    private void LoadFileData(FileInfo fileInfo) {
      this.FileName = fileInfo.Name;
      this.FileSize = fileInfo.Length;
    }


    protected override void OnSave() {
      DatasetData.WriteDataset(this);
    }

    #endregion Methods

  }  // class Dataset

}  // namespace Empiria.FinancialAccounting.Datasets
