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

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Describes a reconciliation type.</summary>
  internal class ReconciliationType : GeneralObject {

    private ReconciliationType() {
      // Required by Empiria Framework.
    }

    static public ReconciliationType Parse(string uid) {
      return BaseObject.ParseKey<ReconciliationType>(uid);
    }


    static internal FixedList<ReconciliationType> GetList() {
      return BaseObject.GetList<ReconciliationType>()
                       .ToFixedList();
    }

  }  // class ReconciliationType

}  // namespace Empiria.FinancialAccounting.Reconciliation
