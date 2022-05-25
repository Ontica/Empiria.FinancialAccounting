/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Service                            *
*  Type     : ExternalValuesData                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for financial concepts coming from external sources.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts.Data {

  /// <summary>Data access layer for financial concepts coming from external sources.</summary>
  static internal class ExternalValuesData {

    static internal ExternalValue GetValue(string externalVariableCode, DateTime date) {
      var sql = "SELECT * FROM COF_CONCEPTOS_VALORES " +
               $"WHERE CLAVE_VARIABLE = '{externalVariableCode}' " +
               $"AND FECHA_APLICACION = {CommonMethods.FormatSqlDbDate(date)}";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetObject<ExternalValue>(dataOperation, ExternalValue.Empty);
    }

  }  // class ExternalValuesData

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Data
