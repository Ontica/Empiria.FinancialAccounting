/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                 Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Rules.dll              Pattern   : Empiria Data Object                     *
*  Type     : GroupingRuleItem                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains data about a financial accounting grouping rule item.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Rules {

  /// <summary>Contains data about a financial accounting grouping rule item.</summary>
  public class GroupingRuleItem : BaseObject {

    #region Constructors and parsers

    protected GroupingRuleItem() {
      // Required by Empiria Framework.
    }

    static public GroupingRuleItem Parse(int id) {
      return BaseObject.ParseId<GroupingRuleItem>(id);
    }

    static public GroupingRuleItem Parse(string uid) {
      return BaseObject.ParseKey<GroupingRuleItem>(uid);
    }

    static public GroupingRuleItem Empty {
      get {
        return GroupingRuleItem.ParseEmpty<GroupingRuleItem>();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_CONCEPTO")]
    public GroupingRule GroupingRule {
      get; private set;
    }


    [DataField("REGLA_CALCULO")]
    public string CalculationRule {
      get; private set;
    }


    [DataField("REF_ID_CONCEPTO")]
    public GroupingRule Reference {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_ESTANDAR")]
    public string AccountNumber {
      get; private set;
    }


    [DataField("NUMERO_CUENTA_AUXILIAR")]
    public string SubledgerAccountNumber {
      get; private set;
    }


    [DataField("CLAVE_SECTOR")]
    public string SectorCode {
      get; private set;
    }


    [DataField("VALOR_DEFAULT")]
    public decimal DefaultValue {
      get; private set;
    }


    [DataField("OPERADOR")]
    public string Operator {
      get; private set;
    }

    #endregion Properties

  }  // class GroupingRuleItem

}  // namespace Empiria.FinancialAccounting.Rules
