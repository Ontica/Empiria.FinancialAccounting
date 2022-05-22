/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Empiria Data Object                     *
*  Type     : GroupingRule                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data about a financial accounting grouping rule.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts {

  /// <summary>Contains data about a financial accounting grouping rule.</summary>
  public class GroupingRule : BaseObject {

    #region Fields

    private FixedList<GroupingRuleItem> items;

    #endregion Fields

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

    [DataField("ID_GRUPO")]
    public FinancialConceptGroup Group {
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

    public FixedList<GroupingRuleItem> Items {
      get {
        if (items == null) {
          items = Group.GetGroupingRuleItems(this);
        }
        return items;
      }
    }

    public int Level {
      get {
        return 1;
      }
    }

    #endregion Properties

    #region Methods

    internal void Cleanup() {
      this.Code = EmpiriaString.Clean(this.Code);
      this.Concept = EmpiriaString.Clean(this.Concept);
    }

    #endregion Methods

  } // class GroupingRule

}  // namespace Empiria.FinancialAccounting.FinancialConcepts
