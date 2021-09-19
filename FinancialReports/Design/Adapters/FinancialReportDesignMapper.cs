/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Mapper class                            *
*  Type     : FinancialReportDesignMapper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map financial reports design data.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Methods used to map financial reports design data.</summary>
  static internal class FinancialReportDesignMapper {

    #region Public mappers

    static internal FinancialReportDesignDto Map(FinancialReportType financialReportType) {
      return new FinancialReportDesignDto {
        Rows = MapRows(financialReportType),
      };
    }

    #endregion Public mappers

    #region Helpers

    static private FixedList<FinancialReportRowDto> MapRows(FinancialReportType financialReportType) {
      var rows = financialReportType.GetRows();

      var mappedRows = rows.Select((x) => MapRow(x));

      return new FixedList<FinancialReportRowDto>(mappedRows);
    }

    static private FinancialReportRowDto MapRow(FinancialReportRow row) {
      return new FinancialReportRowDto {
        UID = row.UID,
        Code = row.Code,
        Label = row.Label,
        Format = row.Format,
        Position = row.Position,
        GroupingRuleUID = row.GroupingRule.UID
      };
    }

    #endregion Helpers

  } // class FinancialReportDesignMapper

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
