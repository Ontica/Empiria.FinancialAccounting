/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Data Service                            *
*  Type     : ExternalValuesData                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for financial external values.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Data;

namespace Empiria.FinancialAccounting.ExternalData.Data {

  /// <summary>Data access layer for financial external values.</summary>
  static internal class ExternalValuesData {

    static internal ExternalValue GetValue(ExternalVariable variable, DateTime date) {
      var sql = "SELECT * FROM COF_VALORES_EXTERNOS " +
               $"WHERE ID_VARIABLE_EXTERNA = {variable.Id} " +
               $"AND FECHA_APLICACION = {CommonMethods.FormatSqlDbDate(date)} " +
               $"AND STATUS_VALOR_EXTERNO <> 'X'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject<ExternalValue>(op, ExternalValue.Empty);
    }

  }  // class ExternalValuesData

}  // namespace Empiria.FinancialAccounting.ExternalData.Data
