/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Data Service                            *
*  Type     : ExternalVariablesData                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for financial external variables definition data.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Collections.Generic;

using Empiria.Data;

namespace Empiria.FinancialAccounting.ExternalData.Data {

  /// <summary>Data access layer for financial external variables definition data.</summary>
  static internal class ExternalVariablesData {

    static internal List<ExternalVariable> GetExternalVariables(ExternalVariablesSet set) {
      var sql = "SELECT * FROM COF_VARIABLES_EXTERNAS " +
               $"WHERE ID_CONJUNTO_BASE = {set.Id} " +
               $"AND STATUS_VARIABLE_EXTERNA <> 'X' " +
               $"ORDER BY POSICION, CLAVE_VARIABLE";

      var op = DataOperation.Parse(sql);

      return DataReader.GetList<ExternalVariable>(op);
    }


    static internal void Write(ExternalVariable o) {
      var op = DataOperation.Parse("write_cof_variable_externa",
          o.Id, o.UID, o.Set.Id, o.Code, o.Name,
          o.Notes, o.ExtData.ToString(), o.Position,
          o.StartDate, o.EndDate,
          (char) o.Status, o.UpdatedBy.Id);

      DataWriter.Execute(op);
    }

  }  // class ExternalVariablesData

}  // namespace Empiria.FinancialAccounting.ExternalData.Data
