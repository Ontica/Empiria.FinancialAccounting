/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Data Transfer Object                    *
*  Type     : IntegrationReportEntryDto                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return the integration entries of a financial report.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.FinancialConcepts;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Output DTO used to return the integration entries of a financial report.</summary>
  public class IntegrationReportEntryDto : DynamicFinancialReportEntryDto {

    public string UID {
      get; internal set;
    }

    public GroupingRuleItemType Type {
      get; internal set;
    }

    public string FinancialConceptUID {
      get; internal set;
    }

    public string ConceptCode {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public FinancialReportItemType ItemType {
      get; internal set;
    }

    public string ItemCode {
      get; internal set;
    }

    public string ItemName {
      get; internal set;
    }

    public string SubledgerAccount {
      get; internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    public string Operator {
      get; internal set;
    }

    public override IEnumerable<string> GetDynamicMemberNames() {
      List<string> members = new List<string>();

      members.Add("UID");
      members.Add("Type");
      members.Add("FinancialConceptUID");
      members.Add("ConceptCode");
      members.Add("Concept");
      members.Add("ItemType");
      members.Add("ItemCode");
      members.Add("ItemName");
      members.Add("SubledgerAccount");
      members.Add("SectorCode");
      members.Add("Operator");

      members.AddRange(base.GetDynamicMemberNames());

      return members;
    }

  } // class IntegrationReportEntryDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
