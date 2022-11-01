/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Interface adapters                      *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Mapper class                            *
*  Type     : ExternalValuesDataSetMapper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial external values.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.ExternalData.Adapters {

  /// <summary>Mapping methods for financial external values.</summary>
  static internal class ExternalValuesDataSetMapper {

    #region Mappers

    static internal ExternalValuesDto Map(ExternalValuesQuery query, ExternalValuesDataSet dataset) {
      FixedList<ExternalValueDatasetEntry> entries = GetEntries(query.DatasetMode, dataset);

      return new ExternalValuesDto {
        Query = query,
        Columns = dataset.VariablesSet.DataColumns,
        Entries = Map(entries),
      };
    }


    static private FixedList<ExternalValuesEntryDto> Map(FixedList<ExternalValueDatasetEntry> entries) {
      return entries.Select(e => Map(e))
                    .ToFixedList();
    }


    static private ExternalValuesEntryDto Map(ExternalValueDatasetEntry entry) {
      var dto = new ExternalValuesEntryDto {
        VariableCode = entry.Variable.Code,
        VariableName = entry.Variable.Name,
      };

      SetTotalsFields(dto, entry);

      return dto;
    }

    #endregion Mappers

    #region Helpers

    static private void SetTotalsFields(ExternalValuesEntryDto o, ExternalValueDatasetEntry entry) {
      var dynamicFieldNames = entry.Values.GetDynamicMemberNames();

      foreach (string fieldName in dynamicFieldNames) {
        o.SetTotalField(fieldName, entry.Values.GetTotalField(fieldName));
      }
    }


    static private FixedList<ExternalValueDatasetEntry> GetEntries(ExternalValuesDataSetMode datasetMode,
                                                               ExternalValuesDataSet dataset) {
      switch (datasetMode) {
        case ExternalValuesDataSetMode.AllValues:
          return dataset.GetAllValues();

        case ExternalValuesDataSetMode.OnlyLoadedValues:
          return dataset.GetLoadedValues();

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled dataset mode {datasetMode}.");

      }
    }

    #endregion Helpers

  }  // class ExternalValuesDataSetMapper

}  // namespace Empiria.FinancialAccounting.ExternalData.Adapters
