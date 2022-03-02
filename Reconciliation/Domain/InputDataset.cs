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

namespace Empiria.FinancialAccounting.Reconciliation {

  /// <summary>Describes a financial accounting reconciliation input dataset.</summary>
  internal class InputDataset : BaseObject {

    #region Constructors and parsers

    private InputDataset() {
      // Required by Empiria Framework.
    }

    static public InputDataset Parse(string uid) {
      return BaseObject.ParseKey<InputDataset>(uid);
    }

    #endregion Constructors and parsers

  }  // class InputDataset

}  // namespace Empiria.FinancialAccounting.Reconciliation
