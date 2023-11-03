/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Mapper class                         *
*  Type     : ReportTypeMapper                              License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Methods used to map ReportType instances.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.FinancialAccounting.FinancialReports;
using Empiria.Storage;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Methods used to map ReportType instances.</summary>
  static internal class ReportTypeMapper {

    static internal FixedList<ReportTypeDto> Map(FixedList<BaseReportType> list) {
      var mappedList = list.Select(x => Map(x))
                           .ToFixedList();

      mappedList.Sort((x, y) => x.Name.CompareTo(y.Name));

      return mappedList;
    }


    static private ReportTypeDto Map(BaseReportType reportType) {
      if (reportType is ReportType rt) {
        return Map(rt);
      } else if (reportType is FinancialReportType frt) {
        return Map(frt);
      } else {
        throw Assertion.EnsureNoReachThisCode($"Unhandled report type {reportType.GetType().FullName}.");
      }
    }


    static private ReportTypeDto Map(ReportType reportType) {
      return new ReportTypeDto {
        UID = reportType.UID,
        Name = reportType.Name,
        Group = reportType.Group,
        PayloadType = reportType.PayloadType,
        AccountsCharts = reportType.AccountsCharts.Select(x => x.UID)
                                                  .ToArray(),
        OutputType = reportType.OutputType,
        Show = reportType.Show,
        ExportTo = ExportToMapper.Map(reportType.ExportTo)
      };
    }


    static private ReportTypeDto Map(FinancialReportType reportType) {
      return new ReportTypeDto {
        UID = reportType.UID,
        Name = reportType.Name,
        Group = reportType.Group,
        AccountsCharts = new[] { reportType.AccountsChart.UID },
        ExportTo = ExportToMapper.Map(reportType.ExportTo),
        Show = new ReportTypeActions {
          SingleDate = true,
        }
      };
    }

  }  // class ReportTypeMapper

}  // namespace Empiria.FinancialAccounting.Reporting
