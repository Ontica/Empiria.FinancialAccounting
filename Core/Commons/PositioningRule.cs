﻿/* Empiria Financial *****************************************************************************************
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


  /// <summary>Payload that describes an item positioning rule to store it in an ordered list.</summary>
  public class ItemPositioning {

    public PositioningRule Rule {
      get; set;
    } = PositioningRule.Undefined;


    public string OffsetUID {
      get; set;
    } = string.Empty;


    public int Position {
      get; set;
    } = -1;


    public void Clean() {
      // no-op
    }

    public void Require() {
      if (Rule.UsesOffset()) {
        Assertion.Require(OffsetUID.Length != 0,
                $"payload.Positioning.Rule is '{Rule}', so Positioning.OffsetUID must be provided.");
      }

      if (Rule.UsesPosition()) {
        Assertion.Require(Position != -1,
                $"payload.Positioning.Rule is '{Rule}', so Positioning.Position must be provided.");

        Assertion.Require(Position > 0,
                $"payload.PositioningRule is '{Rule}', so Positioning.Position must be greater than zero.");
      }
    }


    public void SetIssues(ExecutionResult result) {
      // no-op
    }


    public void SetWarnings(ExecutionResult result) {
      result.AddWarningIf(Rule == PositioningRule.Undefined && OffsetUID.Length != 0,
         $"payload.Positioning.Rule is '{Rule}', but Positioning.OffsetUID value was supplied.");

      result.AddWarningIf(Rule == PositioningRule.Undefined && Position > 0,
        $"payload.Positioning.Rule is '{Rule}', but Positioning.Position value was supplied.");

      result.AddWarningIf(Rule.UsesOffset() && Position != -1,
        $"payload.Positioning.Rule is '{Rule}', but Positioning.Position value was supplied.");

      result.AddWarningIf(Rule.UsesPosition() && OffsetUID.Length != 0,
        $"payload.Positioning.Rule is '{Rule}', but Positioning.OffsetUID value was supplied.");
    }

  }  // class ItemPositioning



  /// <summary>Extension methods for PositioningRule enumeration.</summary>
  public static class PositioningRuleExtensions {

    static public bool UsesOffset(this PositioningRule rule) {
      return rule == PositioningRule.AfterOffset || rule == PositioningRule.BeforeOffset;
    }

    static public bool UsesPosition(this PositioningRule rule) {
      return rule == PositioningRule.ByPositionValue;
    }

  }  // class PositioningRuleExtensions


}  // namespace Empiria.FinancialAccounting