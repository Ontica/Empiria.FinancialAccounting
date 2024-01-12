/* Empiria OnePoint ******************************************************************************************
*                                                                                                            *
*  Module   : Security Subjects Management                 Component : Web Api                               *
*  Assembly : Empiria.OnePoint.Security.WebApi.dll         Pattern   : Query Controller                      *
*  Type     : LogsController                               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web api query methods that retrieve information about logs.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.Storage;
using Empiria.WebApi;

using Empiria.OnePoint.Security.Reporting;

namespace Empiria.OnePoint.Security.WebApi {

  /// <summary>Web api query methods that retrieve information about logs.</summary>
  public class LogsController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v4/onepoint/security/management/operational-logs/excel")]
    public SingleObjectModel ExportOperationalLogToExcel([FromBody] OperationalLogReportQuery query) {

      base.RequireBody(query);

      using (var service = OperationalLogService.UseCaseInteractor()) {

        FixedList<LogEntryDto> logEntries = service.GetLogEntries(query);

        var exporter = new LogExcelExporter(query, logEntries);

        FileReportDto report = exporter.Export();

        base.SetOperation($"Se exportó a Excel la bitácora de {GetOperationalLogName(query.OperationLogType)}.");

        return new SingleObjectModel(base.Request, report);
      }
    }

    #endregion Web Apis

    private string GetOperationalLogName(LogOperationType operationLogType) {
      switch (operationLogType) {
        case LogOperationType.Successful:
          return "Accesos exitosos";
        case LogOperationType.Error:
          return "Accesos no exitosos";
        case LogOperationType.PermissionsManagement:
          return "Gestión de permisos";
        case LogOperationType.UserManagement:
          return "Gestión de usuarios";
        default:
          return "No determinada";
      }
    }


  }  // class LogsController

}  // namespace Empiria.OnePoint.Security.WebApi
