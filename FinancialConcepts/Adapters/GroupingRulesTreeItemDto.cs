/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : GroupingRuleTreeItemDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data transfer object for grouping rules tree items.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Data transfer object for grouping rules tree items.</summary>
  public class GroupingRulesTreeItemDto {

    internal GroupingRulesTreeItemDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public IntegrationEntryType Type {
      get; internal set;
    }

    public string ItemName {
      get; internal set;
    }

    public string ItemCode {
      get; internal set;
    }

    public string SubledgerAccount {
      get; internal set;
    }

    public string SubledgerAccountName {
      get; internal set;
    }

    public string SectorCode {
      get; internal set;
    }

    public string CurrencyCode {
      get; internal set;
    }

    public string Operator {
      get; internal set;
    }

    public string Qualification {
      get; internal set;
    }

    public string ParentCode {
      get; internal set;
    }

    public int Level {
      get; internal set;
    }

  }  // class GroupingRuleTreeItemDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
