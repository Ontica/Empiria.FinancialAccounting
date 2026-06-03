/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : AccountingRule                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about an accounting rule.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.StateEnums;

using Empiria.FinancialAccounting.Reclassification.Data;

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Holds information about an accounting rule.</summary>
  public class AccountingRule {

    #region Constructors and parsers

    private AccountingRule() {
      // Required by Empiria Framework
    }

    static public FixedList<AccountingRule> GetList() {
      return ReclassificationDataService.GetAccountingRules();
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_REGLA")]
    public int Id {
      get; private set;
    }


    [DataField("UID_REGLA")]
    public string UID {
      get; private set;
    }


    [DataField("ID_TIPO_OPERACION")]
    public AccountingOperationType AccountingOperationType {
      get; private set;
    }


    [DataField("CUENTA_DEBE")]
    public string DebitAccount {
      get; private set;
    }


    [DataField("NOMBRE_CUENTA_DEBE")]
    public string DebitAccountName {
      get; private set;
    }


    [DataField("CUENTA_HABER")]
    public string CreditAccount {
      get; private set;
    }


    [DataField("NOMBRE_CUENTA_HABER")]
    public string CreditAccountName {
      get; private set;
    }


    [DataField("STATUS", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get; private set;
    }

    #endregion Properties

  } // class AccountingRule

} // namespace Empiria.FinancialAccounting.Reclassification
