/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Information Holder                      *
*  Type     : GroupingRuleItemDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Financial accounting grouping rule item data transfer object.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Rules.Adapters {

  /// <summary>Financial accounting rule item data transfer object.</summary>
  public class GroupingRuleItemDto {

    internal GroupingRuleItemDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public GroupingRuleItemType Type {
      get;
      internal set;
    }

    public string ItemName {
      get;
      internal set;
    }

    public string ItemCode {
      get;
      internal set;
    }

    public string SubledgerAccount {
      get;
      internal set;
    }

    public string SectorCode {
      get;
      internal set;
    }

    public string Operator {
      get;
      internal set;
    }

  }  // class GroupingRuleItemDto

}  // namespace Empiria.FinancialAccounting.Rules.Adapters
