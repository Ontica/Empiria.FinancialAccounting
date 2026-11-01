/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Information holder                    *
*  Type     : ClassificationEntry                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : A selectable classification linked to a standard account.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/


using Empiria.Json;
using Empiria.Parties;
using Empiria.StateEnums;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>A selectable classification linked to a standard account.</summary>
  public sealed class ClassificationEntry : BaseObject {

    #region Constructors and parsers

    private ClassificationEntry() {
      // Required by Empiria Framework.
    }

    static public ClassificationEntry Parse(int id) => ParseId<ClassificationEntry>(id);

    static public ClassificationEntry Parse(string uid) => ParseKey<ClassificationEntry>(uid);

    static public ClassificationEntry Empty => ParseEmpty<ClassificationEntry>();

    #endregion Constructors and parsers

    #region Properties

    [DataField("ACR_ENTRY_RULE_ID")]
    public ClassificationRule Rule {
      get; private set;
    }


    [DataField("ACR_ENTRY_ROLE_CODE")]
    public string RoleCode {
      get; private set;
    }


    [DataField("ACR_ENTRY_STD_ACCOUNT_ID")]
    public StandardAccount StandardAccount {
      get; private set;
    }


    [DataField("ACR_ENTRY_EXT_DATA")]
    public JsonObject ExtData {
      get; private set;
    }


    [DataField("ACR_ENTRY_POSITION")]
    public int Position {
      get; private set;
    }


    [DataField("ACR_ENTRY_POSTED_BY_ID")]
    public Party PostedBy {
      get; private set;
    }


    [DataField("ACR_ENTRY_STATUS", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get; private set;
    }

    #endregion Properties

  }  // class ClassificationEntry

}  // namespace Empiria.FinancialAccounting.AccountingRules
