/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : AccountingCalendarMapper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for accounting calendars.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for accounting calendars.</summary>
  static internal class AccountingCalendarMapper {

    static internal AccountingCalendarDto Map(Calendar calendar) {
      return new AccountingCalendarDto() {
         UID = calendar.UID,
         Name = calendar.Name,
         Periods = MapPeriods(calendar.OpenedPeriods),
         OpenedAccountingDates = calendar.OpenedAccountingDates()
      };
    }


    static private FixedList<AccountingCalendarPeriodDto> MapPeriods(FixedList<CalendarPeriod> periods) {
      return new FixedList<AccountingCalendarPeriodDto>(periods.Select(x => MapPeriod(x)));
    }


    static private AccountingCalendarPeriodDto MapPeriod(CalendarPeriod period) {
      return new AccountingCalendarPeriodDto {
        UID = period.UID,
        Period = period.Name,
        FromDate = period.FromDate,
        ToDate = period.ToDate,
      };
    }

  }  // class AccountingCalendarMapper

}  // namespace Empiria.FinancialAccounting.Adapters
