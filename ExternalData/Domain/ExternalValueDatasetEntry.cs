/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Information Holder                      *
*  Type     : ExternalValueDatasetEntry                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a dataset entry for external values using dynamic tabular data.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Holds a dataset entry for external values using dynamic tabular data.</summary>
  internal class ExternalValueDatasetEntry {

    public ExternalValueDatasetEntry(ExternalVariable variable) {
      Assertion.Require(variable, nameof(variable));

      this.Variable = variable;
    }


    public ExternalVariable Variable {
      get;
    }

  }  // class ExternalValueDatasetEntry

}  // namespace Empiria.FinancialAccounting.ExternalData
