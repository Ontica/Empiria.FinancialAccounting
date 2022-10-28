/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Mapper class                            *
*  Type     : ExternalValuesDataSetMapper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for financial external values.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Mapping methods for financial external values.</summary>
  static internal class ExternalValuesDataSetMapper {

    static internal ExternalValuesDto Map(ExternalValuesQuery query, ExternalValuesDataSet dataset) {
      return new ExternalValuesDto {
        Query = query,
        Columns = dataset.Set.DataColumns,
        Entries = new FixedList<ExternalValuesEntryDto>()
      };
    }

  }  // class ExternalValuesDataSetMapper

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
