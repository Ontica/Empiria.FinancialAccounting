/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Information Holder                      *
*  Type     : ExternalValuesDataSet                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds dynamic tabular data for a set of financial external values.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Holds dynamic tabular data for a set of financial external values.</summary>
  internal class ExternalValuesDataSet {

    #region Constructors and parsers

    internal ExternalValuesDataSet(ExternalVariablesSet variablesSet, DateTime date) {
      Assertion.Require(variablesSet, nameof(variablesSet));

      VariablesSet = variablesSet;
      Date = date;
    }

    #endregion Constructors and parsers

    #region Properties

    public DateTime Date {
      get;
    }


    public ExternalVariablesSet VariablesSet {
      get;
    }

    #endregion Properties

    #region Methods

    internal FixedList<ExternalValueDatasetEntry> GetAllValues() {
      var list = new List<ExternalValueDatasetEntry>(VariablesSet.ExternalVariables.Count);

      foreach (var variable in VariablesSet.ExternalVariables) {
        var fields = new DynamicFields();
        fields.SetTotalField("domesticCurrencyTotal", decimal.Parse(variable.Id.ToString()));
        if (variable.Id % 3 == 0) {
          fields.SetTotalField("foreignCurrencyTotal", decimal.Parse((variable.Id * 2).ToString()));
        }

        var entry = new ExternalValueDatasetEntry(variable, fields);

        list.Add(entry);
      }

      return list.ToFixedList();
    }


    internal FixedList<ExternalValueDatasetEntry> GetLoadedValues() {
      throw new NotImplementedException();
    }

    #endregion Methods

  } // class ExternalValuesDataSet

}  // namespace Empiria.FinancialAccounting.ExternalData
