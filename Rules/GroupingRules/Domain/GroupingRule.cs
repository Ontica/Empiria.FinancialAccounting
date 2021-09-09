/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Empiria Data Object                     *
*  Type     : GroupingRule                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data about a financial accounting grouping rule.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Rules {

  /// <summary>Contains data about a financial accounting grouping rule.</summary>
  public class GroupingRule : BaseObject {

    #region Constructors and parsers

    protected GroupingRule() {
      // Required by Empiria Framework.
    }

    static public GroupingRule Parse(int id) {
      return BaseObject.ParseId<GroupingRule>(id);
    }

    static public GroupingRule Parse(string uid) {
      return BaseObject.ParseKey<GroupingRule>(uid);
    }

    static public GroupingRule Empty {
      get {
        return GroupingRule.ParseEmpty<GroupingRule>();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("CONJUNTO_BASE")]
    public string RulesSetCode {
      get; private set;
    }

    [DataField("CLAVE_CONCEPTO")]
    public string Code {
      get; private set;
    }

    [DataField("NOMBRE_CONCEPTO")]
    public string Concept {
      get; private set;
    }

    [DataField("POSICION")]
    public int Position {
      get; private set;
    }

    [DataField("FECHA_INICIO")]
    public DateTime StartDate {
      get; private set;
    }

    [DataField("FECHA_FIN")]
    public DateTime EndDate {
      get; private set;
    }

    public int Level {
      get {
        return 1;
      }
    }

    public GroupingRule Parent {
      get {
        return GroupingRule.Empty;
      }
    }

    #endregion Properties

    #region Methods

    #endregion Methods

  } // class GroupingRule

}  // namespace Empiria.FinancialAccounting.Rules
