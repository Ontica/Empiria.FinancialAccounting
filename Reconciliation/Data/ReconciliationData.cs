/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Data Service                            *
*  Type     : ReconciliationData                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read and write methods for reconciliation services.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Reconciliation.Data {

  /// <summary>Provides data read and write methods for reconciliation services.</summary>
  static internal class ReconciliationData {

    static internal FixedList<InputDataset> GetInputDatasets(ReconciliationType reconciliationType,
                                                             DateTime date) {
      var sql = "SELECT * " +
                "FROM COF_CONCILIACION_DATASETS " +
               $"WHERE ID_TIPO_CONCILIACION = {reconciliationType.Id} AND " +
               $"FECHA_CONCILIACION = '{CommonMethods.FormatSqlDate(date)}' AND " +
               $"STATUS <> 'X'";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<InputDataset>(operation);
    }


    static internal void WriteInputDataset(InputDataset o) {
      var op = DataOperation.Parse("write_cof_conciliacion_dataset",
                        o.Id, o.UID, o.ReconciliationType.Id,
                        o.DatasetType.UID, o.Name, o.ReconciliationDate, o.ElaborationDate,
                        o.ExtData.ToString(), o.ElaboratedBy.Id,
                        (char) o.Status);

      DataWriter.Execute(op);
    }

  } // class ReconciliationData

} // namespace Empiria.FinancialAccounting.Reconciliation.Data
