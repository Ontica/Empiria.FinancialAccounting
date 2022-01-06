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

    static internal void AddAccountingDate(Calendar calendar, DateTime date) {
      //var dataOperation = DataOperation.Parse("");

      //DataWriter.Execute(dataOperation);
    }


    static internal FixedList<CalendarPeriod> CalendarPeriods(Calendar calendar) {
      var sql = "SELECT * FROM AO_PERIODS " +
               $"WHERE CALENDAR_ID = {calendar.Id} " +
               $"ORDER BY FROM_DATE";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<CalendarPeriod>(dataOperation);
    }


    static internal void RemoveAccountingDate(Calendar calendar, DateTime date) {
      // throw new NotImplementedException();
    }


  }  // class CalendarData

}  // namespace Empiria.FinancialAccounting.Data
