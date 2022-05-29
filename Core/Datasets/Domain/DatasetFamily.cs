/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria General Object                  *
*  Type     : DatasetFamily                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a dataset family, which is a list of dataset file types with their rules.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Datasets.Data;

namespace Empiria.FinancialAccounting.Datasets {

  /// <summary>Describes a dataset family, which is a list of dataset file types with their rules.</summary>
  public class DatasetFamily : GeneralObject {

    #region Constructors and parsers

    protected DatasetFamily() {
      // Required by Empiria Framework.
    }


    static public DatasetFamily Parse(int id) {
      return BaseObject.ParseId<DatasetFamily>(id);
    }


    static public DatasetFamily Parse(string uid) {
      return BaseObject.ParseKey<DatasetFamily>(uid);
    }


    static internal FixedList<DatasetFamily> GetList() {
      return BaseObject.GetList<DatasetFamily>()
                       .ToFixedList();
    }


    static internal DatasetFamily Empty => BaseObject.ParseEmpty<DatasetFamily>();


    #endregion Constructors and parsers

    #region Properties

    public FixedList<DatasetKind> DatasetKinds {
      get {
        return base.ExtendedDataField.GetFixedList<DatasetKind>("datasetKinds");
      }
    }

    #endregion Properties

    #region Methods

    public FixedList<Dataset> GetDatasetsList(DateTime date) {
      return DatasetData.GetDatasets(this, date);
    }


    public DatasetKind GetDatasetKind(string uid) {
      var datasetKind = this.DatasetKinds.Find(x => x.UID == uid);

      Assertion.Require(datasetKind,
                        $"There is not defined a dataset kind with uid '{uid}'.");

      return datasetKind;
    }


    public FixedList<DatasetKind> MissingDatasetKinds(DateTime date) {
      var loadedDataSets = GetDatasetsList(date);

      FixedList<DatasetKind> filtered =
         this.DatasetKinds.FindAll(x => !loadedDataSets.Exists(loaded => loaded.DatasetKind.UID == x.UID));

      return filtered;
    }

    #endregion Methods

  }  // class DatasetFamily

}  // namespace Empiria.FinancialAccounting.Datasets
