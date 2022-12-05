/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Providers                               *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Service provider                        *
*  Type     : ExternalValuesProvider                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides external variables values.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Collections;

using Empiria.FinancialAccounting.ExternalData;

namespace Empiria.FinancialAccounting.FinancialReports.Providers {

  internal class ExternalValuesProvider {

    private readonly FixedList<EmpiriaHashTable<ExternalValue>> _externalValuesDatasets;

    public ExternalValuesProvider(FixedList<ExternalVariablesSet> externalVariablesSets,
                                  DateTime valuesDate) {

      var list = new List<EmpiriaHashTable<ExternalValue>>();

      foreach (var variablesSet in externalVariablesSets) {
        var valuesSet = new ExternalValuesDataSet(variablesSet, valuesDate);

        list.Add(valuesSet.GetLoadedValuesAsHashTable());
      }

      _externalValuesDatasets = list.ToFixedList();
    }


    internal bool ContainsVariable(string externalVariableCode) {
      foreach (var dataset in _externalValuesDatasets) {
        if (dataset.ContainsKey(externalVariableCode)) {
          return true;
        }
      }

      return false;
    }


    internal FixedList<ExternalValue> GetValues(string externalVariableCode) {
      var values = new List<ExternalValue>();

      foreach (var dataset in _externalValuesDatasets) {
        if (dataset.ContainsKey(externalVariableCode)) {
          values.Add(dataset[externalVariableCode]);
        }
      }

      return values.ToFixedList();
    }

  }  // class ExternalValuesProvider

}  // namespace Empiria.FinancialAccounting.FinancialReports
