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
using System.Collections;

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


  public interface IPositionable : IIdentifiable {

    int Position { get; }

  }

  /// <summary>Payload that describes an entity positioning rule to store it in an ordered list.</summary>
  public class Positioning {

    public PositioningRule Rule {
      get; set;
    } = PositioningRule.Undefined;


    public string OffsetUID {
      get; set;
    } = string.Empty;


    public int Position {
      get; set;
    } = -1;


    private IPositionable OffsetObject;

    public IIdentifiable GetOffsetObject<T>() where T : IPositionable {
      Assertion.Require(OffsetObject, "OffsetObject was not provieded." +
                        "Please before call method SetOffsetObject().");

      return (T) OffsetObject;
    }


    public void SetOffsetObject(IPositionable offsetObject) {
      Assertion.Require(offsetObject, nameof(offsetObject));

      this.OffsetObject = offsetObject;
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
      result.AddWarningIf(Rule == PositioningRule.Undefined && OffsetUID.Length != 0,
          $"payload.Positioning.Rule is '{Rule}', but Positioning.OffsetUID value was supplied.");

      result.AddWarningIf(Rule == PositioningRule.Undefined && Position > 0,
          $"payload.Positioning.Rule is '{Rule}', but Positioning.Position value was supplied.");

      result.AddWarningIf((Rule.UsesOffset() || Rule.IsAbsolute()) && Position != -1,
          $"payload.Positioning.Rule is '{Rule}', but Positioning.Position value was supplied.");

      result.AddWarningIf((Rule.UsesPosition() || Rule.IsAbsolute()) && OffsetUID.Length != 0,
          $"payload.Positioning.Rule is '{Rule}', but Positioning.OffsetUID value was supplied.");


    }


    public int CalculatePosition(ICollection orderedList, int currentPosition = -1) {
      switch (this.Rule) {

        case PositioningRule.AfterOffset:

          if (currentPosition != -1 &&
              currentPosition < OffsetObject.Position) {
            return OffsetObject.Position;
          } else {
            return OffsetObject.Position + 1;
          }


        case PositioningRule.AtEnd:

          if (currentPosition != -1) {
            return orderedList.Count;
          } else {
            return orderedList.Count + 1;
          }


        case PositioningRule.AtStart:
          return 1;


        case PositioningRule.BeforeOffset:

          if (currentPosition != -1 &&
              currentPosition < OffsetObject.Position) {
            return OffsetObject.Position - 1;
          } else {
            return OffsetObject.Position;
          }


        case PositioningRule.ByPositionValue:
          Assertion.Require(1 <= this.Position &&
                                this.Position <= orderedList.Count + 1,
            $"Position value is {this.Position}, " +
            $"but must be between 1 and {orderedList.Count + 1}.");

          return this.Position;

        default:
          throw Assertion.EnsureNoReachThisCode($"Unhandled PositioningRule '{this.Rule}'.");
      }
    }

  }  // class ItemPositioning



  /// <summary>Extension methods for PositioningRule enumeration.</summary>
  public static class PositioningRuleExtensions {

    static public bool IsAbsolute(this PositioningRule rule) {
      return rule == PositioningRule.AtStart || rule == PositioningRule.AtEnd;
    }

    static public bool UsesOffset(this PositioningRule rule) {
      return rule == PositioningRule.AfterOffset || rule == PositioningRule.BeforeOffset;
    }

    static public bool UsesPosition(this PositioningRule rule) {
      return rule == PositioningRule.ByPositionValue;
    }

  }  // class PositioningRuleExtensions

}  // namespace Empiria.FinancialAccounting
