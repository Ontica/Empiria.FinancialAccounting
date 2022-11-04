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

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Methods used to map ReportType instances.</summary>
  static internal class ReportTypeMapper {

    static internal FixedList<ReportTypeDto> Map(FixedList<ReportType> list) {
      var mappedList = list.Select(x => Map(x));

      return new FixedList<ReportTypeDto>(mappedList);
    }


    static private ReportTypeDto Map(ReportType reportType) {
      return new ReportTypeDto {
        UID = reportType.UID,
        Name = reportType.Name,
        Group = reportType.Group,
        PayloadType = reportType.PayloadType,
        AccountsCharts = reportType.AccountsCharts.Select(x => x.UID)
                                                  .ToArray(),
        Show = reportType.Show, 
        ExportTo = reportType.ExportTo.ToArray()
      };
    }

  }  // class ReportTypeMapper

}  // namespace Empiria.FinancialAccounting.Reporting
