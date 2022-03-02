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

using Empiria.FinancialAccounting.Reconciliation.Data;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Describes a reconciliation type.</summary>
  internal class ReconciliationType : GeneralObject {

    #region Constructors and parsers

    protected ReconciliationType() {
      // Required by Empiria Framework.
    }

    static public ReconciliationType Parse(string uid) {
      return BaseObject.ParseKey<ReconciliationType>(uid);
    }


    static internal FixedList<ReconciliationType> GetList() {
      return BaseObject.GetList<ReconciliationType>()
                       .ToFixedList();
    }


    #endregion Constructors and parsers

    #region Properties

    public FixedList<InputDatasetType> DatasetTypes {
      get {
        return base.ExtendedDataField.GetFixedList<InputDatasetType>("inputDatasetTypes");
      }
    }

    #endregion Properties

    #region Methods

    internal FixedList<InputDataset> GetInputDatasetsList(DateTime date) {
      return ReconciliationData.GetInputDatasets(this, date);
    }

    internal FixedList<InputDatasetType> MissingInputDatasetTypes(DateTime date) {
      return this.DatasetTypes;
    }

    #endregion Methods

  }  // class ReconciliationType

}  // namespace Empiria.FinancialAccounting.Reconciliation
