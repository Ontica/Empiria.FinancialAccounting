/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Aggregate root                          *
*  Type     : Calendar                                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about an accounting calendar.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Data;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about an accounting calendar.</summary>
  internal class Calendar : BaseObject {

    static public Calendar Parse(int calendarId) {
      return BaseObject.ParseId<Calendar>(calendarId);
    }

    #region Properties

    public FixedList<CalendarPeriod> OpenedPeriods {
      get {
        return this.Periods.FindAll(x => x.IsOpened);
      }
    }


    public FixedList<CalendarPeriod> Periods {
      get {
        return CalendarData.CalendarPeriods(this);
      }
    }

    #endregion Properties

    #region Methods

    public FixedList<DateTime> OpenedAccountingDates() {
      var list = new List<DateTime>(16);

      foreach (var period in this.OpenedPeriods) {
        list.AddRange(period.GetDatesList().FindAll(x => !list.Contains(x)));
      }

      list.Sort();

      return list.ToFixedList();
    }

    #endregion Methods

  }  // class Calendar

}  // namespace Empiria.FinancialAccounting
