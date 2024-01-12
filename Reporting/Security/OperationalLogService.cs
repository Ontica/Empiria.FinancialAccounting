/* Empiria OnePoint ******************************************************************************************
*                                                                                                            *
*  Module   : Reporting                                    Component : Use case                              *
*  Assembly : Empiria.OnePoint.Security.dll                Pattern   : Use case interactor                   *
*  Type     : OperationalLogService                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Retrieves operational logs.                                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Data;
using Empiria.Services;

namespace Empiria.OnePoint.Security.Reporting {

  /// <summary>Retrieves operational logs.</summary>
  public class OperationalLogService : UseCase {

    #region Constructors and parsers

    static public OperationalLogService UseCaseInteractor() {
      return CreateInstance<OperationalLogService>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<LogEntryDto> GetLogEntries(OperationalLogReportQuery query) {
      Assertion.Require(query, nameof(query));

      return ReadLogEntries(query);
    }

    #endregion Use cases

    private FixedList<LogEntryDto> ReadLogEntries(OperationalLogReportQuery query) {
      var sql = "SELECT * FROM OperationsLog " +
               $"WHERE LogType = '{query.OperationLogType}' AND " +
               $"{FormatSqlDbDate(query.FromDate)} <= LogTimeStamp AND " +
               $"LogTimeStamp < {FormatSqlDbDate(query.ToDate.AddDays(1))} " +
               $"ORDER BY LogTimeStamp DESC";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<LogEntryDto>(op);
    }

    static public string FormatSqlDbDate(DateTime date) {
      string dateAsString = date.Date.ToString("yyyy-MM-dd");

      return $"TO_DATE('{dateAsString}', 'yyyy-MM-dd')";
    }


  }  // class OperationalLogService

} // namespace Empiria.OnePoint.Security.Reporting
