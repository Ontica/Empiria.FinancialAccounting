/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Empiria General Object                  *
*  Type     : DatasetType                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a dataset, which is a list of dataset file types.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Datasets.Data;

namespace Empiria.FinancialAccounting.Datasets {

  /// <summary>Describes a dataset, which is a list of dataset file types.</summary>
  public class DatasetType : GeneralObject {

    #region Constructors and parsers

    protected DatasetType() {
      // Required by Empiria Framework.
    }


    static public DatasetType Parse(int id) {
      return BaseObject.ParseId<DatasetType>(id);
    }


    static public DatasetType Parse(string uid) {
      return BaseObject.ParseKey<DatasetType>(uid);
    }


    static internal FixedList<DatasetType> GetList() {
      return BaseObject.GetList<DatasetType>()
                       .ToFixedList();
    }


    static internal DatasetType Empty => BaseObject.ParseEmpty<DatasetType>();


    #endregion Constructors and parsers

    #region Properties

    internal FixedList<InputDatasetType> DatasetTypes {
      get {
        return base.ExtendedDataField.GetFixedList<InputDatasetType>("inputDatasetTypes");
      }
    }

    #endregion Properties

    #region Methods

    internal FixedList<InputDataset> GetInputDatasetsList(DateTime date) {
      return DatasetData.GetInputDatasets(this, date);
    }


    internal InputDatasetType GetInputDatasetType(string uid) {
      var inputDatasetType = this.DatasetTypes.Find(x => x.UID == uid);

      Assertion.AssertObject(inputDatasetType,
                            $"There is not defined an input dataset type '{uid}'.");

      return inputDatasetType;
    }


    internal FixedList<InputDatasetType> MissingInputDatasetTypes(DateTime date) {
      return this.DatasetTypes;
    }

    #endregion Methods

  }  // class DatasetType

}  // namespace Empiria.FinancialAccounting.Datasets
