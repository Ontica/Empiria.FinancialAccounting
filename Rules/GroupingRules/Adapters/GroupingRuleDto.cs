/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Information Holder                      *
*  Type     : GroupingRuleDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Financial accounting grouping rule data transfer object.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Rules.Adapters {

  /// <summary>Financial accounting rule data transfer object.</summary>
  public class GroupingRuleDto {

    internal GroupingRuleDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Code {
      get; internal set;
    }

    public string Concept {
      get; internal set;
    }

    public int Position {
      get; internal set;
    }

    public int Level {
      get; internal set;
    }

    public string ParentUID {
      get; internal set;
    }

  }  // class GroupingRuleDto

}  // namespace Empiria.FinancialAccounting.Rules.Adapters
