/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Empiria General Object                  *
*  Type     : ReconciliationType                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a reconciliation type.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Datasets;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Describes a reconciliation type.</summary>
  internal class ReconciliationType : GeneralObject {

    #region Constructors and parsers

    protected ReconciliationType() {
      // Required by Empiria Framework.
    }


    static internal ReconciliationType Parse(int id) {
      return BaseObject.ParseId<ReconciliationType>(id);
    }


    static public ReconciliationType Parse(string uid) {
      return BaseObject.ParseKey<ReconciliationType>(uid);
    }


    static internal FixedList<ReconciliationType> GetList() {
      return BaseObject.GetList<ReconciliationType>()
                       .ToFixedList();
    }


    static internal ReconciliationType Empty => BaseObject.ParseEmpty<ReconciliationType>();


    #endregion Constructors and parsers

    #region Properties

    internal FixedList<DatasetKind> DatasetKinds {
      get {
        return base.ExtendedDataField.GetFixedList<DatasetKind>("datasetKinds");
      }
    }

    #endregion Properties

    #region Methods

    internal FixedList<Dataset> GetDatasetsList(DateTime date) {
      // return DatasetType.GetInputDatasets(this, date);

      throw new NotImplementedException();
    }


    internal DatasetKind GetDatasetKind(string uid) {
      var datasetKind = this.DatasetKinds.Find(x => x.UID == uid);

      Assertion.AssertObject(datasetKind,
                            $"There is not defined an dataset kind with uid '{uid}'.");

      return datasetKind;
    }


    internal FixedList<DatasetKind> MissingDatasetKinds(DateTime date) {
      return this.DatasetKinds;
    }

    #endregion Methods

  }  // class ReconciliationType

}  // namespace Empiria.FinancialAccounting.Reconciliation
