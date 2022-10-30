/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Use cases Layer                         *
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

    static internal ExternalValuesDto Map(ExternalValuesQuery query, ExternalValuesDataSet dataset) {
      return new ExternalValuesDto {
        Query = query,
        Columns = dataset.VariablesSet.DataColumns,
        Entries = Map(dataset.GetAllValues()),
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

    static private void SetTotalsFields(ExternalValuesEntryDto o, ExternalValueDatasetEntry entry) {
      var dynamicFieldNames = entry.Values.GetDynamicMemberNames();

      foreach (string fieldName in dynamicFieldNames) {
        o.SetTotalField(fieldName, entry.Values.GetTotalField(fieldName));
      }
    }


  }  // class ExternalValuesDataSetMapper

}  // namespace Empiria.FinancialAccounting.ExternalData.Adapters
