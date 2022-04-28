/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Dataset Services                           Component : Data Layer                              *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : DatasetData                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read and write methods for datasets.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Datasets.Data {

  /// <summary>Provides data read and write methods for datasets.</summary>
  static internal class DatasetData {

    static internal FixedList<Dataset> GetDatasets(DatasetFamily datasetFamily,
                                                   DateTime date) {
      var sql = "SELECT * " +
                "FROM COF_DATASETS " +
               $"WHERE ID_TIPO_DATASET = {datasetFamily.Id} AND STATUS <> 'X' " +
               $"FECHA_OPERACION = '{CommonMethods.FormatSqlDate(date)}' AND " +
               $"ORDER BY ID_DATASET";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Dataset>(operation);
    }


    static internal void WriteDataset(Dataset o) {
      var op = DataOperation.Parse("write_cof_dataset",
                        o.Id, o.UID, o.DatasetFamily.Id,
                        o.FileType.UID, o.OperationDate, o.ElaborationDate,
                        o.ExtData.ToString(), o.ElaboratedBy.Id,
                        (char) o.Status);

      DataWriter.Execute(op);
    }

  } // class DatasetData

} // namespace Empiria.FinancialAccounting.Datasets.Data
