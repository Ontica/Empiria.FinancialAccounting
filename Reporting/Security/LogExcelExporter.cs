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

using Empiria.Office;
using Empiria.Storage;

namespace Empiria.OnePoint.Security.Reporting {

  public class LogExcelExporter {

    private readonly OperationalLogReportQuery _query;
    private readonly FixedList<LogEntryDto> _logEntries;
    private readonly FileTemplateConfig _template;

    public LogExcelExporter(OperationalLogReportQuery query, FixedList<LogEntryDto> logEntries) {
      _query = query;
      _logEntries = logEntries;
      _template = GetExcelTemplate();
    }


    public FileReportDto Export() {
      var excelFile = new ExcelFile(_template);

      excelFile.Open();

      SetHeader(excelFile);

      FillOutRows(excelFile);

      excelFile.Save();

      excelFile.Close();

      return excelFile.ToFileReportDto();
    }


    #region Private methods

    private void FillOutRows(ExcelFile excelFile) {

      switch (_query.OperationLogType) {
        case LogOperationType.Successful:
          FillOutRowsSuccessful(excelFile);
          break;
        case LogOperationType.Error:
          FillOutRowsError(excelFile);
          break;
        case LogOperationType.PermissionsManagement:
          FillOutRowsPermissionsManagement(excelFile);
          break;
        case LogOperationType.UserManagement:
          FillOutRowsUserManagement(excelFile);
          break;
        default:
          throw Assertion.EnsureNoReachThisCode(
                  $"Unrecognized operation log type '{_query.OperationLogType}'.");
      }
    }


    private void FillOutRowsUserManagement(ExcelFile excelFile) {
      int i = 5;

      foreach (var entry in _logEntries) {
        excelFile.SetCell($"A{i}", entry.DateTime);
        excelFile.SetCell($"B{i}", entry.SessionId);
        excelFile.SetCell($"C{i}", entry.EmployeeID);
        excelFile.SetCell($"D{i}", entry.UserName);
        excelFile.SetCell($"E{i}", entry.UserHostAddress);
        excelFile.SetCell($"F{i}", entry.Operation);
        excelFile.SetCell($"G{i}", entry.Subject.FullName);
        excelFile.SetCell($"H{i}", entry.SubjectEmployeeID);
        i++;
      }
    }

    private void FillOutRowsPermissionsManagement(ExcelFile excelFile) {
      int i = 5;

      foreach (var entry in _logEntries) {
        excelFile.SetCell($"A{i}", entry.DateTime);
        excelFile.SetCell($"B{i}", entry.SessionId);
        excelFile.SetCell($"C{i}", entry.EmployeeID);
        excelFile.SetCell($"D{i}", entry.UserName);
        excelFile.SetCell($"E{i}", entry.UserHostAddress);
        excelFile.SetCell($"F{i}", entry.Operation);
        excelFile.SetCell($"G{i}", entry.SubjectObject);
        excelFile.SetCell($"H{i}", entry.Subject.FullName);
        excelFile.SetCell($"I{i}", entry.SubjectEmployeeID);
        i++;
      }
    }

    private void FillOutRowsError(ExcelFile excelFile) {
      int i = 5;

      foreach (var entry in _logEntries) {
        excelFile.SetCell($"A{i}", entry.DateTime);
        excelFile.SetCell($"B{i}", entry.SessionId);
        excelFile.SetCell($"C{i}", entry.EmployeeID);
        excelFile.SetCell($"D{i}", entry.UserName);
        excelFile.SetCell($"E{i}", entry.UserHostAddress);
        excelFile.SetCell($"F{i}", entry.Operation);
        excelFile.SetCell($"G{i}", entry.Description);
        excelFile.SetCell($"H{i}", entry.Exception);
        i++;
      }
    }

    private void FillOutRowsSuccessful(ExcelFile excelFile) {
      int i = 5;

      foreach (var entry in _logEntries) {
        excelFile.SetCell($"A{i}", entry.DateTime);
        excelFile.SetCell($"B{i}", entry.SessionId);
        excelFile.SetCell($"C{i}", entry.EmployeeID);
        excelFile.SetCell($"D{i}", entry.UserName);
        excelFile.SetCell($"E{i}", entry.UserHostAddress);
        excelFile.SetCell($"F{i}", entry.Operation);
        excelFile.SetCell($"G{i}", entry.Description);
        i++;
      }
    }

    private FileTemplateConfig GetExcelTemplate() {
      var templateUID = $"OperationalLogReportTemplate.{_query.OperationLogType}";

      return FileTemplateConfig.Parse(templateUID);
    }


    private void SetHeader(ExcelFile excelFile) {
      excelFile.SetCell($"A2", _template.Title);

      var subTitle = $"Del {_query.FromDate.ToString("dd/MMM/yyyy")} al {_query.ToDate.ToString("dd/MMM/yyyy")}";

      excelFile.SetCell($"A3", subTitle);
    }

    #endregion Private methods
  }  // class LogExcelExporter

}  // namespace Empiria.OnePoint.Security.Reporting
