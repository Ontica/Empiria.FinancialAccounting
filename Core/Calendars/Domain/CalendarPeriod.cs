/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Aggregate root                          *
*  Type     : Calendar                                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about an accounting calendar operation period.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting {

  /// <summary>Holds information about an accounting calendar operation period.</summary>
  internal class CalendarPeriod : BaseObject {

    static public CalendarPeriod Parse(int calendarPeriodId) {
      return BaseObject.ParseId<CalendarPeriod>(calendarPeriodId);
    }

    #region Properties

    [DataField("CALENDAR_ID", ConvertFrom = typeof(long))]
    public int CalendarId {
      get;
      private set;
    }


    [DataField("PERIOD_NAME")]
    public string Name {
      get;
      private set;
    }


    [DataField("FROM_DATE")]
    public DateTime FromDate {
      get;
      private set;
    }


    [DataField("TO_DATE")]
    public DateTime ToDate {
      get;
      private set;
    }


    [DataField("IS_OPEN", ConvertFrom = typeof(int))]
    public bool IsOpened {
      get;
      private set;
    }


    #endregion Properties

    #region Methods

    internal FixedList<DateTime> GetDatesList() {
      var list = new List<DateTime>();

      for (DateTime date = this.FromDate.Date; date <= this.ToDate.Date; date = date.AddDays(1)) {
        list.Add(date.Date);
      }
      return list.ToFixedList();
    }

    #endregion Methods

  }  // class CalendarPeriod

}  // namespace Empiria.FinancialAccounting
