/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Enumeration                             *
*  Type     : PositioningRule                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a positioning rule to maintain entities ordered when they are edited.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Describes a positioning rule to maintain entities ordered when they are edited.</summary>
  public enum PositioningRule {

    AtStart,

    BeforeOffset,

    AfterOffset,

    AtEnd,

    ByPositionValue,

    Undefined

  }  // enum PositioningRule



  /// <summary>Extension methods for PositioningRule.</summary>
  public static class PositioningRuleExtensions {

    static public bool UsesOffset(this PositioningRule rule) {
      return rule == PositioningRule.AfterOffset || rule == PositioningRule.BeforeOffset;
    }


    static public bool UsesPosition(this PositioningRule rule) {
      return rule == PositioningRule.ByPositionValue;
    }

  }  // class PositioningRuleExtensions

}  // namespace Empiria.FinancialAccounting
