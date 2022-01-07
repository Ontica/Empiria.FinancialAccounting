/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : CalendarData                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for accounting calendars.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data access layer for accounting calendars.</summary>
  static internal class CalendarData {


    static internal void AppendPeriod(Calendar calendar, string name,
                                      DateTime fromDate, DateTime toDate) {
      Assertion.AssertObject(name, "name");

      long periodId = CommonMethods.GetNextObjectId("SEC_PERIOD_ID");

      var operation = DataOperation.Parse("apd_calendar_period",
                                          periodId, calendar.Id,
                                          name, fromDate, toDate);

      DataWriter.Execute(operation);
    }


    static internal FixedList<CalendarPeriod> CalendarPeriods(Calendar calendar) {
      var sql = "SELECT * FROM AO_PERIODS " +
               $"WHERE CALENDAR_ID = {calendar.Id} " +
               $"ORDER BY FROM_DATE";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<CalendarPeriod>(dataOperation);
    }


    static internal void RemovePeriod(CalendarPeriod period) {
      Assertion.AssertObject(period, "period");

      var operation = DataOperation.Parse("del_calendar_period",
                                          period.CalendarId, period.Id);

      DataWriter.Execute(operation);
    }

  }  // class CalendarData

}  // namespace Empiria.FinancialAccounting.Data
