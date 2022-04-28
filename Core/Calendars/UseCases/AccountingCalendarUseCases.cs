/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : AccountingCalendarUseCases                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for accounting calendars management.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for accounting calendars management.</summary>
  public class AccountingCalendarUseCases : UseCase {

    #region Constructors and parsers

    protected AccountingCalendarUseCases() {
      // no-op
    }

    static public AccountingCalendarUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountingCalendarUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public AccountingCalendarDto AddPeriod(string calendarUID, AccountingCalendarPeriodDto period) {
      Assertion.AssertObject(calendarUID, "calendarUID");

      var calendar = Calendar.Parse(calendarUID);

      calendar.AddPeriod(period.Period, period.FromDate, period.ToDate);

      return AccountingCalendarMapper.Map(calendar);
    }


    public AccountingCalendarDto GetAccountingCalendar(string calendarUID) {
      Assertion.AssertObject(calendarUID, "calendarUID");

      var calendar = Calendar.Parse(int.Parse(calendarUID));

      return AccountingCalendarMapper.Map(calendar);
    }


    public FixedList<NamedEntityDto> GetAccountingCalendars() {
      var list = Calendar.GetList();

      return list.MapToNamedEntityList();
    }


    public AccountingCalendarDto RemovePeriod(string calendarUID, string periodUID) {
      Assertion.AssertObject(calendarUID, "calendarUID");
      Assertion.AssertObject(periodUID, "periodUID");

      var calendar = Calendar.Parse(calendarUID);

      CalendarPeriod period = calendar.GetPeriod(periodUID);

      calendar.RemovePeriod(period);

      return AccountingCalendarMapper.Map(calendar);
    }

    #endregion Use cases

  }  // class AccountingCalendarUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
