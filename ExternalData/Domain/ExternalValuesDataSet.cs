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

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Holds dynamic tabular data for a set of financial external values.</summary>
  internal class ExternalValuesDataSet {


    static internal ExternalValuesDataSet Parse(ExternalVariablesSet variablesSet, DateTime date) {
      return new ExternalValuesDataSet {
        Set = variablesSet
      };
    }


    public ExternalVariablesSet Set {
      get;
      private set;
    }

  } // class ExternalValuesDataSet

}  // namespace Empiria.FinancialAccounting.ExternalData
