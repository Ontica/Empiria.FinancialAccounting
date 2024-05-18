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

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Holds a dataset entry for external values using dynamic tabular data.</summary>
  internal class ExternalValueDatasetEntry {

    #region Constructors and parsers

    public ExternalValueDatasetEntry(ExternalValue value) :
                                        this(value.ExternalVariable, value.GetDynamicFields()) {
      // no-op
    }


    public ExternalValueDatasetEntry(ExternalVariable variable) : this(variable, new DynamicFields()) {
      // no-op
    }

    public ExternalValueDatasetEntry(ExternalVariable variable, DynamicFields values) {
      Assertion.Require(variable, nameof(variable));
      Assertion.Require(values, nameof(values));

      this.Variable = variable;
      this.Values = values;
    }

    #endregion Constructors and parsers

    #region Properties

    public ExternalVariable Variable {
      get;
    }


    public DynamicFields Values {
      get;
    }

    #endregion Properties

  }  // class ExternalValueDatasetEntry

}  // namespace Empiria.FinancialAccounting.ExternalData
