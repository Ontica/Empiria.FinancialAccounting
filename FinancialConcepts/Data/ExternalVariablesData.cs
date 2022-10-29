﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Service                            *
*  Type     : ExternalVariablesData                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for financial concepts variables coming from external sources.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts.Data {

  /// <summary>Data access layer for financial concepts variables coming from external sources.</summary>
  static internal class ExternalVariablesData {

    static internal ExternalValue GetValue(ExternalVariable variable, DateTime date) {
      var sql = "SELECT * FROM COF_VALORES_EXTERNOS " +
               $"WHERE ID_VARIABLE_EXTERNA = {variable.Id} " +
               $"AND FECHA_APLICACION = {CommonMethods.FormatSqlDbDate(date)} " +
               $"AND STATUS_VALOR_EXTERNO <> 'X'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject<ExternalValue>(op, ExternalValue.Empty);
    }


    static internal FixedList<ExternalVariable> GetExternalVariables(ExternalVariablesSet set) {
      var sql = "SELECT * FROM COF_VARIABLES_EXTERNAS " +
               $"WHERE ID_CONJUNTO_BASE = {set.Id} " +
               $"AND STATUS_VARIABLE_EXTERNA <> 'X' " +
               $"ORDER BY POSICION, CLAVE_VARIABLE";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<ExternalVariable>(op);
    }

  }  // class ExternalVariablesData

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Data
