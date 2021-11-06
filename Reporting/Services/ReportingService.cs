/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : ReportingService                              License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services used to generate financial accounting reports.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides services used to generate financial accounting reports.</summary>
  public class ReportingService : Service {

    #region Constructors and parsers

    private ReportingService() {
      // no-op
    }

    static public ReportingService ServiceInteractor() {
      return Service.CreateInstance<ReportingService>();
    }

    #endregion Constructors and parsers

    #region Services

    public FileReportDto ExportReport(GenerateReportCommand command) {
      Assertion.AssertObject(command, "command");

      throw new NotImplementedException("ExportReport");
    }


    public ReportDataDto GenerateReport(GenerateReportCommand command) {
      Assertion.AssertObject(command, "command");

      throw new NotImplementedException("GenerateReport");
    }


    public FixedList<ReportTypeDto> GetReportTypes() {
      throw new NotImplementedException("GetReportTypes");
    }

    #endregion Services

  } // class ReportingService

} // namespace Empiria.FinancialAccounting.Reporting
