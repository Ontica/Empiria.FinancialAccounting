/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : FinancialReportEntryDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the entries of a financial report.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO used to return the entries of a financial report.</summary>
  public class FinancialReportEntryDto : DynamicFinancialReportEntryDto {

    public string UID {
      get; internal set;
    }

    public FinancialReportItemType ItemType {
      get; internal set;
    }

    public string GroupingRuleUID {
      get; internal set;
    }

    public string ConceptCode {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public int Level {
      get; internal set;
    } = 1;

    public string AccountsChartName {
      get; internal set;
    }

    public string GroupName {
      get; internal set;
    }

    public override IEnumerable<string> GetDynamicMemberNames() {
      List<string> members = new List<string>();

      members.Add("UID");
      members.Add("ItemType");
      members.Add("GroupingRuleUID");
      members.Add("ConceptCode");
      members.Add("Concept");
      members.Add("AccountsChartName");
      members.Add("GroupName");

      members.AddRange(base.GetDynamicMemberNames());

      return members;
    }

  } // class FinancialReportEntryDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
