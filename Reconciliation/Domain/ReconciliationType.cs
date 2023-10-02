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

using Empiria.Storage;

using Empiria.FinancialAccounting.Datasets;

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Describes a reconciliation type.</summary>
  internal class ReconciliationType : DatasetFamily {

    #region Constructors and parsers

    protected ReconciliationType() {
      // Required by Empiria Framework.
    }


    static internal new ReconciliationType Parse(int id) {
      return BaseObject.ParseId<ReconciliationType>(id);
    }


    static public new ReconciliationType Parse(string uid) {
      return BaseObject.ParseKey<ReconciliationType>(uid);
    }


    static internal FixedList<ReconciliationType> GetList() {
      return BaseObject.GetList<ReconciliationType>()
                       .ToFixedList();
    }


    static internal ReconciliationType Empty => BaseObject.ParseEmpty<ReconciliationType>();


    #endregion Constructors and parsers

    #region Properties

    internal AccountsList AccountsList {
      get {
        return ExtendedDataField.Get<AccountsList>("accountsListId");
      }
    }


    public FixedList<ExportTo> ExportTo {
      get {
        return base.ExtendedDataField.GetFixedList<ExportTo>("exportTo");
      }
      private set {
        base.ExtendedDataField.Set("exportTo", value);
      }
    }

    #endregion Properties

  }  // class ReconciliationType

}  // namespace Empiria.FinancialAccounting.Reconciliation
