﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Service provider                        *
*  Type     : ExternalValuesReader                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to update and retrieve financial external values.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Office;
using Empiria.Storage;

using Empiria.FinancialAccounting.Datasets;

using Empiria.FinancialAccounting.ExternalData.Adapters;

namespace Empiria.FinancialAccounting.ExternalData {

  internal class ExternalValuesReader {

    private readonly Dataset _dataset;

    public ExternalValuesReader(Dataset dataset) {
      Assertion.Require(dataset, nameof(dataset));

      _dataset = dataset;

      DataColumns = ((ExternalVariablesSet) dataset.DatasetFamily).DataColumns;
      FileTemplate = FileTemplateConfig.Parse(dataset.DatasetKind.TemplateId);
    }

    public FixedList<DataTableColumn> DataColumns {
      get;
    }


    public FileTemplateConfig FileTemplate {
      get;
    }



    internal FixedList<ExternalValueInputDto> GetEntries() {
      Spreadsheet spreadsheet = OpenSpreadsheet();

      var entries = ReadEntries(spreadsheet);

      Assertion.Require(entries.Count > 0, "El archivo no contiene ningún valor externo.");

      return entries;
    }


    #region Private methods

    private Spreadsheet OpenSpreadsheet() {
      switch (_dataset.DatasetKind.FileType) {

        case FileType.Csv:
          return Spreadsheet.CreateFromCSVFile(_dataset.FullPath);

        case FileType.Excel:
          return Spreadsheet.Open(_dataset.FullPath);

        default:
          throw Assertion.EnsureNoReachThisCode(
            $"Unhandled dataset file type '{_dataset.DatasetKind.FileType}'."
          );
      }
    }


    private FixedList<ExternalValueInputDto> ReadEntries(Spreadsheet spreadsheet) {
      var entriesList = new List<ExternalValueInputDto>(256);

      var dynamicColumns = this.DataColumns.FindAll(column => column.Type == "decimal");

      int rowIndex = FileTemplate.FirstRowIndex;

      while (spreadsheet.HasValue($"A{rowIndex}")) {
        ExternalValueInputDto entry = TryReadEntry(spreadsheet, dynamicColumns, rowIndex);

        if (entry != null) {
          entriesList.Add(entry);
        }

        rowIndex++;
      }

      EmpiriaLog.Info(
        $"Se leyeron {entriesList.Count} registros del archivo de variables externas {_dataset.FullPath}."
      );

      return entriesList.ToFixedList();
    }


    private ExternalValueInputDto TryReadEntry(Spreadsheet spreadsheet,
                                               FixedList<DataTableColumn> dynamicColumns,
                                               int rowIndex) {
      var spreadsheetRow = new SpreadsheetRowReader(spreadsheet, rowIndex);

      DynamicFields fields = spreadsheetRow.GetDynamicFields(dynamicColumns);

      if (fields.IsEmptyInstance) {
        return null;
      }

      var dto = new ExternalValueInputDto {
        VariableCode = spreadsheetRow.GetVariableCode(),
        Position = rowIndex,
        Dataset = _dataset,
        ApplicationDate = _dataset.OperationDate,
        UpdatedDate = _dataset.UpdatedTime,
        UpdatedBy = _dataset.UploadedBy,
        Status = _dataset.Status,
      };

      dto.VariableUID = dto.GetExternalVariable().UID;

      foreach (var fieldName in fields.GetDynamicMemberNames()) {
        dto.SetTotalField(fieldName, fields.GetTotalField(fieldName));
      }

      return dto;
    }

    #endregion Private methods

  }  // internal class ExternalValuesReader

}  // namespace Empiria.FinancialAccounting.ExternalData
