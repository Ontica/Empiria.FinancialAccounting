/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Abstract class                          *
*  Type     : BaseReportType                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Abstract class that describes a report type.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Abstract class that describes a report type.</summary>
  public abstract class BaseReportType : GeneralObject {

    protected BaseReportType() {
      // Required by Empiria Framework.
    }

    static public BaseReportType Parse(int id) {
      return BaseObject.ParseId<BaseReportType>(id);
    }


    static public BaseReportType Parse(string uid) {
      return BaseObject.ParseKey<BaseReportType>(uid);
    }

    static public FixedList<BaseReportType> GetList() {
      return BaseObject.GetList<BaseReportType>(string.Empty, "ObjectName")
                       .ToFixedList();
    }

  }  // class BaseReportType

}  // namespace Empiria.FinancialAccounting.FinancialReports
