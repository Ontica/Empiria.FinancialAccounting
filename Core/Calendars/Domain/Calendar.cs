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


    static internal Calendar Parse(string calendarUID) {
      return Calendar.Parse(int.Parse(calendarUID));
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

    internal void AddPeriod(string name, DateTime fromDate, DateTime toDate) {
      Assertion.Require(name, "name");

      Assertion.Require(fromDate <= toDate,
          "La fecha de inicio del período debe ser menor o igual a la fecha final.");

      CalendarData.AppendPeriod(this, name, fromDate, toDate);
    }


    internal CalendarPeriod GetPeriod(string periodUID) {
      Assertion.Require(periodUID, "periodUID");

      CalendarPeriod period = this.Periods.Find(x => x.UID == periodUID);

      Assertion.Require(period,
            $"No se encontró un período con identificador {periodUID} en el calendario {this.Name}.");

      return period;
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


    internal void RemovePeriod(CalendarPeriod period) {
      Assertion.Require(period, "period");
      Assertion.Require(this.Periods.Contains(period),
            $"El período {period.Name} no partenece al calendario {this.Name}.");

      CalendarData.RemovePeriod(period);
    }

    #endregion Methods

  }  // class Calendar

}  // namespace Empiria.FinancialAccounting
