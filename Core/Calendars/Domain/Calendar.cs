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
  internal class Calendar : BaseObject, INamedEntity {

    #region Constructors and parsers

    protected Calendar() {

    }

    static public Calendar Parse(int calendarId) {
      return BaseObject.ParseId<Calendar>(calendarId);
    }


    static public FixedList<Calendar> GetList() {
      return BaseObject.GetList<Calendar>()
                       .ToFixedList();
    }


    #endregion Constructors and parsers

    #region Properties

    [DataField("CALENDAR_NAME")]
    public string Name {
      get;
      private set;
    }


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

    internal void AddAccountingDate(DateTime date) {
      CalendarData.AddAccountingDate(this, date);
    }


    public FixedList<DateTime> OpenedAccountingDates() {
      var list = new List<DateTime>(16);

      foreach (var period in this.OpenedPeriods) {
        var periodDates = period.GetDatesList();
        var uniqueDates = periodDates.FindAll(x => !list.Contains(x));

        list.AddRange(uniqueDates);
      }

      list.Sort();

      return list.ToFixedList();
    }


    internal void RemoveAccountingDate(DateTime date) {
      CalendarData.RemoveAccountingDate(this, date);
    }

    #endregion Methods

  }  // class Calendar

}  // namespace Empiria.FinancialAccounting
